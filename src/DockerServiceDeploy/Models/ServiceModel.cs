using DockerServiceDeploy.Exceptions;
using DockerServiceDeploy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DockerServiceDeploy.Models
{
	public class ServiceModel
	{
		public string Name { get; set; }

		public string Image { get; set; }

		public Dictionary<string, ServiceEnvironment> Environments { get; set; }
			= new Dictionary<string, ServiceEnvironment>();

		public DeployModel Deploy { get; set; }
			= new DeployModel();

		public EndpointMode EndpointMode { get; set; }
			= EndpointMode.Vip;

		public HealthcheckModel Healthcheck { get; set; }
			= new HealthcheckModel();

		public List<PortModel> Ports { get; set; }
			= new List<PortModel>();

		public RestartPolicyModel RestartPolicy { get; set; }
			= new RestartPolicyModel();

		#region Environment Variables

		public ServiceModel ExpandVariables(string envName)
		{
			// Get requested environment
			var env = Environments.FirstOrDefault(k => k.Key.Equals(envName, StringComparison.OrdinalIgnoreCase));
			if (env.Value == null) throw new DockerDeployException($"No such environment '{envName}', these are available: {string.Join(", ", Environments.Keys)}.");

			Environment.SetEnvironmentVariable("NAME", Name);

			// Set env vars as actual environment variables, so we can use them in the JSON service model
			foreach (var envVar in env.Value)
			{
				Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
			}

			return this
				.ReInterpret<JObject>()
				.ExpandEnvironmentVariables()
				.ReInterpret<ServiceModel>()
			;
		}

		#endregion Environment Variables

		#region Loading & Saving

		private static JsonSerializerSettings? _jsonSerializerSettings;

		public static JsonSerializerSettings JsonSerializerSettings
		{
			get
			{
				if (_jsonSerializerSettings == null)
				{
					_jsonSerializerSettings = new JsonSerializerSettings()
					{
						ContractResolver = new DefaultContractResolver()
						{
							// PropertyName -> property-name
							NamingStrategy = new KebabCaseNamingStrategy()
						},
						Formatting = Formatting.Indented
					};

					_jsonSerializerSettings.Converters.Add(new StringEnumConverter());
				}

				return _jsonSerializerSettings;
			}
		}

		public static ServiceModel ParseFile(string path)
		{
			return Parse(File.ReadAllText(path));
		}

		public static ServiceModel Parse(string json)
		{
			// TODO: Validation
			return JsonConvert.DeserializeObject<ServiceModel>(json, JsonSerializerSettings)!;
		}

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, JsonSerializerSettings);
		}

		public void Write(string path)
		{
			File.WriteAllText(path, Serialize());
		}

		#endregion Loading & Saving
	}
}