using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace DockerServiceDeploy
{
	public static class Json
	{
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

		public static T DeserializeFile<T>(string path)
		{
			return Deserialize<T>(File.ReadAllText(path));
		}

		public static T Deserialize<T>(string json)
		{
			// TODO: Validation
			return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings)!;
		}

		public static string Serialize(object source)
		{
			return JsonConvert.SerializeObject(source, JsonSerializerSettings);
		}

		public static void SerializeFile(object source, string path)
		{
			File.WriteAllText(path, Serialize(source));
		}

		public static TTo ReInterpret<TTo>(this object source)
		{
			return Json.Deserialize<TTo>(Serialize(source));
		}
	}
}