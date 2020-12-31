using System;

namespace Flyingpie.DockerServiceDeploy.Exceptions
{
	public class DockerDeployException : Exception
	{
		public DockerDeployException(string? message) : base(message)
		{
		}
	}
}