namespace SiteWatch.Models
{
	public class Settings
	{
		public TelegramSettings Telegram { get; set; }

		public IpFilterSettings IpFilterSettings { get; set; }
	}

	public class TelegramSettings
	{
		public string ApiToken { get; set; }

		public int ChatId { get; set; }
	}
}
