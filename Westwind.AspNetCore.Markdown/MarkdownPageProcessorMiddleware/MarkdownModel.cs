using Microsoft.AspNetCore.Html;

namespace Westwind.AspNetCore.Markdown
{
    public class MarkdownModel
    {
        /// <summary>
        /// Title extracted from the document. Title source is
        /// either a YAML title attribute or the first # element
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// HTML output from the rendered markdown text
        /// </summary>
        public HtmlString RenderedMarkdown { get; set; }

        /// <summary>
        /// The raw Markdown Document text. May include a Yaml header
        /// </summary>
        public string RawMarkdown { get; set; }

        /// <summary>
        /// If there is a Yaml header on the document it's set here
        /// </summary>
        public string YamlHeader { get; set; }

        /// <summary>
        /// Relative Path of the Markdown File served
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Relative Path of the Markdown File served
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        /// Folder configuration 
        /// </summary>
        public MarkdownProcessingFolder FolderConfiguration { get; set; }
    }
}
