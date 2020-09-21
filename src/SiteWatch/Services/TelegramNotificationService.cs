using NLog;
using SiteWatch.Models;
using SiteWatch.Services.Interfaces;

namespace SiteWatch.Services
{
	public class TelegramNotificationService : INotificationService
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public void NotifyPageChanged(PageWatcher pageWatcher, string url, string oldHtml, string newHtml)
		{
			Logger.Info("Send Telegram notification for watcher {watcherName}, URL: {url}", pageWatcher.Name, url);
		}
	}
}
