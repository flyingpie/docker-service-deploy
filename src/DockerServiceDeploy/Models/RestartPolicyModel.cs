using System;

namespace Flyingpie.DockerServiceDeploy.Models
{
	public class RestartPolicyModel
	{
		/// <summary>
		/// Delay between restart attempts.
		/// </summary>
		public TimeSpan Delay { get; set; }
			= TimeSpan.Zero;

		/// <summary>
		/// Condition for restart.
		/// </summary>
		public RestartPolicyCondition Condition { get; set; }
			= RestartPolicyCondition.OnFailure;

		/// <summary>
		/// Maximum attempts to restart a given container before giving up (default value is 0, which is ignored).
		/// </summary>
		public uint MaxAttempts { get; set; }
			= 3;
	}
}