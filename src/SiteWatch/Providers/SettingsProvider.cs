using System;
using System.IO;
using Newtonsoft.Json;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;

namespace SiteWatch.Providers
{
	public class SettingsProvider : ISettingsProvider
	{
		private const string SettingsFileName = "watch.json";

		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
			TypeNameHandling = TypeNameHandling.Auto,
			Formatting = Formatting.Indented
		};

		private readonly Lazy<Settings> _settingsLazy = new Lazy<Settings>(SettingsFactory);

		public Settings Settings => _settingsLazy.Value;

		private static Settings SettingsFactory()
		{
			var path = GetFullFilePath();
			if (!File.Exists(path))
			{
				return new Settings();
			}

			var json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<Settings>(json, JsonSerializerSettings);
		}

		private static string GetFullFilePath()
			=> Path.GetFullPath($"./{SettingsFileName}");

		public void Save()
		{
			var path = GetFullFilePath();
			var json = JsonConvert.SerializeObject(Settings, JsonSerializerSettings);
			File.WriteAllText(path, json);
		}
	}
}
