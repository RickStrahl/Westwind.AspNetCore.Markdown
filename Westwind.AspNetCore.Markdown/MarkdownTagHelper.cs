#region License
/**************************************************************
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
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using Westwind.AspnetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown
{

    /// <summary>
    /// &lt;markdown&gt; TagHelper that lets you embed Markdown text directly
    /// into your Razor page. Razor expressions are evaluated **before** 
    /// Markdown is parsed.
    /// </summary>        
    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContext;

        /// <summary>
        /// When set to true (default) strips leading white space based
        /// on the first line of non-empty content. The first line of
        /// content determines the format of the white spacing and removes
        /// it from all other lines.
        /// </summary>
        [HtmlAttributeName("normalize-whitespace")]
        public bool NormalizeWhitespace { get; set; } = true;


        /// <summary>
        /// Scripts `script` tags and `javascript:` directive
        /// from the generated HTML content
        /// </summary>
        [HtmlAttributeName("sanitize-html")]
        public bool SanitizeHtml { get; set; } = true;


        /// <summary>
        /// Optional Content property that allows you to bind a 
        /// Markdown model expression to the content. 
        /// 
        /// This Markdown content takes priority over the 
        /// body content of the control.
        /// </summary>
        [HtmlAttributeName("markdown")]
        public ModelExpression Markdown { get; set; }


        /// <summary>
        /// Optional file to load content. Use       
        /// </summary>
        [HtmlAttributeName("Filename")]
        public string Filename { get; set; }


        

        public MarkdownTagHelper(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }


        /// <summary>
        /// Process markdown and generate HTML output
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            string content = null;
            if (!string.IsNullOrEmpty(Filename))
            {
                try
                {
                    var filename  = HttpRequestExtensions.MapPath(_httpContext.HttpContext.Request, Filename);

                    using (var reader = File.OpenText(filename))
                    {
                        content = await reader.ReadToEndAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new FileLoadException("Couldn't load Markdown file: " + Path.GetFileName(Filename), ex);
                }

                if (string.IsNullOrEmpty(content))
                    return;
            }
            else
            {

                if (Markdown != null)
                    content = Markdown.Model?.ToString();

                if (content == null)
                    content = (await output.GetChildContentAsync(NullHtmlEncoder.Default))
                        .GetContent(NullHtmlEncoder.Default);

                if (string.IsNullOrEmpty(content))
                    return;

                content = content.Trim('\n', '\r');
            }
        
            string markdown = NormalizeWhiteSpaceText(content);            

            var parser = MarkdownParserFactory.GetParser();
            var html = parser.Parse(markdown,SanitizeHtml);

            output.TagName = null;  // Remove the <markdown> element
            output.Content.SetHtmlContent(html);
        }

        string NormalizeWhiteSpaceText(string text)
        {
            if (!NormalizeWhitespace || string.IsNullOrEmpty(text))
                return text;

            var lines = GetLines(text);
            if (lines.Length < 1)
                return text;

            string line1 = null;

            // find first non-empty line
            for (int i = 0; i < lines.Length; i++)
            {
                line1 = lines[i];
                if (!string.IsNullOrEmpty(line1))
                    break;
            }

            if (string.IsNullOrEmpty(line1))
                return text;

            string trimLine = line1.TrimStart();
            int whitespaceCount = line1.Length - trimLine.Length;
            if (whitespaceCount == 0)
                return text;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > whitespaceCount)
                    sb.AppendLine(lines[i].Substring(whitespaceCount));
                else
                    sb.AppendLine(lines[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        static string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null)
                return null;

            s = s.Replace("\r\n", "\n");

            if (maxLines < 1)
                return s.Split(new char[] { '\n' });

            return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }
    }
}
