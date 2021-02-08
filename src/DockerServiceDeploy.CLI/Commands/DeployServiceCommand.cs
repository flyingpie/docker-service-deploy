using CommandLine;
using DockerServiceDeploy.CLI.CommandLineParsing;
using DockerServiceDeploy.Docker;
using DockerServiceDeploy.Exceptions;
using DockerServiceDeploy.Extensions;
using DockerServiceDeploy.Models;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DockerServiceDeploy.CLI.Commands
{
	[Verb("deploy-service", HelpText = "Deploys a docker service using a 'service.json' service specification file.")]
	[VerbGroup("Services")]
	public class DeployServiceCommand : BaseDockerCommand, IAsyncCommand
	{
		[Option('f', "file", Required = true, HelpText = "Path to service.json file to deploy.")]
		public string ServiceJsonFile { get; set; }

		[Option('e', "env", Required = true, HelpText = "For what environment to deploy, eg. 'DEV', 'TST', etc.")]
		public string Environment { get; set; }

		public async Task ExecuteAsync()
		{
			EnvVars.Set(Environment);

			var sw = new Stopwatch();
			sw.Start();

			// Expand environment variables, and convert service.json to Docker API ServiceSpec
			var serviceSpec = ServiceModel
				.ParseFile(ServiceJsonFile)
				.ExpandVariables(Environment)
				.ToServiceSpec(Environment)
			;

			try
			{
				await DeployClient.DeployAsync(serviceSpec);
			}
			catch (DockerDeployException ex)
			{
				// Don't include the original exception, since this type of exception is meant for specific error, where the stack trace is not important
				Log.Error($"Deploy failed: {ex.Message}");
				throw;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Deploy failed: {ex.Message}");
				throw;
			}

			sw.Stop();

			Log.Information($"Deploy completed, took {sw.Elapsed}.");
		}
	}
}