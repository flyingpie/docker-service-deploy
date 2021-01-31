using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Endpoints.Utils
{
	[Route("utils")]
	public class KillAppEndpoint : BaseAsyncEndpoint
	{
		[HttpPost("kill-app")]
		public Task HandleAsync()
		{
			Process.GetCurrentProcess().Kill();

			return Task.CompletedTask;
		}
	}
}