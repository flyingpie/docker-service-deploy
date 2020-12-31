using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Flyingpie.DockerServiceDeploy.IntegrationTest.Utils
{
	public static class AssertException
	{
		public static void Thrown<TException>(Func<Task> action, Action<TException> callback, bool assertInnerException = true) where TException : Exception
		{
			var isExceptionThrown = false;

			try
			{
				action().GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				if (assertInnerException)
				{
					ex = ex.GetInnerMostException();
				}

				var expectedEx = ex as TException;
				if (expectedEx == null)
				{
					throw new AssertFailedException($"Expected exception of type '{typeof(TException).Name}', but was '{ex.GetType().Name}'.");
				}

				isExceptionThrown = true;

				callback(expectedEx);
			}

			if (!isExceptionThrown)
			{
				throw new AssertFailedException("Expected exception to be thrown, but it did not");
			}
		}

		public static void Thrown<TException>(Action action, Action<TException> predicate, bool assertInnerException = true) where TException : Exception
		{
			Thrown(async () => action(), predicate, assertInnerException);
		}

		public static void Thrown(Func<Task> action, Action<Exception> predicate, bool assertInnerException = true)
		{
			Thrown<Exception>(action, predicate, assertInnerException);
		}

		public static void Thrown(Action action, Action<Exception> predicate, bool assertInnerException = true)
		{
			Thrown(async () => action(), predicate, assertInnerException);
		}
	}
}