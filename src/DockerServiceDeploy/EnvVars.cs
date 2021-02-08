using System;
using System.Runtime.InteropServices;

namespace DockerServiceDeploy
{
	public static class EnvVars
	{
		public static void Set(string env)
		{
			// Env
			Environment.SetEnvironmentVariable("ENV", env);

			// OS
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Environment.SetEnvironmentVariable("OS", "linux");
			}
			else
			{
				Environment.SetEnvironmentVariable("OS", "windows");
			}
		}
	}
}