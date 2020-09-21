using System;

namespace SiteWatch.Models
{
	public class PageState
	{
		public string LastHtml { get; set; }

		public DateTimeOffset LastCheck { get; set; }

		public DateTimeOffset LastChange { get; set; }
	}
}