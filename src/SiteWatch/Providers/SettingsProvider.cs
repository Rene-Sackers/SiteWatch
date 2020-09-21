using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SiteWatch.Providers
{
	public class SettingsProvider : ISettingsProvider
	{
		private readonly Lazy<Models.Settings> _settingsLazy;

		public Models.Settings Settings => _settingsLazy.Value;

		public SettingsProvider()
		{
			_settingsLazy = new Lazy<Models.Settings>(SettingsFactory);
		}

		private static Models.Settings SettingsFactory()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile("appsettings.Development.json", true)
				.Build();

			return configuration.Get<Models.Settings>();
		}
	}
}
