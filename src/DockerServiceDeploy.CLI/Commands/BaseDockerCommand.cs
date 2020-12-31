using Docker.DotNet;
using Docker.DotNet.Models;
using Flyingpie.DockerServiceDeploy.Docker;

namespace Flyingpie.DockerServiceDeploy.CLI.Commands
{
	public abstract class BaseDockerCommand
	{
		private AuthConfig? _authConfig;

		public AuthConfig AuthConfig => _authConfig ??= DockerClientFactory.CreateAuthConfig();

		private DockerClient? _dockerClient;

		public DockerClient DockerClient => _dockerClient ??= DockerClientFactory.CreateClient();

		private DeployClient? _deployClient;

		public DeployClient DeployClient
		{
			get
			{
				if (_deployClient == null)
				{
					_deployClient = new DeployClient(DockerClient, AuthConfig);
				}

				return _deployClient;
			}
		}
	}
}