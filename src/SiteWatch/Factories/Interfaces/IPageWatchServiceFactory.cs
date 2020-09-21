using SiteWatch.Models;
using SiteWatch.Services;

namespace SiteWatch.Factories.Interfaces
{
	public interface IPageWatchServiceFactory
	{
		PageWatchService Create(PageWatcher pageWatcher);
	}
}