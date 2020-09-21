using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SiteWatch.Helpers;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services.Interfaces;

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

	public class AddUrlModel
	{
		[Required]
		public string Url { get; set; }
	}

	public class EditBase : ComponentBase
	{
		[Inject]
		public IWatchersSettingsProvider WatchersSettingsProvider { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		[Inject]
		public IPageScrapeService PageScrapeService { get; set; }

		[Parameter]
		public int Id { get; set; }

		protected string DeleteUrl { get; set; }

		protected EditWatcherModel EditWatcherModel { get; set; }

		protected EditContext EditContext { get; set; }

		private ValidationMessageStore _editValidationMessageStore;

		protected AddUrlModel AddUrlModel { get; set; }

		protected EditContext AddUrlEditContext { get; set; }

		private ValidationMessageStore _addUrlValidationMessageStore;

		protected string PreviewQueryHtmlResult { get; set; }

		private PageWatcher _watcher;
		
		protected override void OnInitialized()
		{
			SetUpAddWatcherContext();
		}

		private void SetUpAddWatcherContext()
		{
			_watcher = WatchersSettingsProvider.Settings.PageWatchers.FirstOrDefault(w => w.Id == Id);
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
			_editValidationMessageStore = new ValidationMessageStore(EditContext);
		}

		private void SetUpAddUrlContext()
		{
			AddUrlModel = new AddUrlModel();
			AddUrlEditContext = new EditContext(AddUrlModel);
			_addUrlValidationMessageStore = new ValidationMessageStore(AddUrlEditContext);
		}

		protected void ShowAddUrl()
		{
			SetUpAddUrlContext();
		}

		protected void AddUrlSubmit()
		{
			_addUrlValidationMessageStore.Clear();

			if (!AddUrlEditContext.Validate())
				return;

			if (_watcher.Urls.Any(u => string.Equals(u, AddUrlModel.Url, StringComparison.OrdinalIgnoreCase)))
			{
				_addUrlValidationMessageStore.Add(() => AddUrlModel.Url, "This URL was already added to the watcher.");
				return;
			}

			if (!Uri.TryCreate(AddUrlModel.Url, UriKind.Absolute, out _))
			{
				_addUrlValidationMessageStore.Add(() => AddUrlModel.Url, "This is not a valid URL.");
				return;
			}

			_watcher.Urls.Add(AddUrlModel.Url);
			EditWatcherModel.Urls.Add(AddUrlModel.Url);
			WatchersSettingsProvider.Save();

			AddUrlModel = null;
		}

		protected void DeleteUrlSubmit()
		{
			_watcher.Urls.Remove(DeleteUrl);
			_watcher.PageStates.Remove(DeleteUrl);
			EditWatcherModel.Urls.Remove(DeleteUrl);
			WatchersSettingsProvider.Save();

			DeleteUrl = null;
		}

		protected async Task PreviewQuery(string url)
		{
			PreviewQueryHtmlResult = TelegramHtmlFormatter.HtmlToTelegramFormattedText(await PageScrapeService.GetHtmlForQueryString(url, EditWatcherModel.QuerySelector));
		}

		protected void Submit()
		{
			_editValidationMessageStore.Clear();

			if (!EditContext.Validate())
				return;

			var otherWatcherHasSameName = WatchersSettingsProvider.Settings.PageWatchers
				.Any(pw => pw.Id != _watcher.Id && string.Equals(pw.Name, EditWatcherModel.Name, StringComparison.OrdinalIgnoreCase));
			if (otherWatcherHasSameName)
			{
				_editValidationMessageStore.Add(() => EditWatcherModel.Name, "Watcher name already in use.");
				return;
			}

			if (!int.TryParse(EditWatcherModel.Interval, out var parsedInterval))
			{
				_editValidationMessageStore.Add(() => EditWatcherModel.Interval, "Not a valid number.");
				return;
			}

			_watcher.Name = EditWatcherModel.Name;
			_watcher.QuerySelector = EditWatcherModel.QuerySelector;
			_watcher.Interval = Math.Max(PageWatcher.MinimumInterval, parsedInterval);

			WatchersSettingsProvider.Save();

			NavigationManager.NavigateTo("/");
		}
	}
}
