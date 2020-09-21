using Telegram.Bot;

namespace SiteWatch.Providers.Interfaces
{
	public interface ITelegramBotClientProvider
	{
		TelegramBotClient Client { get; }
	}
}