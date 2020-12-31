using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest.Utils
{
	public static class Extensions
	{
		public static void AssertContainsMessageTerms(this Exception exception, params string[] terms)
		{
			if (!exception.ContainsExceptionWithMessageTerms(terms))
			{
				throw new AssertFailedException($"Expected exception with message terms '{string.Join(", ", terms)}', but was not found. Message:{Environment.NewLine}{exception.GetFullExceptionMessage()}.");
			}
		}

		public static bool ContainsExceptionWithMessageTerms(this Exception exception, params string[] terms)
		{
			if (terms.All(term => exception.Message.ToLowerInvariant().Contains(term.ToLowerInvariant())))
			{
				return true;
			}

			if (exception.InnerException != null)
			{
				return exception.InnerException.ContainsExceptionWithMessageTerms(terms);
			}

			return false;
		}

		public static Exception GetInnerMostException(this Exception exception)
		{
			while (exception?.InnerException != null) exception = exception.InnerException;

			return exception;
		}

		public static string GetFullExceptionMessage(this Exception exception)
		{
			var sb = new StringBuilder();

			while (exception != null)
			{
				sb.AppendLine($"--- {exception.GetType().FullName} ---");
				sb.AppendLine($"{exception.Message}");
				sb.AppendLine($"----------");

				exception = exception.InnerException;
			}

			return sb.ToString();
		}
	}
}