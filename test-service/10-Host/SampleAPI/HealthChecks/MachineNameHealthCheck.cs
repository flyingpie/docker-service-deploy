using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.HealthChecks
{
	public class MachineNameHealthCheck : IHealthCheck
	{
		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
			=> Task.FromResult(HealthCheckResult.Healthy(data: new Dictionary<string, object>()
			{
				{ "MachineName", Environment.MachineName }
			}));
	}
}