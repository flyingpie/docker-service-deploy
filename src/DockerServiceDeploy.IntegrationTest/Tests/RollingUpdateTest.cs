using Docker.DotNet.Models;
using Flyingpie.DockerServiceDeploy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

using static Flyingpie.DockerServiceDeploy.IntegrationTest.Constants;
using static Flyingpie.DockerServiceDeploy.IntegrationTest.DockerAssert;
using static Flyingpie.DockerServiceDeploy.IntegrationTest.TestServiceModel;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest.Tests
{
	[TestClass]
	public class RollingUpdateTest
	{
		[TestMethod]
		public async Task Ingress_SingleInstance_Success()
		{
			// Arrange
			var model = CreateModel();

			var port = TestData.FreeTcpPort();

			model.Ports.Add(new PortModel()
			{
				Target = "80",
				Published = $"{port}",
				Mode = PortMode.Ingress
			});

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			using var http = new HttpTester(new Uri($"http://win211:{port}/utils/long-response-time?duration=00:00:03"));

			await http.WaitForHitAsync();

			// Act
			model.Environments[UT][TestData.Id()] = TestData.Id(); // Forces update

			await DeployAsync(model);

			// Assert
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Shutdown);
			await AssertServiceTasksAsync(model.Name, allCount: 2, withStateCount: 1, TaskState.Running);

			http.AssertSuccessful();
		}

		[TestMethod]
		[DataRow(4)]
		public async Task Ingress_SingleInstance_MultipleUpdates(int updateCount)
		{
			// Arrange
			var model = CreateModel();

			var port = TestData.FreeTcpPort();

			model.Ports.Add(new PortModel()
			{
				Target = "80",
				Published = $"{port}",
				Mode = PortMode.Ingress
			});

			await DeployAsync(model);
			await AssertServiceTasksAsync(model.Name, 1, TaskState.Running);

			using var http = new HttpTester(new Uri($"http://win211:{port}/utils/long-response-time?duration=00:00:03"));

			await http.WaitForHitAsync();

			// Act
			for (uint i = 0; i < updateCount; i++)
			{
				model.Environments[UT][TestData.Id()] = TestData.Id(); // Forces update

				await DeployAsync(model);

				// Assert
				await AssertServiceTasksAsync(model.Name, allCount: 2 + i, withStateCount: 1 + i, TaskState.Shutdown);
				await AssertServiceTasksAsync(model.Name, allCount: 2 + i, withStateCount: 1, TaskState.Running);

				http.AssertSuccessful();
			}
		}
	}
}