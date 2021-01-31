using Flyingpie.DockerServiceDeploy.SampleAPI.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Flyingpie.DockerServiceDeploy.SampleAPI
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddMemoryCache()
				.AddOptions()
				.ConfigHealthChecks()
				.ConfigMvc()
				.ConfigHost()
			;
		}

		public void Configure(IApplicationBuilder app)
		{
			app
				.UseHsts()
				.UseRouting()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapHealthChecks("/health");
					endpoints.ConfigMvc();
				})
			;
		}
	}
}