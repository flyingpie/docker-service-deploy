using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Flyingpie.DockerServiceDeploy.Docker
{
	public class DockerConfig
	{
		public Dictionary<string, DockerConfigAuthConfig>? Auths { get; set; }

		public static bool TryLoad([NotNullWhen(true)] out DockerConfig? config)
		{
			config = null;

			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker", "config.json");

			try
			{
				Log.Information($"Attempting to load docker config file from '{path}'.");

				// Just throw an exception if the file does not exist, so it's logged
				config = Json.DeserializeFile<DockerConfig>(path);

				return true;
			}
			catch (Exception ex)
			{
				Log.Warning(ex, $"Could not load Docker config from '{path}': {ex.Message}.");
				return false;
			}
		}
	}

	public class DockerConfigAuthConfig
	{
		public string? Auth { get; set; }

		public bool TryGetCredentials([NotNullWhen(true)] out string? username, [NotNullWhen(true)] out string? password)
		{
			username = null;
			password = null;

			if (string.IsNullOrWhiteSpace(Auth)) return false;

			try
			{
				// Base64 -> UTF8
				var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(Auth));

				// Split username:password
				var split = decoded.Split(":", StringSplitOptions.RemoveEmptyEntries);
				if (split.Length != 2) return false;

				username = split[0];
				password = split[1];

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}