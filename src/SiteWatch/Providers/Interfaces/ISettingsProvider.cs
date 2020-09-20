using SiteWatch.Models;

namespace SiteWatch.Providers.Interfaces
{
	public interface ISettingsProvider
	{
		Settings Settings { get; }

		void Save();
	}
}