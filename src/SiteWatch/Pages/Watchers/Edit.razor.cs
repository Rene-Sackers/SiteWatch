using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SiteWatch.Extensions;
using SiteWatch.Models;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services.Interfaces;

namespace SiteWatch.Pages.Watchers
{
	public class EditWatcherModel
	{
		public bool Enabled { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Interval { get; set; }

		public IGetDataAction GetDataAction { get; set; }

		public IParseDataAction ParseDataAction { get; set; }
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

		protected EditWatcherModel EditWatcherModel { get; set; }

		protected EditContext EditContext { get; set; }

		private ValidationMessageStore _editValidationMessageStore;

		protected string PreviewQueryHtmlResult { get; set; }

		private PageWatcher _watcher;

		private string _getDataActionName;

		protected string GetDataActionName
		{
			get => _getDataActionName;
			set
			{
				_getDataActionName = value;
				GetDataActionChanged();
			}
		}

		private string _parseDataActionName;

		protected string ParseDataActionName
		{
			get => _parseDataActionName;
			set
			{
				_parseDataActionName = value;
				ParseDataActionChanged();
			}
		}

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

			GetDataActionName = _watcher.GetDataAction?.GetType().Name ?? nameof(HttpGetAction);
			ParseDataActionName = _watcher.ParseDataAction?.GetType().Name ?? nameof(HtmlQuerySelectorAction);
			EditWatcherModel = new EditWatcherModel
			{
				Enabled = _watcher.Enabled,
				Name = _watcher.Name,
				Interval = _watcher.Interval.ToString(),
				GetDataAction = _watcher.GetDataAction.Clone() ?? new HttpGetAction(),
				ParseDataAction = _watcher.ParseDataAction.Clone() ?? new HtmlQuerySelectorAction()
			};

			EditContext = new EditContext(EditWatcherModel);
			_editValidationMessageStore = new ValidationMessageStore(EditContext);
		}

		protected async Task PreviewQuery(string url)
		{
			//PreviewQueryHtmlResult = TelegramHtmlFormatter.HtmlToTelegramFormattedText(await PageScrapeService.GetHtmlForQueryString(url, EditWatcherModel.QuerySelector));
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

			_watcher.Enabled = EditWatcherModel.Enabled;
			_watcher.Name = EditWatcherModel.Name;
			_watcher.GetDataAction = EditWatcherModel.GetDataAction;
			_watcher.ParseDataAction = EditWatcherModel.ParseDataAction;
			_watcher.Interval = Math.Max(PageWatcher.MinimumInterval, parsedInterval);

			WatchersSettingsProvider.Save();

			NavigationManager.NavigateTo("/");
		}

		protected void GetDataActionChanged()
		{
			if (EditWatcherModel == null)
				return;

			switch (GetDataActionName)
			{
				case nameof(HttpGetAction):
					EditWatcherModel.GetDataAction = new HttpGetAction();
					break;
				case nameof(HttpPostAction):
					EditWatcherModel.GetDataAction = new HttpPostAction();
					break;
			}

			StateHasChanged();
		}

		protected void ParseDataActionChanged()
		{
			if (EditWatcherModel == null)
				return;

			switch (ParseDataActionName)
			{
				case nameof(HtmlQuerySelectorAction):
					EditWatcherModel.ParseDataAction = new HtmlQuerySelectorAction();
					break;
				case nameof(JsonPathAction):
					EditWatcherModel.ParseDataAction = new JsonPathAction();
					break;
			}

			StateHasChanged();
		}
	}
}
