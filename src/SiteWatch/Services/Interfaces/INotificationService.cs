using SiteWatch.Models;

namespace SiteWatch.Services.Interfaces
{
	public interface INotificationService
	{
		void NotifyPageChanged(PageWatcher pageWatcher, string url, string oldHtml, string newHtml);
	}
}