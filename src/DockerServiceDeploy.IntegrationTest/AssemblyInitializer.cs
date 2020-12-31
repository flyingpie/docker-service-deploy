using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using static Flyingpie.DockerServiceDeploy.IntegrationTest.Constants;

// Run tests in parallel
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace Flyingpie.DockerServiceDeploy.IntegrationTest
{
	[TestClass]
	public static class AssemblyInitializer
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			Logging.Setup(isVerbose: true);

			CleanupDockerServicesAsync().GetAwaiter().GetResult();
		}

		public static async Task CleanupDockerServicesAsync()
		{
			foreach (var s in await DockerClient.Swarm.ListServicesAsync())
			{
				await DockerClient.Swarm.RemoveServiceAsync(s.ID);
			}
		}
	}
}