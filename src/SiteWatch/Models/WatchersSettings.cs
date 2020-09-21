using System.Collections.Generic;

namespace SiteWatch.Models
{
	public class WatchersSettings
	{
		public List<PageWatcher> PageWatchers { get; set; } =  new List<PageWatcher>();
	}
}
