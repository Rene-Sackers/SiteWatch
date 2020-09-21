using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services.Interfaces;

namespace SiteWatch.Services
{
	public class WatchService : IWatchService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private readonly IWatchersSettingsProvider _watchersSettingsProvider;
		private readonly IPageScrapeService _pageScrapeService;
		private readonly INotificationService _notificationService;
		private readonly List<PageWatchService> _watcherServices = new List<PageWatchService>();

		public WatchService(
			IWatchersSettingsProvider watchersSettingsProvider,
			IPageScrapeService pageScrapeService,
			INotificationService notificationService)
		{
			_watchersSettingsProvider = watchersSettingsProvider;
			_pageScrapeService = pageScrapeService;
			_notificationService = notificationService;

			_watchersSettingsProvider.SettingsChanged += WatchersSettingsProviderOnWatchersSettingsChanged;
		}

		private async void WatchersSettingsProviderOnWatchersSettingsChanged()
		{
			await SetUpWatchers();
		}

		public async Task SetUpWatchers()
		{
			Logger.Info("Setting up watchers");

			var stopTasks = _watcherServices.Select(ws => ws.Stop()).ToArray();
			await Task.WhenAll(stopTasks);
			_watcherServices.Clear();

			foreach (var pageWatcher in _watchersSettingsProvider.Settings.PageWatchers)
			{
				var watcherService = new PageWatchService(
					pageWatcher,
					_pageScrapeService,
					_notificationService,
					_watchersSettingsProvider);

				_watcherServices.Add(watcherService);

				watcherService.Start();
			}
		}
	}
}
