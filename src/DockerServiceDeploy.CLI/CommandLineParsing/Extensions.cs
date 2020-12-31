using System;
using System.Collections.Generic;

namespace Flyingpie.DockerServiceDeploy.CLI.CommandLineParsing
{
	public static class Extensions
	{
		public static string[] IsVerboseSwitchEnabled(this string[] args, out bool isVerboseEnabled)
		{
			isVerboseEnabled = false;
			var result = new List<string>();

			foreach (var arg in args)
			{
				if (string.IsNullOrWhiteSpace(arg)) continue;

				if (arg.Trim().Equals("--verbose", StringComparison.OrdinalIgnoreCase))
				{
					isVerboseEnabled = true;
					continue;
				}

				result.Add(arg);
			}

			return result.ToArray();
		}
	}
}