using System.Linq;
using System.Text.RegularExpressions;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Base class that includes various fix up methods for custom parsing
    /// that can be called by the specific implementations.
    /// </summary>
    public abstract class MarkdownParserBase : IMarkdownParser
    {
        protected static Regex strikeOutRegex =
            new Regex("~~.*?~~", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Parses markdown
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="sanitizeHtml">If true sanitizes the generated HTML by removing script tags and other common XSS issues.
        /// Note: Not a complete XSS solution but addresses the most obvious vulnerabilities. For more thourough HTML sanitation
        /// </param>
        /// <returns></returns>
        public abstract string Parse(string markdown, bool sanitizeHtml = true);

        /// <summary>
        /// Parses strikeout text ~~text~~. Single line (to linebreak) allowed only.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ParseStrikeout(string html)
        {
            if (html == null)
                return null;

            var matches = strikeOutRegex.Matches(html);
            foreach (Match match in matches)
            {
                string val = match.Value;

                if (match.Value.Contains('\n'))
                    continue;

                val = "<del>" + val.Substring(2, val.Length - 4) + "</del>";
                html = html.Replace(match.Value, val);
            }

            return html;
        }

        static readonly Regex YamlExtractionRegex = new Regex("^---[\n,\r\n].*?^---[\n,\r\n]",
            RegexOptions.Singleline | RegexOptions.Multiline);

        /// <summary>
        /// Strips 
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public string StripFrontMatter(string markdown)
        {
            string extractedYaml = null;
            var match = YamlExtractionRegex.Match(markdown);
            if (match.Success)
                extractedYaml = match.Value;

            if (!string.IsNullOrEmpty(extractedYaml))
                markdown = markdown.Replace(extractedYaml, "");

            return markdown;
        }

        #region Html Sanitation

        /// <summary>
        /// Global list of tags that are cleaned up by the script sanitation
        /// as a pipe separated list.
        ///
        /// You can change this value which applies to the scriptScriptTags
        /// option, but it is an application wide global setting.
        /// 
        /// Default: script|iframe|object|embed|form
        /// </summary>
        public static string HtmlSanitizeTagBlackList { get; set; } = "script|iframe|object|embed|form";

        /// <summary>
        /// Parses out script tags that might not be encoded yet
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string Sanitize(string html)
        {
            return StringUtils.SanitizeHtml(html,HtmlSanitizeTagBlackList);
        }

        #endregion

        public static Regex fontAwesomeIconRegEx = new Regex(@"@icon-.*?[\s|\.|\,|\<]");

        /// <summary>
        /// Post processing routine that post-processes the HTML and 
        /// replaces @icon- with fontawesome icons
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ParseFontAwesomeIcons(string html)
        {
            var matches = fontAwesomeIconRegEx.Matches(html);
            foreach (Match match in matches)
            {
                string iconblock = match.Value.Substring(0, match.Value.Length - 1);
                string icon = iconblock.Replace("@icon-", "");
                html = html.Replace(iconblock, "<i class=\"fa fa-" + icon + "\"></i> ");
            }

            return html;
        }

    }
}
