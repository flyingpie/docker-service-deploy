using Flyingpie.DockerServiceDeploy.Docker;
using Flyingpie.DockerServiceDeploy.Extensions;
using Flyingpie.DockerServiceDeploy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Flyingpie.DockerServiceDeploy.IntegrationTest.Constants;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest
{
	public static class TestServiceModel
	{
		public static ServiceModel CreateModel()
		{
			var model = new ServiceModel()
			{
				Name = $"{NAME}-{TestData.Id()}",
				Image = IMAGE,

				Environments =
				{
					{
						"UT", new ServiceEnvironment()
					}
				},

				Healthcheck = new HealthcheckModel()
				{
					Retries = 1
				},

				Deploy = new DeployModel()
				{
					Mode = DeployMode.Replicated,
					Placement = new PlacementModel()
					{
						Constraints = new List<string>()
						{
							"node.hostname == win211" // TODO: Use local hostname
						}
					}
				},

				RestartPolicy = new RestartPolicyModel()
				{
					Condition = RestartPolicyCondition.None,
					MaxAttempts = 0
				}
			};

			return model;
		}

		public static async Task DeployAsync(ServiceModel model)
		{
			var deployClient = new DeployClient(DockerClientFactory.CreateClient(), DockerClientFactory.CreateAuthConfig());

			var serviceSpec = model.ToServiceSpec("UT");

			await deployClient.DeployAsync(serviceSpec);
		}
	}
}