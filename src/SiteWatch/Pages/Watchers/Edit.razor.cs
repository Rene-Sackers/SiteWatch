using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;

namespace SiteWatch.Pages.Watchers
{
	public class EditWatcherModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string QuerySelector { get; set; }

		[Required]
		public string Interval { get; set; }

		public List<string> Urls { get; set; } = new List<string>();
	}

	public class EditBase : ComponentBase
	{
		[Inject]
		public ISettingsProvider SettingsProvider { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		[Parameter]
		public int Id { get; set; }

		protected bool IsAddingUrl { get; set; }

		protected string NewUrl { get; set; }

		protected EditWatcherModel EditWatcherModel { get; set; }

		protected EditContext EditContext { get; set; }

		private ValidationMessageStore _validationMessageStore;

		private PageWatcher _watcher;
		
		protected override void OnInitialized()
		{
			SetUpAddWatcherContext();
		}

		private void SetUpAddWatcherContext()
		{
			_watcher = SettingsProvider.Settings.PageWatchers.FirstOrDefault(w => w.Id == Id);
			if (_watcher == null)
			{
				NavigationManager.NavigateTo("/");
				return;
			}

			EditWatcherModel = new EditWatcherModel
			{
				Name = _watcher.Name,
				QuerySelector = _watcher.QuerySelector,
				Interval = _watcher.Interval.ToString(),
				Urls = _watcher.Urls.ToList()
			};
			EditContext = new EditContext(EditWatcherModel);
			_validationMessageStore = new ValidationMessageStore(EditContext);
		}

		protected void Submit()
		{
			_validationMessageStore.Clear();

			if (!EditContext.Validate())
				return;

			var otherWatcherHasSameName = SettingsProvider.Settings.PageWatchers
				.Any(pw => pw.Id != _watcher.Id && string.Equals(pw.Name, EditWatcherModel.Name, StringComparison.OrdinalIgnoreCase));
			if (otherWatcherHasSameName)
			{
				_validationMessageStore.Add(() => EditWatcherModel.Name, "Watcher name already in use.");
				return;
			}

			_watcher.Name = EditWatcherModel.Name;
			_watcher.QuerySelector = EditWatcherModel.QuerySelector;
			_watcher.Interval = Math.Max(10, int.Parse(EditWatcherModel.Interval));

			SettingsProvider.Save();

			NavigationManager.NavigateTo("/");
		}
	}
}
