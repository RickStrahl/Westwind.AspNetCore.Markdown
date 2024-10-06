﻿using System.Text.RegularExpressions;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown;

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
    /// Render Extensions Manager used for this parser instance. Defaults
    /// to the global Singleton instance that can be globally configured
    /// on startup. Property provided here allows local overriding of the
    /// extension manager.
    /// </summary>
    public MarkdownRenderExtensionManager RenderExtensionManager { get; set; } = MarkdownRenderExtensionManager.Current;


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
