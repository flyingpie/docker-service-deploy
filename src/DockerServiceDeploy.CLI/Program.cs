using Flyingpie.DockerServiceDeploy.CLI.CommandLineParsing;

namespace Flyingpie.DockerServiceDeploy
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			args = args.IsVerboseSwitchEnabled(out var isVerboseEnabled);

			Logging.Setup(isVerboseEnabled);

			return CommandExecutor.Execute(args);
		}
	}
}