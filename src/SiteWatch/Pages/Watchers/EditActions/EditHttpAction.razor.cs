using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SiteWatch.Models;

namespace SiteWatch.Pages.Watchers.EditActions
{
	public class EditHttpActionBase : ComponentBase
	{
		[Parameter]
		public HttpAction HttpAction { get; set; }

		protected AddUrlModel AddUrlModel { get; set; }

		protected EditContext AddUrlEditContext { get; set; }

		private ValidationMessageStore _addUrlValidationMessageStore;

		protected string DeleteUrl { get; set; }

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

			if (HttpAction.Urls.Any(u => string.Equals(u, AddUrlModel.Url, StringComparison.OrdinalIgnoreCase)))
			{
				_addUrlValidationMessageStore.Add(() => AddUrlModel.Url, "This URL was already added to the watcher.");
				return;
			}

			if (!Uri.TryCreate(AddUrlModel.Url, UriKind.Absolute, out _))
			{
				_addUrlValidationMessageStore.Add(() => AddUrlModel.Url, "This is not a valid URL.");
				return;
			}

			HttpAction.Urls.Add(AddUrlModel.Url);

			AddUrlModel = null;
		}

		protected void DeleteUrlSubmit()
		{
			HttpAction.Urls.Remove(DeleteUrl);

			DeleteUrl = null;
		}
	}
}
