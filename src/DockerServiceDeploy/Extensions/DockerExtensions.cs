using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DockerServiceDeploy.Extensions
{
	public static class DockerExtensions
	{
		/// <summary>
		/// Converts a string-based task state to an enum-based one.
		///
		/// https://docs.docker.com/engine/api/v1.41/#operation/ServiceInspect
		/// </summary>
		public static ServiceUpdateState GetState(this UpdateStatus status)
		{
			if (status?.State == null) throw new ArgumentNullException(nameof(status));

			switch (status.State)
			{
				case "completed":
					return ServiceUpdateState.Completed;

				case "paused":
					return ServiceUpdateState.Paused;

				case "updating":
					return ServiceUpdateState.Updating;

				case "rollback_started":
					return ServiceUpdateState.RollbackStarted;

				case "rollback_paused":
					return ServiceUpdateState.RollbackPaused;

				case "rollback_completed":
					return ServiceUpdateState.RollbackCompleted;

				default:
					throw new Exception($"Unknown service update state '{status.State}'.");
			}
		}

		public static string GetTasksState(this IEnumerable<TaskResponse> tasks)
		{
			var sb = new StringBuilder();

			sb.AppendLine("tasks");

			foreach (var task in tasks)
			{
				sb.AppendLine($"{task.ID}|{task.Status.State}|{task.DesiredState}");
			}

			return sb.ToString().Hash();
		}

		public static Task<IList<TaskResponse>> ListAsync(this ITasksOperations tasks, string serviceName, CancellationToken cancellationToken = default)
		{
			// Get tasks running as part of the service
			// https://docs.docker.com/engine/api/v1.41/#operation/TaskList
			var tasksListParams = new TasksListParameters()
			{
				Filters = new Dictionary<string, IDictionary<string, bool>>()
				{
					{ "service", new Dictionary<string, bool>() { { serviceName, true } } }
				}
			};

			return tasks.ListAsync(tasksListParams, cancellationToken);
		}
	}
}

namespace Docker.DotNet.Models
{
	public enum ServiceUpdateState
	{
		Creating,
		Unknown,

		Completed,
		Paused,
		Updating,

		RollbackStarted,
		RollbackPaused,
		RollbackCompleted
	}
}