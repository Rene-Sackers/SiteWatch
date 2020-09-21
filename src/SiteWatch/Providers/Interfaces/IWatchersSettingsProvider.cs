using System.Threading.Tasks;
using SiteWatch.Models;

namespace SiteWatch.Providers.Interfaces
{
	public interface IWatchersSettingsProvider
	{
		WatchersSettings Settings { get; }

		Task Save(bool trigerSettingsChanged = true);
		event WatchersWatchersSettingsProvider.SettingsChangedHandler SettingsChanged;
	}
}