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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Middleware that allows you to serve static Markdown files from disk
    /// and merge them using a configurable View template.
    /// </summary>
    public class MarkdownPageProcessorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MarkdownConfiguration _configuration;

#if NETCOREAPP2_1
        private readonly IHostingEnvironment _env;
#else
        private readonly IWebHostEnvironment _env;
#endif

        public MarkdownPageProcessorMiddleware(RequestDelegate next,
            MarkdownConfiguration configuration,
#if NETCOREAPP2_1
            IHostingEnvironment _env
#else
          IWebHostEnvironment _env
#endif
        )
        {
            _next = next;
            _configuration = configuration;
            this._env = _env;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (string.IsNullOrEmpty(path))
                return _next(context);

            bool hasExtension = !string.IsNullOrEmpty(Path.GetExtension(path));
            bool hasMdExtension = path.EndsWith(".md");
            bool isRoot = path == "/";
            bool processAsMarkdown = false;

            var basePath = _env.WebRootPath;
            var relativePath = path;
            relativePath = PathHelper.NormalizePath(relativePath).Substring(1).TrimEnd('\\', '/');
            var pageFile = Path.Combine(basePath, relativePath);

            // process any Markdown file that has .md extension explicitly
            foreach (var folder in _configuration.MarkdownProcessingFolders)
            {
                if (!path.StartsWith(folder.RelativePath, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (isRoot && folder.RelativePath != "/")
                    continue;

                if (context.Request.Path.Value.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
                {
                    processAsMarkdown = true;
                }
                else if (path.StartsWith(folder.RelativePath, StringComparison.InvariantCultureIgnoreCase) &&
                         (folder.ProcessExtensionlessUrls && !hasExtension ||
                          hasMdExtension && folder.ProcessMdFiles))
                {

                    // it's a physical directory - don't convert that - only virtual files
                    if (!hasExtension && Directory.Exists(pageFile))
                        continue;

                    if (!hasExtension)
                    {
                        pageFile += ".md";
                    }

                    if (!File.Exists(pageFile))
                        continue;

                    processAsMarkdown = true;
                }

                if (processAsMarkdown)
                {
                    var model = new MarkdownModel
                    {
                        FolderConfiguration = folder,
                        RelativePath = path,
                        PhysicalPath = pageFile
                    };

                    // push the model into the context for controller to pick up
                    context.Items["MarkdownProcessor_Model"] = model;

                    // rewrite path to our controller so we can use _layout page
                    context.Request.Path = "/markdownprocessor/markdownpage";
                    break;
                }
            }

            return _next(context);
        }
    }
}
