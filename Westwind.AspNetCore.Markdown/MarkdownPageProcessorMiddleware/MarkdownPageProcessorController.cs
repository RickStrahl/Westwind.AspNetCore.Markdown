using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown
{

    /// <summary>
    /// A generic controll implementation for processing Markdown
    /// files directly as HTML content
    /// </summary>    
    public class MarkdownPageProcessorController : Controller
    {
        public MarkdownConfiguration MarkdownProcessorConfig { get; }
        private readonly IHostingEnvironment hostingEnvironment;

        public MarkdownPageProcessorController(IHostingEnvironment hostingEnvironment,
            MarkdownConfiguration config)
        {
            MarkdownProcessorConfig = config;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("markdownprocessor/markdownpage")]		
        public async Task<IActionResult> MarkdownPage()
        {
            var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;

            var basePath = hostingEnvironment.WebRootPath;
            var relativePath = model.RelativePath;
            if (relativePath == null)
                return NotFound();

            if (!System.IO.File.Exists(model.PhysicalPath))
                return NotFound();

            // string markdown = await File.ReadAllTextAsync(pageFile);
            string markdown;
            using (var fs = new FileStream(model.PhysicalPath,
                FileMode.Open,
                FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                markdown = await sr.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(markdown))
                return NotFound();

            // set title, raw markdown, yamlheader and rendered markdown
            ParseMarkdownToModel(markdown, model);

            if (model.FolderConfiguration != null)
            {
                model.FolderConfiguration.PreProcess?.Invoke(model, this);
                return View(model.FolderConfiguration.ViewTemplate, model);
            }

            return View(MarkdownConfiguration.DefaultMarkdownViewTemplate, model);
        }

        private MarkdownModel ParseMarkdownToModel(string markdown, MarkdownModel model = null)
        {
            if (model == null)
                model = new MarkdownModel();

            
            if (model.FolderConfiguration.ExtractTitle)
            {
                var firstLines = StringUtils.GetLines(markdown, 30);
                var firstLinesText = String.Join("\n", firstLines);

                // Assume YAML 
                if (markdown.StartsWith("---"))
                {
                    var yaml = StringUtils.ExtractString(firstLinesText, "---", "---", returnDelimiters: true);
                    if (yaml != null)
                    {
                        model.Title = StringUtils.ExtractString(yaml, "title: ", "\n");
                        model.YamlHeader = yaml.Replace("---", "").Trim();
                    }
                }

                if (model.Title == null)
                {
                    foreach (var line in firstLines.Take(10))
                    {
                        if (line.TrimStart().StartsWith("# "))
                        {
                            model.Title = line.TrimStart(new char[] {' ', '\t', '#'});
                            break;
                        }
                    }
                }
            }

            model.RawMarkdown = markdown;
            model.RenderedMarkdown = Markdown.ParseHtmlString(markdown, sanitizeHtml:model.FolderConfiguration.SanitizeHtml);

            return model;
        }
    }
}
