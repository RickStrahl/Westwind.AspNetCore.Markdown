#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 
 *          http://www.west-wind.com/
 * 
 * Created: 3/25/2018
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion


using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Westwind.AspnetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown
{
    public static class Markdown
    {
        /// <summary>
        /// Renders raw markdown from string to HTML
        /// </summary>
        /// <param name="markdown">The markdown to parse into HTML</param>
        /// <param name="usePragmaLines">print line number ids into the document</param>
        /// <param name="forceReload">forces the markdown parser to be reloaded</param>
        /// <param name="sanitizeHtml">Remove script tags from HTML output</param>
        /// <returns></returns>
        public static string Parse(string markdown, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";

            var parser = MarkdownParserFactory.GetParser(usePragmaLines, forceReload);
            return parser.Parse(markdown,sanitizeHtml);
        }

        /// <summary>
        /// Renders raw Markdown from string to HTML.
        /// </summary>
        /// <param name="markdown">The markdown to parse into HTML</param>
        /// <param name="usePragmaLines">print line number ids into the document</param>
        /// <param name="forceReload">forces the markdown parser to be reloaded</param>
        /// <param name="sanitizeHtml">Remove script tags from HTML output</param>
        /// <returns></returns>
        public static HtmlString ParseHtmlString(string markdown, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = false)
        {
            return new HtmlString(Parse(markdown, usePragmaLines, forceReload, sanitizeHtml));
        }

        /// <summary>
        /// Parses content from a file on disk from Markdown to HTML.
        /// </summary>
        /// <param name="filename">A physical or virtual filename path. If running under System.Web this method uses MapPath to resolve paths.
        /// For non-HttpContext environments this file name needs to be fully qualified.</param>
        /// <param name="usePragmaLines">Generates line numbers as ids into headers and paragraphs. Useful for previewers to match line numbers to rendered output</param>
        /// <param name="forceReload">Forces the parser to reloaded. Otherwise cached instance is used</param>
        /// <param name="sanitizeHtml">Strips out scriptable tags and attributes for prevent XSS attacks. Minimal implementation.</param>
        /// <returns>HTML result as a string</returns>
        public static string ParseFromFile(string markdownFile, bool usePragmaLines = false, bool forceReload = false,
                                           bool sanitizeHtml = false)
        {
            if (string.IsNullOrEmpty(markdownFile))
                return markdownFile;

            var context = MarkdownMiddlewareExtensions.GetHttpContext();
            var filename = HttpRequestExtensions.MapPath(context.Request, markdownFile);

            string markdown = null;

            try
            {
                using (var reader = File.OpenText(filename))
                {
                    markdown = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Couldn't load Markdown file: " + Path.GetFileName(markdownFile), ex);
            }

            var parser = MarkdownParserFactory.GetParser();
            var html = parser.Parse(markdown, sanitizeHtml);

            return html;
        }


        /// <summary>
        /// Parses content from a file on disk from Markdown to HTML.
        /// </summary>
        /// <param name="filename">A physical or virtual filename path. If running under System.Web this method uses MapPath to resolve paths.
        /// For non-HttpContext environments this file name needs to be fully qualified.</param>
        /// <param name="usePragmaLines">Generates line numbers as ids into headers and paragraphs. Useful for previewers to match line numbers to rendered output</param>
        /// <param name="forceReload">Forces the parser to reloaded. Otherwise cached instance is used</param>
        /// <param name="sanitizeHtml">Strips out scriptable tags and attributes for prevent XSS attacks. Minimal implementation.</param>
        /// <returns>HTML result as an HTML string for embedding in Razor views</returns>
        public static HtmlString ParseHtmlStringFromFile(string markdownFile, bool usePragmaLines = false,
            bool forceReload = false,
            bool sanitizeHtml = false)
        {
            return new HtmlString(ParseFromFile(markdownFile, usePragmaLines, forceReload, sanitizeHtml));
        }

        /// <summary>
            /// Parses content from a file on disk from Markdown to HTML.
            /// </summary>
            /// <param name="filename">A physical or virtual filename path. If running under System.Web this method uses MapPath to resolve paths.
            /// For non-HttpContext environments this file name needs to be fully qualified.</param>
            /// <param name="usePragmaLines">Generates line numbers as ids into headers and paragraphs. Useful for previewers to match line numbers to rendered output</param>
            /// <param name="forceReload">Forces the parser to reloaded. Otherwise cached instance is used</param>
            /// <param name="sanitizeHtml">Strips out scriptable tags and attributes for prevent XSS attacks. Minimal implementation.</param>
            /// <returns>HTML result as a string</returns>
            public static async Task<string> ParseHtmlFromFileAsync(string markdownFile, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = false)
        {
            if (string.IsNullOrEmpty(markdownFile))
                return markdownFile;

            var context = MarkdownMiddlewareExtensions.GetHttpContext();
            var filename = HttpRequestExtensions.MapPath(context.Request,markdownFile);

            string content = null;

            try
            {
                using (var reader = File.OpenText(filename))
                {
                    content = await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Couldn't load Markdown file: " + Path.GetFileName(markdownFile), ex);
            }

            return content;
        }


    }
}
