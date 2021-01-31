using Flyingpie.DockerServiceDeploy.SampleAPI.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Configuration
{
	/// <summary>
	/// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1
	/// </summary>
	public static class HealthCheckConfiguration
	{
		public static IServiceCollection ConfigHealthChecks(this IServiceCollection services)
		{
			services
				.AddSingleton<MockHealthCheckState>()

				.AddHealthChecks()

				.AddCheck<MachineNameHealthCheck>(nameof(MachineNameHealthCheck))
				.AddCheck<MockHealthCheck>(nameof(MockHealthCheck))
			;

			return services;
		}
	}
}