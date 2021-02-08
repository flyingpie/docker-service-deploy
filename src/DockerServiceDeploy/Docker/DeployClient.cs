using Docker.DotNet;
using Docker.DotNet.Models;
using DockerServiceDeploy.Exceptions;
using DockerServiceDeploy.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DockerServiceDeploy.Docker
{
	public class DeployClient
	{
		public TimeSpan DeployTimeout = TimeSpan.FromMinutes(2);

		private readonly DockerClient _client;
		private readonly AuthConfig _authConfig;

		public DeployClient(DockerClient client, AuthConfig authConfig)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
			_authConfig = authConfig ?? throw new ArgumentNullException(nameof(authConfig));
		}

		public async Task DeployAsync(ServiceSpec serviceSpec)
		{
			// Check if service already exists
			var services = await _client.Swarm.ListServicesAsync();
			var service = services.FirstOrDefault(x => x.Spec.Name == serviceSpec.Name);

			if (service == null)
			{
				// Create service
				Log.Information($"Creating service '{serviceSpec.Name}'...");

				var create = new ServiceCreateParameters()
				{
					RegistryAuth = _authConfig,
					Service = serviceSpec
				};

				await _client.Swarm.CreateServiceAsync(create);
			}
			else
			{
				// Update service
				Log.Information($"Updating service '{serviceSpec.Name}'...");

				var update = new ServiceUpdateParameters()
				{
					RegistryAuth = _authConfig,
					Service = serviceSpec,
					Version = (long)service.Version.Index
				};

				await _client.Swarm.UpdateServiceAsync(service.ID, update);
			}

			await WaitForDeploymentToCompleteAsync(serviceSpec.Name, new CancellationTokenSource(DeployTimeout).Token);
		}

		public async Task WaitForDeploymentToCompleteAsync(string serviceName, CancellationToken cancellation = default)
		{
			SwarmService? service = null;

			while (service == null && !cancellation.IsCancellationRequested)
			{
				service = (await _client.Swarm.ListServicesAsync()).FirstOrDefault(x => x.Spec.Name == serviceName);

				await Task.Delay(TimeSpan.FromSeconds(1));
			}

			if (service == null) throw new Exception($"Could not get service tasks");

			await WaitForTasksToReachDesiredStateAsync(service.ID, cancellation);

			Log.Information("Done");
		}

		/// <summary>
		/// Deploys a service and waits for it to complete.
		/// Based on the docker cli "service create" and "service update" commands.
		///
		/// https://github.com/docker/classicswarm/blob/master/vendor/github.com/docker/docker/api/types/swarm/service.go
		/// </summary>
		public async Task WaitForTasksToReachDesiredStateAsync(string serviceId, CancellationToken cancellationToken = default)
		{
			var start = DateTime.UtcNow;
			var loopDelay = TimeSpan.FromSeconds(3);

			while (!cancellationToken.IsCancellationRequested)
			{
				// Get information about the deployed service
				var service = await _client.Swarm.InspectServiceAsync(serviceId);

				// This may throw an exception if we get an unknown state back
				// If no update status is available, we are creating a new service
				var serviceUpdateState = service.UpdateStatus?.GetState() ?? ServiceUpdateState.Creating;

				// Get tasks running as part of the service
				var tasks = (await _client.Tasks.ListAsync(service.Spec.Name, cancellationToken))
					// We want tasks that are expected to go to the 'Running' state, this exludes previous tasks
					.Where(t => t.DesiredState == TaskState.Running)
					.OrderByDescending(t => t.CreatedAt)
					.ToList()
				;

				// Print current service update status
				PrintServiceTasks(service, tasks);

				// See if the service update has completed
				switch (serviceUpdateState)
				{
					case ServiceUpdateState.Creating:

						await WaitForTasksToConvergeAsync(service, cancellationToken);

						var t = (await _client.Tasks.ListAsync(service.Spec.Name));
						if (!t.All(tt => tt.Status.State == TaskState.Running))
						{
							throw new DockerDeployException($"Some tasks failed. This may mean that the deployment has stopped due to failing tasks.");
						}

						return;

					case ServiceUpdateState.Completed:
						Log.Information("Service update completed");
						return;

					case ServiceUpdateState.Paused:
						throw new DockerDeployException($"Service update paused: {service.UpdateStatus?.Message}");

					case ServiceUpdateState.RollbackStarted:
						Log.Warning($"Service rolling back: {service.UpdateStatus?.Message}");
						break;

					case ServiceUpdateState.RollbackPaused:
						throw new DockerDeployException($"Service rollback paused: {service.UpdateStatus?.Message}");

					case ServiceUpdateState.RollbackCompleted:
						await WaitForTasksToConvergeAsync(service, cancellationToken);
						throw new DockerDeployException($"Service rolled back: {service.UpdateStatus?.Message}");
				}

				await Task.Delay(loopDelay);
			}

			// We should not get here if the update completed successfully, only if it took too long
			throw new DockerDeployException($"Service update did not complete. This may mean that tasks are taking longer than expected, or the wait time is to low for the amount of tasks to update.");
		}

		public void PrintServiceTasks(SwarmService service, IEnumerable<TaskResponse> tasks)
		{
			var serviceUpdateState = service.UpdateStatus?.GetState() ?? ServiceUpdateState.Creating;

			// Print current service update status
			Log.Information($"Service deploy in progress: '{serviceUpdateState}' (message: {service.UpdateStatus?.Message})");

			// Print tasks statuses
			if (!tasks.Any())
			{
				Log.Information($" └ (no tasks yet)");
			}

			foreach (var tt in tasks)
			{
				// TODO: Convert NodeID to Node Hostname, after this bug gets fixed:
				// https://github.com/dotnet/Docker.DotNet/issues/458
				if (string.IsNullOrWhiteSpace(tt.Status.Err))
				{
					Log.Information($" └ [node:{tt.NodeID}] [task:{tt.ID}] {tt.Status.State} -> {tt.DesiredState} (message: {tt.Status.Message})");
				}
				else
				{
					Log.Warning($" └ [node:{tt.NodeID}] [task:{tt.ID}] {tt.Status.State} -> {tt.DesiredState} (message: {tt.Status.Message}) (error: {tt.Status.Err})");
				}
			}
		}

		public async Task WaitForTasksToConvergeAsync(SwarmService service, CancellationToken cancellationToken)
		{
			var wait = TimeSpan.FromSeconds(5);

			var st = new TasksListState(cancelAfterTasksListStaleFor: wait);

			while (!cancellationToken.IsCancellationRequested)
			{
				var tasks = await _client.Tasks.ListAsync(service.Spec.Name);

				st.SetTasksListState(tasks);

				// See if there are tasks that are expected to change state soon
				var nonConverged = tasks
					.Where(t => t.Status.State != t.DesiredState) // State is not (yet) the desired state
					.Where(t => t.Status.State != TaskState.Failed) // "Failed" won't ever reach any other state
				;

				if (nonConverged.Any())
				{
					st.Reset();
				}

				if (st.IsStale)
				{
					Log.Information($"Tasks stable");
					return;
				}

				Log.Information($"Waiting for service to stabilize...");

				PrintServiceTasks(service, tasks);

				await Task.Delay(TimeSpan.FromSeconds(1));
			}
		}
	}
}