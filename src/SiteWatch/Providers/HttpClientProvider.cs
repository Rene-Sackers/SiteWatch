using System;
using System.Net.Http;
using System.Net.Http.Headers;
using SiteWatch.Providers.Interfaces;

namespace SiteWatch.Providers
{
	public class HttpClientProvider : IDisposable, IHttpClientProvider
	{
		private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(ClientFactory);

		public HttpClient Client => _httpClient.Value;

		private static HttpClient ClientFactory()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36 Edg/85.0.564.44");
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

			return client;
		}

		public void Dispose()
		{
			if (_httpClient.IsValueCreated)
				_httpClient.Value.Dispose();
		}
	}
}
