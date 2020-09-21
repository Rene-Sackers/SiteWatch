using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using NLog;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services.Interfaces;

namespace SiteWatch.Services
{
	public class PageScrapeService : IPageScrapeService, IDisposable
	{
		private const int MaxConcurrentScrapes = 10;

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly IHttpClientProvider _httpClientProvider;
		private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(MaxConcurrentScrapes);

		public PageScrapeService(IHttpClientProvider httpClientProvider)
		{
			_httpClientProvider = httpClientProvider;
		}

		public async Task<string> GetHtmlForQueryString(string url, string queryString)
		{
			await _semaphoreSlim.WaitAsync();

			try
			{
				Logger.Info("Scraping {url}. Query string: {queryString}", url, queryString);

				if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(queryString))
					return null;

				var request = new HttpRequestMessage(HttpMethod.Get, url);
				request.Headers.Host = new Uri(url).Host;
				var response = await _httpClientProvider.Client.SendAsync(request);

				var document = new HtmlDocument();
				document.Load(await response.Content.ReadAsStreamAsync());

				return document.DocumentNode.QuerySelector(queryString)?.OuterHtml;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Failed to scrape {url} with query string {queryString}", url, queryString);
				return null;
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}

		public void Dispose()
		{
			_semaphoreSlim?.Dispose();
		}
	}
}
