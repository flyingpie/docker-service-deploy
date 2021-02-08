using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DockerServiceDeploy.IntegrationTest
{
	public sealed class HttpTester : IDisposable
	{
		// TODO: Use multiple clients that overlap requests, so a request is always pending
		// TODO: Record hit container

		private readonly ConcurrentBag<HttpHealthResult> _results = new ConcurrentBag<HttpHealthResult>();

		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		private readonly Uri _endpoint;

		public HttpTester(Uri endpoint)
		{
			_endpoint = endpoint;

			Start();
		}

		public void Start()
		{
			Task.Run(async () =>
			{
				var client = new HttpClient()
				{
					Timeout = TimeSpan.FromSeconds(15)
				};

				var sw = new Stopwatch();

				while (!_cts.Token.IsCancellationRequested)
				{
					try
					{
						sw.Restart();

						var req = new HttpRequestMessage()
						{
							RequestUri = _endpoint
						};

						var resp = await client.SendAsync(req);

						_results.Add(new HttpHealthResult()
						{
							Succeeded = true,
							StatusCode = resp.StatusCode,
							Duration = sw.Elapsed
						});
					}
					catch (Exception ex)
					{
						_results.Add(new HttpHealthResult()
						{
							Succeeded = false,
							Duration = sw.Elapsed,
							Error = ex.Message
						});
					}
				}
			});
		}

		public void Stop()
		{
			_cts.Cancel();
		}

		public void AssertSuccessful()
		{
			var results = string.Join(", ", _results
				.GroupBy(res => new { res.StatusCode, res.Error })
				.Select(res => $"{res.Key} ({res.Count()})")
			);

			Log.Information($"HttpTester result: {results}");

			Assert.IsTrue(_results.All(res => res.Succeeded && res.StatusCode == HttpStatusCode.OK));
		}

		public async Task WaitForHitAsync(CancellationToken cancellationToken = default)
		{
			var cCount = _results.Count;

			while (!cancellationToken.IsCancellationRequested)
			{
				if (_results.Count > cCount) return;

				await Task.Delay(TimeSpan.FromMilliseconds(250));
			}
		}

		public void Dispose()
		{
			Stop();
		}

		private class HttpHealthResult
		{
			public bool Succeeded { get; set; }

			public HttpStatusCode? StatusCode { get; set; }

			public TimeSpan Duration { get; set; }

			public string? Error { get; set; }
		}
	}
}