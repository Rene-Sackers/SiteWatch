using System;
using System.Threading.Tasks;
using NLog;
using SiteWatch.Helpers;
using SiteWatch.Models;
using SiteWatch.Providers;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace SiteWatch.Services
{
	public class TelegramNotificationService : INotificationService
	{
		private readonly ITelegramBotClientProvider _telegramBotClientProvider;
		private readonly ISettingsProvider _settingsProvider;

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public TelegramNotificationService(
			ITelegramBotClientProvider telegramBotClientProvider,
			ISettingsProvider settingsProvider)
		{
			_telegramBotClientProvider = telegramBotClientProvider;
			_settingsProvider = settingsProvider;
		}

		public async Task NotifyPageChanged(PageWatcher pageWatcher, string url, string oldHtml, string newHtml)
		{
			Logger.Info("Send Telegram notification for watcher {watcherName}, URL: {url}", pageWatcher.Name, url);

			var telegramMarkup = TelegramHtmlFormatter.HtmlToTelegramFormattedText(newHtml);
			var message = "Page changed\r\n" +
				$"Watcher: {pageWatcher.Name}\r\n" +
				$"URL: {url}\r\n" +
				"New content:\r\n" +
				$"{telegramMarkup}";

			if (_settingsProvider.Settings.Telegram.ChatId == 0)
			{
				Logger.Error("Telegram chat ID is not configured");
				return;
			}

			try
			{
				await _telegramBotClientProvider.Client.SendTextMessageAsync(
					_settingsProvider.Settings.Telegram.ChatId,
					message,
					ParseMode.Markdown,
					disableWebPagePreview: true);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Failed to send Telegram notification");
			}
		}
	}
}
