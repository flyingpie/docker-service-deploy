using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI.Endpoints.Utils
{
	[Route("utils")]
	public class ReadFileEndpoint : BaseAsyncEndpoint
	{
		[HttpGet("read-file")]
		public async Task<string> ReadFileAsync(string path)
		{
			var contents = await System.IO.File.ReadAllTextAsync(path);

			return contents;
		}
	}
}