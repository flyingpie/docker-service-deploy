using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Configuration
{
	public static class HostConfiguration
	{
		public static IServiceCollection ConfigHost(this IServiceCollection services)
		{
			services
				.Configure<HostOptions>(opts =>
				{
					// How long to wait for pending requests to finish
					// After this, any remaining requests will be cancelled
					opts.ShutdownTimeout = TimeSpan.FromMinutes(1);
				})
			;

			return services;
		}
	}
}