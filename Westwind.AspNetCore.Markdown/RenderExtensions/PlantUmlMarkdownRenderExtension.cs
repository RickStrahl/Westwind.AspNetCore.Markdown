using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown;

/// <summary>
/// Renders PlantUML diagrams using plantuml code block syntax
/// into a rendered image in preview and rendered output.
/// </summary>
public class PlantUmlMarkdownRenderExtension : IMarkdownRenderExtension
{
    private const string StartUmlString = "\n```plantuml";
    private static readonly Regex plantUmlRegex = new Regex(@"(\n```plantuml[\S\s]).*?([\s\S]```)", RegexOptions.Singleline);

    /// <summary>
    /// The URL of the PlantUML server to render diagrams. Override this
    /// to render to /svg/ or /txt/ for different formats.
    /// </summary>
    public string PlantUmlServerUrl = "http://www.plantuml.com/plantuml/png/";


    /// <summary>
    /// Fix up input Markdown before it's rendered by converting plantuml codeblocks
    /// into image elements with a link wrapper to the PlantUML server editor.
    /// </summary>
    /// <param name="args"></param>
    public void BeforeMarkdownRendered(ModifyMarkdownArguments args)
    {
        if (string.IsNullOrEmpty(args.Markdown) ||
            !args.Markdown.Contains(StartUmlString))
            return;

        var markdown = args.Markdown;
        var matches = plantUmlRegex.Matches(markdown);
        foreach (var match in matches)
        {
            var origBlock = match?.ToString();
            var umlBlock = origBlock.TrimStart();
            if (umlBlock.StartsWith("```"))
                umlBlock = umlBlock.Replace("```plantuml", string.Empty).Trim(' ', '`', '\n', '\r');

            var url = PlantUmlServerUrl + PlantUmlTextEncoding.EncodeUrl(umlBlock);
            var html = $"<img src=\"{url}\" alt=\"diagram\" />";

            if (url.Contains("plantuml.com"))
            {
                var linkUrl = url.ReplaceMany(new string[] { "/svg/", "/png/", "/txt/" }, "/uml/");
                if (!string.IsNullOrEmpty(linkUrl))
                    html = $"<a href=\"{linkUrl}\" target=\"_blank\">{html}</a>";
            }

            markdown = markdown.Replace(origBlock, html);
        }

        args.Markdown = markdown;
    }

    public void AfterMarkdownRendered(ModifyHtmlAndHeadersArguments args)
    { }


    public void AfterDocumentRendered(ModifyHtmlArguments args)
    { }
}



/// <summary>
/// Provides methods for encoding PlantUML text, as described at http://plantuml.com/text-encoding
/// </summary>
internal static class PlantUmlTextEncoding
{
    public static string EncodeUrl(string text)
    {
        return Encode64(Deflate(ToUtf8(text)));
    }

    private static string ToUtf8(string text)
    {
        return Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
    }

    private static byte[] Deflate(string text)
    {
        using (var ms = new MemoryStream())
        {
            using (var deflate = new DeflateStream(ms, CompressionMode.Compress))
            {
                using (var writer = new StreamWriter(deflate, Encoding.UTF8))
                {
                    writer.Write(text);
                }
            }

            return ms.ToArray();
        }
    }

    private static string Encode64(byte[] data)
    {
        var r = new StringBuilder();

        for (var i = 0; i < data.Length; i += 3)
        {
            if (i + 2 == data.Length)
            {
                r.Append(Append3Bytes(data[i], data[i + 1], 0));
            }
            else if (i + 1 == data.Length)
            {
                r.Append(Append3Bytes(data[i], 0, 0));
            }
            else
            {
                r.Append(Append3Bytes(data[i], data[i + 1], data[i + 2]));
            }
        }

        return r.ToString();
    }

    private static string Append3Bytes(byte b1, byte b2, int b3)
    {
        var c1 = b1 >> 2;
        var c2 = (b1 & 0x3) << 4 | b2 >> 4;
        var c3 = (b2 & 0xF) << 2 | b3 >> 6;
        var c4 = b3 & 0x3F;
        var r = new StringBuilder();
        r.Append(Encode6bit(c1 & 0x3F));
        r.Append(Encode6bit(c2 & 0x3F));
        r.Append(Encode6bit(c3 & 0x3F));
        r.Append(Encode6bit(c4 & 0x3F));
        return r.ToString();
    }

    private static char Encode6bit(int b)
    {
        if (b < 10)
        {
            return Convert.ToChar(48 + b);
        }
        b -= 10;
        if (b < 26)
        {
            return Convert.ToChar(65 + b);
        }
        b -= 26;
        if (b < 26)
        {
            return Convert.ToChar(97 + b);
        }
        b -= 26;
        if (b == 0)
        {
            return '-';
        }
        if (b == 1)
        {
            return '_';
        }
        return '?';
    }
}





