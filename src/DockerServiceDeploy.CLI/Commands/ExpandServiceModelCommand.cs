using CommandLine;
using Flyingpie.DockerServiceDeploy.CLI.CommandLineParsing;
using Flyingpie.DockerServiceDeploy.Models;
using Newtonsoft.Json.Linq;
using System;

namespace Flyingpie.DockerServiceDeploy.CLI.Commands
{
	[Verb("expand-service-model", HelpText = "Reads a service.json file, expands any environment variables using the specified environment name, and prints the resulting model.")]
	[VerbGroup("Services")]
	public class ExpandServiceModelCommand : ICommand
	{
		[Option('f', "file", Required = true, HelpText = "Path to service.json file to deploy.")]
		public string ServiceJsonFile { get; set; }

		[Option('e', "env", Required = true, HelpText = "For what environment to deploy, eg. 'DEV', 'TST', etc.")]
		public string Environment { get; set; }

		[Option('p', "jpath", HelpText = "Return only the segment as specified by this JPath expression.")]
		public string? JPath { get; set; }

		public void Execute()
		{
			EnvVars.Set(Environment);

			var model = ServiceModel.ParseFile(ServiceJsonFile);

			model = model.ExpandVariables(Environment);

			if (!string.IsNullOrWhiteSpace(JPath))
			{
				Console.WriteLine(model.ReInterpret<JObject>().SelectToken(JPath));
			}
			else
			{
				Console.WriteLine(model.Serialize());
			}
		}
	}
}