using CommandLine;
using DockerServiceDeploy.CLI.CommandLineParsing;
using DockerServiceDeploy.Docker;
using Serilog;

namespace DockerServiceDeploy.CLI.Commands
{
	[Verb("test-auth-config", HelpText = "Attempts to load auth from the local .docker/config.json file to see if it's accessible.")]
	[VerbGroup("Utils")]
	public class TestAuthConfigCommand : BaseDockerCommand, ICommand
	{
		public void Execute()
		{
			var authConfig = DockerClientFactory.CreateAuthConfig();

			Log.Information($"Loaded auth config with username '{authConfig.Username}' and password '{authConfig.Password}'.");
		}
	}
}