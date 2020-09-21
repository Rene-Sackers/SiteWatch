using System.Threading.Tasks;

namespace SiteWatch.Services.Interfaces
{
	public interface IPageScrapeService
	{
		Task<string> GetHtmlForQueryString(string url, string queryString);
	}
}