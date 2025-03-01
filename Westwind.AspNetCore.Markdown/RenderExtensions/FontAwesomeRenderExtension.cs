using System.Text.RegularExpressions;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown;

/// <summary>
/// Render Extension that extends markdown rendering via `@icon-` sytnax
/// with a name of a font-awsome icon. 
///
/// * @icon-home  - renders `fas fa-home`
/// * @icon-regular-home - renders `far fa-home`
/// * @icon-duotone-home - renders `fad fa-home`
/// * @icon-solid-home - renders `fas fa-home`
///
/// Other:
/// -spin to spin
/// -color:steelblue
/// </summary>
public class FontAwesomeRenderExtension : IMarkdownRenderExtension
{
    public string Name { get; set; } = "FontAwesomeRenderExtension";

    private static Regex fontAwesomeIconRegEx = new Regex(@"@icon-.*?[\s|\.|\,|\<]");

    public void BeforeMarkdownRendered(ModifyMarkdownArguments args)
    {
        var md = args.Markdown;

        var matches = fontAwesomeIconRegEx.Matches(md);
        foreach (Match match in matches)
        {
            string iconblock = match.Value.Substring(0, match.Value.Length - 1);

            string faPrefix = "fas";
            string icon = null;
            if (iconblock.StartsWith("@icon-regular-"))
            {
                faPrefix = "far";
                icon = iconblock.Replace("@icon-regular-", "");
            }
            else if (iconblock.StartsWith("@icon-duotone-"))
            {
                faPrefix = "fad";
                icon = iconblock.Replace("@icon-duotone-", "");
            }
            else if (iconblock.StartsWith("@icon-solid-"))
            {
                faPrefix = "fas";
                icon = iconblock.Replace("@icon-solid-", "");
            }
            else if (iconblock.StartsWith("@icon-light-"))
            {
                faPrefix = "fal";
                icon = iconblock.Replace("@icon-light-", "");
            }
            else
            {
                faPrefix = "fas";
                icon = iconblock.Replace("@icon-", "");
            }


            string color = null;
            var idx = icon.IndexOf("-color:");
            if (idx > 0)
            {
                var extr = StringUtils.ExtractString(icon, "-color:", "-", caseSensitive: false, allowMissingEndDelimiter: true, returnDelimiters: true);
                color = StringUtils.ExtractString(extr, "-color:", "-", caseSensitive: false, allowMissingEndDelimiter: true, returnDelimiters: false);
                if (!string.IsNullOrEmpty(color))
                {
                    color = $";color: {color}";
                    icon = icon.Replace(extr.TrimEnd('-'), string.Empty);
                }
            }


            if (iconblock.EndsWith("-spin") || iconblock.Contains("-spin-"))
                icon = icon.Replace("-spin", " fa-spin ");

            md = md.Replace(iconblock, $"<i class=\"{faPrefix} fa-" + icon + $"\" style=\"font-size: 1.1em{color}\"></i> ");
        }

        if (md != args.Markdown)
            args.Markdown = md;
    }

    public void AfterMarkdownRendered(ModifyHtmlAndHeadersArguments args)
    { }

    public void AfterDocumentRendered(ModifyHtmlArguments args)
    { }

}
