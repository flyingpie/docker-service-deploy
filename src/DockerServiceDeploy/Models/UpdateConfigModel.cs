namespace DockerServiceDeploy.Models
{
	public class UpdateConfigModel
	{
		public FailureAction FailureAction { get; set; }
			= FailureAction.Rollback;

		public UpdateOrder Order { get; set; }
			= UpdateOrder.StartFirst;

		public uint Parallelism { get; set; }
			= 2;
	}
}