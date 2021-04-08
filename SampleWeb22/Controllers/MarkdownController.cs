using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;

namespace SampleWeb.Controllers
{
    public class MarkdownController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public MarkdownController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("markdown/markdownpage")]
        public async Task<IActionResult> MarkdownPage()
        {            
            var basePath = hostingEnvironment.WebRootPath;
            var relativePath = HttpContext.Items["MarkdownPath_OriginalPath"] as string;
            if (relativePath == null)
                return NotFound();

            relativePath = NormalizePath(relativePath).Substring(1);
            var pageFile = Path.Combine(basePath,relativePath);            
            if (!System.IO.File.Exists(pageFile))
                return NotFound();

            var markdown = await System.IO.File.ReadAllTextAsync(pageFile);
            if (string.IsNullOrEmpty(markdown))
                return NotFound();

            ViewBag.MarkdownText = Markdown.ParseHtmlString(markdown);
            return View("MarkdownPage");
        }

        /// <summary>
        /// Normalizes a file path to the operating system default
        /// slashes.
        /// </summary>
        /// <param name="path"></param>
        static string NormalizePath(string path)
        {
            //return Path.GetFullPath(path); // this always turns into a full OS path

            if (string.IsNullOrEmpty(path))
                return path;

            char slash = Path.DirectorySeparatorChar;
            path = path.Replace('/', slash).Replace('\\', slash);
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }
    }
}
