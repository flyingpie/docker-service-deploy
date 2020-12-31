using System;
using System.Net;
using System.Net.Sockets;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest
{
	public static class TestData
	{
		// Success
		// Fails on start
		// Healthcheck fails
		// Slow start
		// Service Create
		// Service Update
		// RestartPolicy
		// UpdateConfig (rollback)
		// Service Create + Rollback

		public static string Id() => Guid.NewGuid().ToString();

		public static int FreeTcpPort()
		{
			var l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();

			int port = ((IPEndPoint)l.LocalEndpoint).Port;

			l.Stop();

			return port;
		}
	}
}