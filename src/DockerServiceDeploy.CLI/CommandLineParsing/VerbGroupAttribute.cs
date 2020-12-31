using System;

namespace Flyingpie.DockerServiceDeploy.CLI.CommandLineParsing
{
	public class VerbGroupAttribute : Attribute
	{
		public VerbGroupAttribute(string group)
		{
			Name = group;
		}

		public string Name { get; }
	}
}