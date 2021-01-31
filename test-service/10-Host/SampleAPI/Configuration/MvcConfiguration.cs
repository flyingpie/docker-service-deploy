using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Configuration
{
	public static class MvcConfiguration
	{
		public static IServiceCollection ConfigMvc(this IServiceCollection services)
		{
			services.AddControllers();

			return services;
		}

		public static IEndpointRouteBuilder ConfigMvc(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapControllers();

			return endpoints;
		}
	}
}