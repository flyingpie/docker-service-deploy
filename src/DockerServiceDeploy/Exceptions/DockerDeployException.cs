using System;

namespace DockerServiceDeploy.Exceptions
{
	public class DockerDeployException : Exception
	{
		public DockerDeployException(string? message) : base(message)
		{
		}
	}
}