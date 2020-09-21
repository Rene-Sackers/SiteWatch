using System;
using SiteWatch.Providers.Interfaces;
using Telegram.Bot;

namespace SiteWatch.Providers
{
	public class TelegramBotClientProvider : ITelegramBotClientProvider
	{
		private readonly ISettingsProvider _settingsProvider;

		private readonly Lazy<TelegramBotClient> _clientLazy;

		public TelegramBotClient Client => _clientLazy.Value;

		public TelegramBotClientProvider(ISettingsProvider settingsProvider)
		{
			_settingsProvider = settingsProvider;
			_clientLazy = new Lazy<TelegramBotClient>(BotClientFactory);
		}

		private TelegramBotClient BotClientFactory()
			=> new TelegramBotClient(_settingsProvider.Settings.Telegram.ApiToken);
	}
}
