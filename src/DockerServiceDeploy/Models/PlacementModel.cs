using System.Collections.Generic;

namespace Flyingpie.DockerServiceDeploy.Models
{
	public class PlacementModel
	{
		public List<string> Constraints { get; set; }
			= new List<string>();
	}
}