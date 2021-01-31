using Ardalis.ApiEndpoints;
using Flyingpie.DockerServiceDeploy.SampleAPI.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Endpoints.Utils
{
	[Route("utils")]
	public class SetHealthStateEndpoint : BaseAsyncEndpoint
	{
		[HttpPost("health-state")]
		public IActionResult Handle(
			[FromServices] MockHealthCheckState healthCheckState,
			string status,
			string? description = null)
		{
			if (healthCheckState == null) throw new ArgumentNullException(nameof(healthCheckState));

			var healthStatus = Enum.Parse<HealthStatus>(status, true);

			healthCheckState.Result = new HealthCheckResult(healthStatus, description);

			return Ok();
		}
	}
}