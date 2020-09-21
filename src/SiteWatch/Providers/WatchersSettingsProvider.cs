using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;

namespace SiteWatch.Providers
{
	public class WatchersSettingsProvider : IWatchersSettingsProvider
	{
		private const string SettingsFileName = "watch.json";

		private static readonly SemaphoreSlim SaveSemaphore = new SemaphoreSlim(1);

		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
			TypeNameHandling = TypeNameHandling.Auto,
			Formatting = Formatting.Indented
		};

		private readonly Lazy<WatchersSettings> _settingsLazy = new Lazy<WatchersSettings>(SettingsFactory);

		public WatchersSettings Settings => _settingsLazy.Value;

		public delegate void SettingsChangedHandler();

		public event SettingsChangedHandler SettingsChanged;

		private static WatchersSettings SettingsFactory()
		{
			var path = GetFullFilePath();
			if (!File.Exists(path))
			{
				return new WatchersSettings();
			}

			var json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<WatchersSettings>(json, JsonSerializerSettings);
		}

		private static string GetFullFilePath()
			=> Path.GetFullPath($"./{SettingsFileName}");

		public async Task Save(bool trigerSettingsChanged = true)
		{
			await SaveSemaphore.WaitAsync();

			try
			{
				var path = GetFullFilePath();
				var json = JsonConvert.SerializeObject(Settings, JsonSerializerSettings);
				await File.WriteAllTextAsync(path, json);

				// Currently, the watch service listens for this event and will re-create all watchers, temporarily added a boolean to skip the call when not needed
				// Implement a repository with add/remove/update functions so we know when to recreate watchers instead of a single settings provider.
				if (trigerSettingsChanged)
					SettingsChanged?.Invoke();
			}
			finally
			{
				SaveSemaphore.Release();
			}

		}
	}
}
