using Flyingpie.DockerServiceDeploy.CLI.CommandLineParsing;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			args = args.IsVerboseSwitchEnabled(out var isVerboseEnabled);

			Logging.Setup(isVerboseEnabled);

			CommandExecutor.Execute(args);
		}
	}
}