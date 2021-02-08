using System.Threading.Tasks;

namespace DockerServiceDeploy.CLI.Commands
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