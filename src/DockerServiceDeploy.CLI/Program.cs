using DockerServiceDeploy.CLI.CommandLineParsing;

namespace DockerServiceDeploy
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