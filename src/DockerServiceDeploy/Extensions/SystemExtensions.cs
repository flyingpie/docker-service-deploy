using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace System
{
	public static class SystemExtensions
	{
		public static uint AsUInt(this string source)
		{
			if (!uint.TryParse(source, out var asUint)) throw new Exception($"Could not parsed value '{source}' as uint.");

			return asUint;
		}

		public static string GetEnumMemberValue<T>(this T value)
			where T : struct
		{
			var enumMemberValue = value
				.GetType()
				?.GetField(value.ToString())
				?.GetCustomAttribute<EnumMemberAttribute>()
				?.Value
			;

			return !string.IsNullOrWhiteSpace(enumMemberValue) ? enumMemberValue : value.ToString();
		}

		public static string ExpandVariables(this string source)
		{
			if (string.IsNullOrWhiteSpace(source)) return source;

			return Environment.ExpandEnvironmentVariables(source);
		}

		public static string Hash(this string input)
		{
			var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
			return string.Concat(hash.Select(b => b.ToString("x2")));
		}

		public static double TotalMicroseconds(this TimeSpan timeSpan)
			=> timeSpan.TotalMilliseconds * 1000;

		public static double TotalNanoseconds(this TimeSpan timeSpan)
			=> timeSpan.TotalMicroseconds() * 1000;
	}
}