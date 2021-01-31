using System;

namespace Flyingpie.DockerServiceDeploy.SampleAPI
{
	public static class Flags
	{
		public static bool IsSlowStartup()
		{
			return Environment.GetEnvironmentVariable("START_SLOW")?.ToLowerInvariant() == "true";
		}

		public static bool IsStartFails()
		{
			return Environment.GetEnvironmentVariable("START_FAILS")?.ToLowerInvariant() == "true";
		}

		public static bool IsUnhealthyAtStart()
		{
			return Environment.GetEnvironmentVariable("START_UNHEALTHY")?.ToLowerInvariant() == "true";
		}
	}
}