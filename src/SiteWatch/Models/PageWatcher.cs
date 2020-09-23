using System;
using System.Collections.Generic;

namespace SiteWatch.Models
{
	public class PageWatcher
	{
		public const int MinimumInterval = 10;

		public bool Enabled { get; set; } = true;

		public int Id { get; set; }

		public int Interval { get; set; } = 60;

		public string Name { get; set; }

		public IGetDataAction GetDataAction { get; set; }

		public IParseDataAction ParseDataAction { get; set; }

		public List<string> Urls { get; set; } = new List<string>();

		public Dictionary<string, PageState> PageStates = new Dictionary<string, PageState>();
	}

	public interface IGetDataAction
	{
	}

	public interface IParseDataAction
	{
	}

	public abstract class HttpAction : IGetDataAction
	{
		public List<KeyValueItem> Headers { get; set; } = new List<KeyValueItem>();

		public List<string> Urls { get; set; } = new List<string>();
	}

	public class HttpGetAction : HttpAction
	{
		public List<KeyValueItem> QueryParameters { get; set;} = new List<KeyValueItem>();
	}

	public class HttpPostAction : HttpAction
	{
		public List<KeyValueItem> FormData { get; set; } = new List<KeyValueItem>();
	}

	public class HtmlQuerySelectorAction : IParseDataAction
	{
		public string QuerySelector { get; set; }
	}

	public class JsonPathAction : IParseDataAction
	{
		public string Path { get; set; }
	}
}