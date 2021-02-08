using System.Collections.Generic;

namespace DockerServiceDeploy.Models
{
	public class PlacementModel
	{
		public List<string> Constraints { get; set; }
			= new List<string>();
	}
}