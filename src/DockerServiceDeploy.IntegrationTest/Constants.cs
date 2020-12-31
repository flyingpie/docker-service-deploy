using Docker.DotNet;
using Flyingpie.DockerServiceDeploy.Docker;
using System;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest
{
	public static class Constants
	{
		public const string NAME = "TODO";
		public const string IMAGE = "TODO";
		public const string UT = "UT";

		public const string START_FAILS = "START_FAILS";
		public const string START_SLOW = "START_SLOW";
		public const string START_UNHEALTHY = "START_UNHEALTHY";

		public const string TRUE = "true";

		private static DockerClient? _dockerClient;

		public static DockerClient DockerClient
		{
			get
			{
				// TODO: Remove
				Environment.SetEnvironmentVariable("DOCKER_HOST", "tcp://192.168.178.211:2375");

				_dockerClient ??= DockerClientFactory.CreateClient();

				return _dockerClient;
			}
		}
	}
}