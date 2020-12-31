using Docker.DotNet.Models;
using Flyingpie.DockerServiceDeploy.Models;
using System;
using System.Linq;

namespace Flyingpie.DockerServiceDeploy.Extensions
{
	public static class ModelExtensions
	{
		public static ServiceSpec ToServiceSpec(this ServiceModel model, string envName)
		{
			return new ServiceSpec()
			{
				Name = $"{envName}_{model.Name}",
				Mode = new ServiceMode()
				{
					Global = model.Deploy.Mode == DeployMode.Global
						? new GlobalService()
						: null,

					Replicated = model.Deploy.Mode == DeployMode.Replicated
						? new ReplicatedService()
						{
							Replicas = model.Deploy.Replicas
						}
						: null
				},
				EndpointSpec = new EndpointSpec()
				{
					Mode = model.EndpointMode.GetEnumMemberValue(),
					Ports = model.Ports
						.Select(port => new PortConfig()
						{
							Protocol = port.Protocol.GetEnumMemberValue(),
							PublishMode = port.Mode.GetEnumMemberValue(),
							PublishedPort = port.Published.AsUInt(),
							TargetPort = port.Target.AsUInt()
						})
						.ToList()
				},
				TaskTemplate = new TaskSpec()
				{
					ContainerSpec = new ContainerSpec()
					{
						Image = model.Image,
						Env = model
							.Environments[envName]
							.Select(e => $"{e.Key}={e.Value}")
							.ToList(),

						Healthcheck = new HealthConfig()
						{
							Retries = model.Healthcheck.Retries
						}

						//Mounts = new[]
						//{
						//	new Mount()
						//	{
						//		Source = "",
						//		Target = "",
						//		Type = ""
						//	}
						//}
					},
					Placement = new Placement()
					{
						Constraints = model.Deploy.Placement.Constraints
					},
					RestartPolicy = new SwarmRestartPolicy()
					{
						Condition = model.RestartPolicy.Condition.GetEnumMemberValue(),
						Delay = (long)model.RestartPolicy.Delay.TotalNanoseconds(),
						MaxAttempts = model.RestartPolicy.MaxAttempts
					}
				},

				//Networks = new[]
				//{
				//	new NetworkAttachmentConfig()
				//	{
				//		Target = "networkname",
				//		Aliases = new []
				//		{
				//			"alias1"
				//		}
				//	}
				//},

				//RollbackConfig = new SwarmUpdateConfig()
				//{
				//}

				UpdateConfig = new SwarmUpdateConfig()
				{
					//Delay = 0,

					// Defaults
					//FailureAction = "pause", // pause|continue|rollback
					//MaxFailureRatio = 0,
					//Monitor = 5_000_000_000,
					//Parallelism = 1,
					//Order = "stop-first" // stop-first|start-first

					FailureAction = model.Deploy.UpdateConfig.FailureAction.GetEnumMemberValue(),
					Order = model.Deploy.UpdateConfig.Order.GetEnumMemberValue(),
					Parallelism = model.Deploy.UpdateConfig.Parallelism

					// If we can run multiple instances on the same node (eg. behind ingress port or random port on startup), use order = "start-first" and parallelism > 1 (preferred).
					// Otherwise, use order = "stop-first", parallelism = 1 and rollback.
				}
			};
		}
	}
}