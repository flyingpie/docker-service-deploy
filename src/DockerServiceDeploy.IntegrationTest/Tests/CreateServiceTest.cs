using Docker.DotNet.Models;
using Flyingpie.DockerServiceDeploy.Exceptions;
using Flyingpie.DockerServiceDeploy.IntegrationTest.Utils;
using Flyingpie.DockerServiceDeploy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using static Flyingpie.DockerServiceDeploy.IntegrationTest.Constants;
using static Flyingpie.DockerServiceDeploy.IntegrationTest.DockerAssert;
using static Flyingpie.DockerServiceDeploy.IntegrationTest.TestServiceModel;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest.Tests
{
	[TestClass]
	public class CreateServiceTest
	{
		[TestMethod]
		public async Task CreateService_Success()
		{
			// Arrange
			var model = CreateModel();

			// Act
			await DeployAsync(model);

			// Assert
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);
		}

		[TestMethod]
		[DataRow(2U)]
		[DataRow(3U)]
		public async Task CreateService_Success_Replicas(uint replicas)
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.Replicas = replicas;

			// Act
			await DeployAsync(model);

			// Assert
			await AssertServiceTasksAsync(model.Name, replicas, TaskState.Running);
		}

		[TestMethod]
		public async Task CreateService_StartFails()
		{
			// Arrange
			var model = CreateModel();

			model.Environments[UT][START_FAILS] = TRUE;

			// Act
			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Some tasks failed. This may mean that the deployment has stopped due to failing tasks.")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Failed);
		}

		[TestMethod]
		[DataRow(1U)]
		[DataRow(2U)]
		[DataRow(3U)]
		public async Task CreateService_StartFails_Restarts(uint maxAttempts)
		{
			// Arrange
			var model = CreateModel();

			model.RestartPolicy.Condition = RestartPolicyCondition.Any;
			model.RestartPolicy.MaxAttempts = maxAttempts;

			model.Environments[UT][START_FAILS] = TRUE;

			// Act
			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Some tasks failed. This may mean that the deployment has stopped due to failing tasks.")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, maxAttempts + 1, TaskState.Failed); // Add initial attempt, eg. 1 retry attempt = 2 tasks
		}

		[TestMethod]
		public async Task CreateService_StartUnhealthy()
		{
			// Arrange
			var model = CreateModel();

			model.Environments[UT][START_UNHEALTHY] = TRUE;

			// Act
			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Some tasks failed. This may mean that the deployment has stopped due to failing tasks.")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Failed);
		}

		[TestMethod]
		public async Task CreateService_StartSlow()
		{
			// Arrange
			var model = CreateModel();

			model.Environments[UT][START_SLOW] = TRUE;

			// Act
			await DeployAsync(model);

			// Assert
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);
		}

		// TODO: Test labels that don't match any node (and service ends up with 0 instances)
		// TODO: Node names
	}
}