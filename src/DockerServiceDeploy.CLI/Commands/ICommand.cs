using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.CLI.Commands
{
	public interface ICommand
	{
		void Execute();
	}

	public interface IAsyncCommand
	{
		Task ExecuteAsync();
	}
}