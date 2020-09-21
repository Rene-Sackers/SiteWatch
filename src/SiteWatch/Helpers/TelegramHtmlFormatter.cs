using System;
using System.Text.RegularExpressions;

namespace SiteWatch.Helpers
{
	public static class TelegramHtmlFormatter
	{
		private static readonly Regex ParagraphRegex = new Regex(@"<\/p([^>]*)>", RegexOptions.Compiled);
		private static readonly Regex BoldRegex = new Regex(@"<(?<tag>h\d|b|strong)>(?<content>.+)<\/(\k<tag>)>", RegexOptions.Compiled);
		private static readonly Regex NewLineRegex = new Regex("<br.*?>", RegexOptions.Compiled);
		private static readonly Regex AnchorRegex = new Regex(@"<a .*?href=""(?<href>[^""]*)[^>]*>(?<text>[^<]*)<\/\s*?a>", RegexOptions.Compiled);
		private static readonly Regex ScriptTagRegex = new Regex(@"<script.*?>[^<]*<\/.*?script>", RegexOptions.Compiled);
		private static readonly Regex FinalTagRegex = new Regex("<.*?>", RegexOptions.Compiled);

		public static string HtmlToTelegramFormattedText(string html)
		{
			html = ParagraphRegex.Replace(html, Environment.NewLine);
			html = BoldRegex.Replace(html, "**${content}**");
			html = NewLineRegex.Replace(html, Environment.NewLine);
			html = AnchorRegex.Replace(html, "[${text}](${href})");
			html = ScriptTagRegex.Replace(html, string.Empty);
			html = FinalTagRegex.Replace(html, string.Empty);
			html = html.Replace("  ", string.Empty);
			html = html.Replace("\r\n", "\n");

			const string tripleNewLine = "\n\n\n";
			while (html.IndexOf(tripleNewLine, StringComparison.Ordinal) > -1)
			{
				html = html.Replace(tripleNewLine, "\n\n");
			}

			if ("\n" != Environment.NewLine)
				html = html.Replace("\n", Environment.NewLine);

			return html;
		}
	}
}
