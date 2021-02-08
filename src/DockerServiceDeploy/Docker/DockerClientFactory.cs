using Docker.DotNet;
using Docker.DotNet.Models;
using Serilog;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DockerServiceDeploy.Docker
{
	public static class DockerClientFactory
	{
		public static AuthConfig CreateAuthConfig()
		{
			var authConfig = new AuthConfig();

			if (!DockerConfig.TryLoad(out var dockerConfig))
			{
				return authConfig;
			}

			// TODO: Properly handle multiple registries
			if (dockerConfig.Auths != null && dockerConfig.Auths.Any() && dockerConfig.Auths.First().Value.TryGetCredentials(out var username, out var password))
			{
				Log.Information($"Loading auth configuration for registry '{dockerConfig.Auths.First().Key}'");

				authConfig.Username = username;
				authConfig.Password = password;
			}
			else
			{
				Log.Information($"No auth config found in docker config file.");
			}

			return authConfig;
		}

		public static DockerClient CreateClient()
		{
			var uriFromEnv = Environment.GetEnvironmentVariable("DOCKER_HOST");

			Uri uri;

			if (!string.IsNullOrWhiteSpace(uriFromEnv))
			{
				uri = new Uri(uriFromEnv);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				uri = new Uri("unix:///var/run/docker.sock");
			}
			else
			{
				uri = new Uri("npipe://./pipe/docker_engine");
			}

			Log.Information($"Using docker endpoint '{uri}'");

			return new DockerClientConfiguration(uri).CreateClient();
		}
	}
}