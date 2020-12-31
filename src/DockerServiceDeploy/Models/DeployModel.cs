namespace Flyingpie.DockerServiceDeploy.Models
{
	public class DeployModel
	{
		public DeployMode Mode { get; set; }
			= DeployMode.Global;

		public uint Replicas { get; set; }
			= 1;

		public PlacementModel Placement { get; set; }
			= new PlacementModel();

		public UpdateConfigModel UpdateConfig { get; set; }
			= new UpdateConfigModel();
	}
}