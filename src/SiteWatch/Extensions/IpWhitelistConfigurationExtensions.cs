using Microsoft.AspNetCore.Builder;
using SiteWatch.Middleware;

namespace SiteWatch.Extensions
{
	public static class IpWhitelistConfigurationExtensions
	{
		public static IApplicationBuilder UseIpWhitelist(this IApplicationBuilder applicationBuilder)
		{
			return applicationBuilder.UseMiddleware<IpFilter>();
		}
	}
}
