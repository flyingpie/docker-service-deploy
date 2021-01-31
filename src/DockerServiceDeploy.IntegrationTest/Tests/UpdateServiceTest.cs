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
	public class UpdateServiceTest
	{
		[TestMethod]
		public async Task UpdateService_Success()
		{
			// Arrange
			var model = CreateModel();

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Environments[UT][TestData.Id()] = TestData.Id(); // Forces update

			await DeployAsync(model);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Shutdown);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Running);
		}

		[TestMethod]
		[DataRow(1U)]
		[DataRow(2U)]
		[DataRow(3U)]
		public async Task UpdateService_Success_Replicas(uint replicas)
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.Replicas = replicas;

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, replicas, TaskState.Running);

			// Act
			model.Environments[UT][TestData.Id()] = TestData.Id(); // Forces update

			await DeployAsync(model);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: replicas * 2, withStateCount: replicas, TaskState.Shutdown);
			await AssertServiceTasksAsync(model.Name, allCount: replicas * 2, withStateCount: replicas, TaskState.Running);
		}

		#region UpdateService - StartFails

		[TestMethod]
		public async Task UpdateService_StartFails_StartFirst_Pause()
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.UpdateConfig.FailureAction = FailureAction.Pause;

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Environments[UT][START_FAILS] = TRUE;

			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Service update paused: update paused due to failure or early termination of task")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Running);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Failed);
		}

		[TestMethod]
		public async Task UpdateService_StartFails_StartFirst_Rollback()
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.UpdateConfig.FailureAction = FailureAction.Rollback;

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Environments[UT][START_FAILS] = TRUE;

			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Service rolled back: rollback completed")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Running);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Failed);
		}

		[TestMethod]
		public async Task UpdateService_StartFails_StopFirst_Pause()
		{
			// Arrange
			var model = CreateModel();

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Deploy.UpdateConfig.FailureAction = FailureAction.Pause;
			model.Deploy.UpdateConfig.Order = UpdateOrder.StopFirst;

			model.Environments[UT][START_FAILS] = TRUE;

			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Service update paused: update paused due to failure or early termination of task")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Shutdown);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Failed);
		}

		[TestMethod]
		public async Task UpdateService_StartFails_StopFirst_Rollback()
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.UpdateConfig.FailureAction = FailureAction.Rollback;
			model.Deploy.UpdateConfig.Order = UpdateOrder.StopFirst;

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Environments[UT][START_FAILS] = TRUE;

			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Service rolled back: rollback completed")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 3, withStateCount: 1, TaskState.Shutdown);
			await AssertServiceTasksAsync(model.Name, allCount: 3, withStateCount: 1, TaskState.Failed);
			await AssertServiceTasksAsync(model.Name, allCount: 3, withStateCount: 1, TaskState.Running);
		}

		#endregion UpdateService - StartFails

		[TestMethod]
		public async Task UpdateService_StartUnhealthy_StartFirst_Pause()
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.UpdateConfig.FailureAction = FailureAction.Pause;

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Environments[UT][START_UNHEALTHY] = TRUE;

			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Service update paused: update paused due to failure or early termination of task")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Running);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Failed);
		}

		[TestMethod]
		public async Task UpdateService_StartUnhealthy_StartFirst_Rollback()
		{
			// Arrange
			var model = CreateModel();

			model.Deploy.UpdateConfig.FailureAction = FailureAction.Rollback;

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			// Act
			model.Environments[UT][START_UNHEALTHY] = TRUE;

			AssertException.Thrown<DockerDeployException>
			(
				() => DeployAsync(model),
				ex => ex.AssertContainsMessageTerms("Service rolled back: rollback completed")
			);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Running);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Failed);
		}
	}
}