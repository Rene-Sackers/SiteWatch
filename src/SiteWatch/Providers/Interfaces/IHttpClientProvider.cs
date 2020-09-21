using System.Net.Http;

namespace SiteWatch.Providers.Interfaces
{
	public interface IHttpClientProvider
	{
		HttpClient Client { get; }
	}
}