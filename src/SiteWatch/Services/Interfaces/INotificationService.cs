using System.Threading.Tasks;
using SiteWatch.Models;

namespace SiteWatch.Services.Interfaces
{
	public interface INotificationService
	{
		Task NotifyPageChanged(PageWatcher pageWatcher, string url, string oldHtml, string newHtml);
	}
}