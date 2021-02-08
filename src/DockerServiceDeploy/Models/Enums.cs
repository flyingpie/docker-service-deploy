using System.Runtime.Serialization;

namespace DockerServiceDeploy.Models
{
	public enum DeployMode
	{
		[EnumMember(Value = "global")]
		Global,

		[EnumMember(Value = "replicated")]
		Replicated
	}

	public enum EndpointMode
	{
		[EnumMember(Value = "dnsrr")]
		DnsRR,

		[EnumMember(Value = "vip")]
		Vip
	}

	public enum FailureAction
	{
		[EnumMember(Value = "pause")]
		Pause,

		[EnumMember(Value = "continue")]
		Continue,

		[EnumMember(Value = "rollback")]
		Rollback
	}

	public enum PortMode
	{
		[EnumMember(Value = "host")]
		Host,

		[EnumMember(Value = "ingress")]
		Ingress
	}

	public enum PortProtocol
	{
		[EnumMember(Value = "tcp")]
		Tcp,

		[EnumMember(Value = "udp")]
		Udp
	}

	public enum RestartPolicyCondition
	{
		[EnumMember(Value = "none")]
		None,

		[EnumMember(Value = "on-failure")]
		OnFailure,

		[EnumMember(Value = "any")]
		Any,
	}

	public enum UpdateOrder
	{
		[EnumMember(Value = "start-first")]
		StartFirst,

		[EnumMember(Value = "stop-first")]
		StopFirst
	}

	public enum VolumeType
	{
		[EnumMember(Value = "bind")]
		Bind,

		[EnumMember(Value = "volume")]
		Volume
	}
}