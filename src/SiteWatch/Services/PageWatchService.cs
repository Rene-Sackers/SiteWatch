using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services.Interfaces;

namespace SiteWatch.Services
{
	public class PageWatchService : IDisposable
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly PageWatcher _watcher;
		private readonly IPageScrapeService _pageScrapeService;
		private readonly INotificationService _notificationService;
		private readonly IWatchersSettingsProvider _watchersSettingsProvider;

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly TaskCompletionSource<object> _stopTaskCompletionSource = new TaskCompletionSource<object>();

		public PageWatchService(
			PageWatcher watcher,
			IPageScrapeService pageScrapeService,
			INotificationService notificationService,
			IWatchersSettingsProvider watchersSettingsProvider)
		{
			_watcher = watcher;
			_pageScrapeService = pageScrapeService;
			_notificationService = notificationService;
			_watchersSettingsProvider = watchersSettingsProvider;
		}

		public void Start()
		{
			if (_cancellationTokenSource.IsCancellationRequested)
				throw new InvalidOperationException("Can't start after service has been stopped.");

			Logger.Info("Starting watcher {watcherName}", _watcher.Name);

			WatchInterval();
		}

		public async Task Stop()
		{
			Logger.Info("Stopping watcher {watcherName}", _watcher.Name);

			_cancellationTokenSource.Cancel(false);
			await _stopTaskCompletionSource.Task;
		}

		private async void WatchInterval()
		{
			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				Logger.Info("Interval lapsed. Watcher: {watcherName}", _watcher.Name);

				await ScrapePages();

				try
				{
					await Task.Delay(TimeSpan.FromSeconds(Math.Max(PageWatcher.MinimumInterval, _watcher.Interval)), _cancellationTokenSource.Token);
				}
				catch (TaskCanceledException)
				{
					// ignore
				}
			}

			_stopTaskCompletionSource.SetResult(null);
		}

		private async Task ScrapePages()
		{
			foreach (var url in _watcher.Urls)
			{
				var newHtml = await _pageScrapeService.GetHtmlForQueryString(url, _watcher.QuerySelector);

				if (!_watcher.PageStates.ContainsKey(url))
					_watcher.PageStates.Add(url, new PageState());

				var pageState = _watcher.PageStates[url];

				pageState.LastCheck = DateTimeOffset.Now;

				if (pageState.LastHtml == newHtml)
					continue;

				pageState.LastChange = DateTimeOffset.Now;


				await _notificationService.NotifyPageChanged(_watcher, url, pageState.LastHtml, newHtml);

				pageState.LastHtml = newHtml;

				await _watchersSettingsProvider.Save(false);
			}
		}

		public void Dispose()
		{
			_cancellationTokenSource?.Dispose();
		}
	}
}