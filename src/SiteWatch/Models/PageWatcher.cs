using System.Collections.Generic;

namespace SiteWatch.Models
{
	public class PageWatcher
	{
		public int Id { get; set; }

		public int Interval { get; set; } = 60;

		public string Name { get; set; }

		public string QuerySelector { get; set; }

		public List<string> Urls { get; set; } = new List<string>();
	}
}