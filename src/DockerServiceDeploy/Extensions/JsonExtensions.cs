using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DockerServiceDeploy.Extensions
{
	public static class JsonExtensions
	{
		public static readonly Regex EnvVarRegex = new Regex("%([^%]*)%", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// Looks through the source JSON for variables and expands environment variables.
		/// </summary>
		public static T ExpandEnvironmentVariables<T>(this T node) where T : JContainer
		{
			node
				// Get all descendants (eg. children, grand children, etc.)
				.Descendants()
				// Don't touch comments
				.Where(t => t.Type != JTokenType.Comment)
				// ToList allows us to use 'ForEach' and prevents 'collection modified'-exceptions
				.ToList()
				// Expand all found values
				.ForEach(token =>
				{
					// Literal values (eg. strings, booleans, etc.
					if (token is JValue jVal)
					{
						var jValStr = jVal.ToString();
						var expanded = Environment.ExpandEnvironmentVariables(jValStr);

						var matches = EnvVarRegex.Matches(expanded);
						if (matches.Any()) throw new InvalidOperationException($"Found unresolvable environment variable: {expanded}");

						if (!expanded.Equals(jValStr))
						{
							token.Replace(expanded);
						}
					}
					// Properties, to allow the expansion of variables in property names
					else if (token is JProperty jProp)
					{
						var jPropName = jProp.Name;
						var expanded = Environment.ExpandEnvironmentVariables(jPropName);

						var matches = EnvVarRegex.Matches(expanded);
						if (matches.Any()) throw new InvalidOperationException($"Found unresolvable environment variable: {expanded}");

						if (!expanded.Equals(jPropName))
						{
							token.Replace(new JProperty(expanded, jProp.Value));
						}
					}
				})
			;

			return node;
		}
	}
}