using System.Collections.Generic;

namespace SiteWatch.Models
{
	public class Settings
	{
		public List<PageWatcher> PageWatchers { get; set; } =  new List<PageWatcher>();
	}
}
