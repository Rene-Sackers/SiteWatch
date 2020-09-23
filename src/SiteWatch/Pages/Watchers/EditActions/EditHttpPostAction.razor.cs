using Microsoft.AspNetCore.Components;
using SiteWatch.Models;

namespace SiteWatch.Pages.Watchers.EditActions
{
	public class EditHttpPostActionBase : ComponentBase
	{
		[Parameter]
		public HttpPostAction HttpPostAction { get; set; }
	}
}
