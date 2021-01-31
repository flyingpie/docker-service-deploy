using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Endpoints.Utils
{
	[Route("utils")]
	public class GetLongResponseTimeEndpoint : BaseAsyncEndpoint
	{
		[HttpGet("long-response-time")]
		public async Task<ActionResult<ResponseVM>> HandleAsync(TimeSpan duration)
		{
			await Task.Delay(duration);

			return Ok(new ResponseVM()
			{
				MachineName = Environment.MachineName,
				Message = $"Waited for {duration}"
			});
		}

		public class ResponseVM
		{
			public string? MachineName { get; set; }

			public string? Message { get; set; }
		}
	}
}