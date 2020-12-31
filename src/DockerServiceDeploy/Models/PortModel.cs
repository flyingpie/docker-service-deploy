namespace Flyingpie.DockerServiceDeploy.Models
{
	public class PortModel
	{
		public string Target { get; set; }

		public string Published { get; set; }

		public PortProtocol Protocol { get; set; }
			= PortProtocol.Tcp;

		public PortMode Mode { get; set; }
			= PortMode.Ingress;
	}
}