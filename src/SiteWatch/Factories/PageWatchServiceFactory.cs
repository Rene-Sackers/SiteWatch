using SiteWatch.Factories.Interfaces;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services;
using SiteWatch.Services.Interfaces;

namespace SiteWatch.Factories
{
	public class PageWatchServiceFactory : IPageWatchServiceFactory
	{
		private readonly IPageScrapeService _pageScrapeService;
		private readonly INotificationService _notificationService;
		private readonly IWatchersSettingsProvider _watchersSettingsProvider;

		public PageWatchServiceFactory(
			IPageScrapeService pageScrapeService,
			INotificationService notificationService,
			IWatchersSettingsProvider watchersSettingsProvider)
		{
			_pageScrapeService = pageScrapeService;
			_notificationService = notificationService;
			_watchersSettingsProvider = watchersSettingsProvider;
		}

		public PageWatchService Create(PageWatcher pageWatcher)
		{
			return new PageWatchService(
				pageWatcher,
				_pageScrapeService,
				_notificationService,
				_watchersSettingsProvider);
		}
	}
}
