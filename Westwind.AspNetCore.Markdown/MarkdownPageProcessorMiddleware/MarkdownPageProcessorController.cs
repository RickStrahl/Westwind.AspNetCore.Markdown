 using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
 using Markdig.Extensions.Abbreviations;
 using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown.Utilities;


namespace Westwind.AspNetCore.Markdown
{

    /// <summary>
    /// A generic controller implementation for processing Markdown
    /// files directly as HTML content
    /// </summary>
    [ApiExplorerSettings(IgnoreApi=true)]
    public class MarkdownPageProcessorController : Controller
    {
        public MarkdownConfiguration MarkdownProcessorConfig { get; }

        private readonly IWebHostEnvironment hostingEnvironment;
        
        public MarkdownPageProcessorController(
            IWebHostEnvironment hostingEnvironment,
            MarkdownConfiguration config)
        {
            MarkdownProcessorConfig = config;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("markdownprocessor/markdownpage")]		
        public async Task<IActionResult> MarkdownPage()
        {
            // Model saved in middleware processing
            var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;
            if (model == null)
                throw new InvalidOperationException(
                    "This controller is not accessible directly unless the Markdown Model is set");

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

            string yaml = null;
            string firstLinesText = null;
            var firstLines = StringUtils.GetLines(markdown, 50).ToList();

            if (markdown.StartsWith("---"))
            {
                firstLinesText = String.Join("\n", firstLines);
                yaml = StringUtils.ExtractString(firstLinesText, "---", "---", returnDelimiters: true);
            }

            if (yaml != null && yaml.Contains("basePath: "))
                model.BasePath = StringUtils.ExtractString(yaml, "basePath: ", "\n")?.Trim();
            if (string.IsNullOrEmpty(model.BasePath))
                model.BasePath = model.FolderConfiguration.BasePath;

            if (model.FolderConfiguration.ExtractTitle )
            {
                if (yaml != null)
                {
                    model.Title = StringUtils.ExtractString(yaml, "title: ", "\n");
                    model.YamlHeader = yaml.Replace("---", "").Trim();
                }

                // if we don't have Yaml headers the header has to be closer to the top
                firstLines = firstLines.Take(10).ToList();
                
                if (string.IsNullOrEmpty(model.Title))
                {
                    foreach (var line in firstLines)
                    {
                        if (line.TrimStart().StartsWith("# "))
                        {
                            model.Title = line.TrimStart(' ', '\t', '#');
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    for (var index = 0; index < firstLines.Count; index++)
                    {                    
                        var line = firstLines[index];
                        if (line.TrimStart().StartsWith("===") && index > 0)
                        {
                            // grab the previous line
                            model.Title = firstLines[index-1].Trim();
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
