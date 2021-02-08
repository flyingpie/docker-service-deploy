using Serilog;
using Serilog.Events;

namespace DockerServiceDeploy
{
	public static class Logging
	{
		public static void Setup(bool isVerbose)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Is(isVerbose ? LogEventLevel.Verbose : LogEventLevel.Information)

				.WriteTo.Console()

				.WriteTo.File("logs/log.txt")

				.CreateLogger();
		}
	}
}