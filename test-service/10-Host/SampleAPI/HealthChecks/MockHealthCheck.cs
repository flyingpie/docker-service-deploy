using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.HealthChecks
{
	public class MockHealthCheck : IHealthCheck
	{
		private readonly MockHealthCheckState _healthCheckState;

		public MockHealthCheck(MockHealthCheckState healthCheckState)
		{
			_healthCheckState = healthCheckState ?? throw new ArgumentNullException(nameof(healthCheckState));
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			return Task.FromResult(_healthCheckState.Result);
		}
	}

	public class MockHealthCheckState
	{
		public HealthCheckResult Result { get; set; } = new HealthCheckResult(Flags.IsUnhealthyAtStart() ? HealthStatus.Unhealthy : HealthStatus.Healthy, description: "Default description");
	}
}