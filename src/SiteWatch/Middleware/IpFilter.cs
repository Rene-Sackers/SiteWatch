using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;
using SiteWatch.Providers;

namespace SiteWatch.Middleware
{
	public class IpFilter
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly RequestDelegate _next;
		private readonly IPAddress[] _whitelistedAddresses;

		public IpFilter(RequestDelegate next, ISettingsProvider settingsProvider)
		{
			_next = next;

			_whitelistedAddresses = settingsProvider
				.Settings
				.IpFilterSettings
				?.Whitelist
				?.Select(IPAddress.Parse)
				.ToArray();

			Logger.Info("Loaded IP whitelist {whitelist}", _whitelistedAddresses);
		}

		public async Task Invoke(HttpContext context)
		{
			// No whitelist configured
			if (_whitelistedAddresses?.Any() != true)
			{
				await _next.Invoke(context);
				return;
			}

			var ipAddress = context.Connection.RemoteIpAddress;
			var isWhitelisted = _whitelistedAddresses.Any(ip => ip.Equals(ipAddress));

			if (!isWhitelisted)
			{
				Logger.Warn("Blocked non-whitelisted IP: {ip}", ipAddress);
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				return;
			}

			await _next.Invoke(context);
		}
	}
}
