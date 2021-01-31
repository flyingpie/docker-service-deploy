using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Endpoints.Utils
{
	[Route("utils")]
	public class StopAppEndpoint : BaseAsyncEndpoint
	{
		[HttpPost("stop-app")]
		public Task HandleAsync()
		{
			Task.Run(() =>
			{
				Console.WriteLine($"Exiting app");
				Environment.Exit(0);
			});

			return Task.CompletedTask;
		}
	}
}