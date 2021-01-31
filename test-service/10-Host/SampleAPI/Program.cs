using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.SampleAPI
{
	public static class Program
	{
		public static readonly IConfiguration Configuration = new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.Build()
		;

		public static void Main(string[] args)
		{
			if (Flags.IsSlowStartup())
			{
				var c = 30;
				for (int i = 0; i < c; i++)
				{
					Console.WriteLine($"Emulating slow startup ({i}/{c})");

					Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
				}
			}

			if (Flags.IsStartFails())
			{
				throw new Exception("Startup fails flag was specified.");
			}

			using var host = BuildWebHost(args).Build();

			host.Run();
		}

		public static IHostBuilder BuildWebHost(string[] args)
			=> Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(config =>
				{
					config.AddConfiguration(Configuration);
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseStartup<Startup>()
					;
				})
			;
	}
}