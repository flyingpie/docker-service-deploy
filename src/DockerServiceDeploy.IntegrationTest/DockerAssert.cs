using Docker.DotNet.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

using static DockerServiceDeploy.IntegrationTest.Constants;

namespace DockerServiceDeploy.IntegrationTest
{
	public static class DockerAssert
	{
		public static Task AssertServiceTasksAsync(string serviceName, uint taskCount, TaskState expectedState)
					=> AssertServiceTasksAsync(serviceName, taskCount, taskCount, expectedState);

		public static async Task AssertServiceTasksAsync(string serviceName, uint allCount, uint withStateCount, TaskState expectedState)
		{
			var service = (await DockerClient.Swarm.ListServicesAsync())
				.FirstOrDefault(s => s.Spec.Name == $"{UT}_{serviceName}")
			;

			Assert.IsNotNull(service);

			var tasks = (await DockerClient.Tasks.ListAsync())
				.Where(t => t.ServiceID == service.ID)
				.ToList()
			;

			var tasksDesc = string.Join(", ", tasks.GroupBy(t => t.Status.State).Select(t => $"{t.Key} ({t.Count()})"));

			Assert.AreEqual(
				expected: allCount,
				actual: (uint)tasks.Count,
				message: $"Expected {allCount}, but found {tasks.Count}. These tasks were found: {tasksDesc}"
			);

			Assert.AreEqual(
				expected: withStateCount,
				actual: (uint)tasks.Count(t => t.Status.State == expectedState),
				message: $"No {withStateCount} tasks were found with state '{expectedState}'. These tasks were found: {tasksDesc}"
			);
		}
	}
}