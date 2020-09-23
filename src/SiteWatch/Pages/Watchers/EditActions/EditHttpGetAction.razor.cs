using Microsoft.AspNetCore.Components;
using SiteWatch.Models;

namespace SiteWatch.Pages.Watchers.EditActions
{
	public class EditHttpGetActionBase : ComponentBase
	{
		[Parameter]
		public HttpGetAction HttpGetAction { get; set; }
	}
}
