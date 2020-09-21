using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;

namespace SiteWatch.Pages
{
	public class AddWatcherModel
	{
		[Required]
		public string Name { get; set; }
	}

	public class IndexBase : ComponentBase
	{
		[Inject]
		public IWatchersSettingsProvider WatchersSettingsProvider { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		protected bool IsAddingWatcher { get; set; }

		protected AddWatcherModel AddWatcherModel { get; set; }

		protected EditContext AddWatcherEditContext { get; set; }

		protected PageWatcher DeleteWatcher { get; set; }

		private ValidationMessageStore _addWatcherValidationMessageStore;

		protected override void OnInitialized()
		{
			SetUpAddWatcherContext();
		}

		private void SetUpAddWatcherContext()
		{
			AddWatcherModel = new AddWatcherModel();
			AddWatcherEditContext = new EditContext(AddWatcherModel);
			_addWatcherValidationMessageStore = new ValidationMessageStore(AddWatcherEditContext);
		}

		protected void SubmitAddWatcher()
		{
			_addWatcherValidationMessageStore.Clear();

			if (!AddWatcherEditContext.Validate())
				return;

			var otherWatcherHasSameName = WatchersSettingsProvider.Settings.PageWatchers
				.Any(pw => string.Equals(pw.Name, AddWatcherModel.Name, StringComparison.OrdinalIgnoreCase));
			if (otherWatcherHasSameName)
			{
				_addWatcherValidationMessageStore.Add(() => AddWatcherModel.Name, "Watcher name already in use.");
				return;
			}

			var nextId = WatchersSettingsProvider.Settings.PageWatchers.Any()
				? WatchersSettingsProvider.Settings.PageWatchers.Max(pw => pw.Id) + 1
				: 1;

			WatchersSettingsProvider.Settings.PageWatchers.Add(new PageWatcher
			{
				Id = nextId,
				Name = AddWatcherModel.Name
			});

			WatchersSettingsProvider.Save();

			SetUpAddWatcherContext();
			IsAddingWatcher = false;

			NavigationManager.NavigateTo($"/Watchers/Edit/{nextId}");
		}

		protected void ConfirmDeleteWatcher()
		{
			if (DeleteWatcher == null)
				return;

			WatchersSettingsProvider.Settings.PageWatchers.Remove(DeleteWatcher);
			WatchersSettingsProvider.Save();

			DeleteWatcher = null;
		}
	}
}
