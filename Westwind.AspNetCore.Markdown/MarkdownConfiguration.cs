﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Markdig;

namespace Westwind.AspNetCore.Markdown
{

    /// <summary>
    /// Holds configuration information about the MarkdownPageProcessor
    /// </summary>
    public class MarkdownConfiguration
    {
        public const string DefaultMarkdownViewTemplate = "~/Views/__MarkdownPageTemplate.cshtml";

        /// <summary>
        /// List of relative virtual folders where any extensionless URL is 
        /// matched to an .md file on disk
        /// </summary>
        public List<MarkdownProcessingFolder> MarkdownProcessingFolders { get; set; } = new List<MarkdownProcessingFolder>();

        public IMarkdownParserFactory MarkdownParserFactory { get; set; } = new MarkdigMarkdownParserFactory();

        /// <summary>
        /// Optional global configuration for setting up the Markdig Pipeline
        /// </summary>
        public Action<MarkdownPipelineBuilder> ConfigureMarkdigPipeline { get; set; }

        /// <summary>
        /// Global HtmlTagBlackList when StripScriptTags is set for Markdown parsing     
        /// </summary>
        public string HtmlTagBlackList { get; set; } = "script|iframe|object|embed|form";

        /// <summary>
        /// Adds a folder to the list of folders that are to be 
        /// processed by this middleware. 
        /// </summary>
        /// <param name="path">The path to work on. Examples: /docs/ or /classes/docs/.</param>
        /// <param name="viewTemplate">Path to a View Template. Defaults to: ~/Views/__MarkdownPageTemplate.cshtml</param>
        /// <param name="processMdFiles">Process files with an .md extension</param>
        /// <param name="processExtensionlessUrls">Process extensionless Urls as Markdown. Assume matching .md file is available that holds the actual Markdown text</param>
        /// <returns></returns>
        public MarkdownProcessingFolder AddMarkdownProcessingFolder(string path,
                                                                    string viewTemplate = null,
                                                                    bool processMdFiles = true,
                                                                    bool processExtensionlessUrls = true)
        {

            if (!path.StartsWith("/"))
                path = "/" + path;
            if (!path.EndsWith("/"))
                path = "/" + path + "/";

            var folder = new MarkdownProcessingFolder()
            {
                RelativePath = path,
                ProcessMdFiles = processMdFiles,
                ProcessExtensionlessUrls = processExtensionlessUrls
            };

            if (!string.IsNullOrEmpty(viewTemplate))
                folder.ViewTemplate = viewTemplate;

            MarkdownProcessingFolders.Add(folder);

            return folder;
        }

        /// <summary>
        /// Adds a url masked folder to the list of folders that are to be 
        /// processed by this middleware. 
        /// </summary>
        /// <param name="path">The path to work on. Examples: /docs/ or /classes/docs/.</param>
        /// <param name="viewTemplate">Path to a View Template. Defaults to: ~/Views/__MarkdownPageTemplate.cshtml</param>
        /// <param name="processMdFiles">Process files with an .md extension</param>
        /// <param name="processExtensionlessUrls">Process extensionless Urls as Markdown. Assume matching .md file is available that holds the actual Markdown text</param>
        /// <returns></returns>
        public MarkdownProcessingFolder AddMarkdownMaskedProcessingFolder(string urlMask,
                                                                    string filePath,
                                                                    string viewTemplate = null,
                                                                    bool processMdFiles = true,
                                                                    bool processExtensionlessUrls = true)
        {

            if (!filePath.StartsWith("/"))
                filePath = "/" + filePath;
            if (!filePath.EndsWith("/"))
                filePath = "/" + filePath + "/";
            if (!urlMask.StartsWith("/"))
                urlMask = "/" + urlMask;
            if (!urlMask.EndsWith("/"))
                urlMask = "/" + urlMask + "/";

            var folder = new MarkdownProcessingFolder()
            {
                RelativePath = filePath,
                ProcessMdFiles = processMdFiles,
                ProcessExtensionlessUrls = processExtensionlessUrls,
                UrlPathMask = urlMask
            };

            if (!string.IsNullOrEmpty(viewTemplate))
                folder.ViewTemplate = viewTemplate;

            MarkdownProcessingFolders.Add(folder);

            return folder;
        }
    }

    /// <summary>
    /// Configures an individual Folder that is to be processed
    /// by the Markdown Page Processor
    /// </summary>
    public class MarkdownProcessingFolder
    {
        /// <summary>
        /// Site Relative path where this folder processes markdown files.
        /// Can use ~ virtual path syntax.
        /// </summary>
        public string RelativePath { get; set; }


        /// <summary>
        /// If used, provides masking between url path and RelativePath.
        /// Can use ~ virtual path syntax.
        /// </summary>
        public string UrlPathMask { get; set; }


        /// <summary>
        /// An optional Base Path that can be set on the model to embed in a
        /// Markdown page template.
        ///
        /// This can be useful if you are retrieving content from another site,
        /// and you need to locate depedent resources from the same location as
        /// the source file.
        ///
        /// You can also specify `basePath: <path>` in the Yaml header of the
        /// the rendered Markdown document. 
        /// </summary>
        public string BasePath {get; set;}

        /// <summary>
        /// View Template to use to render the Markdown page
        /// </summary>
        public string ViewTemplate { get; set; } = MarkdownConfiguration.DefaultMarkdownViewTemplate;

        /// <summary>
        /// If true processes files with .md extension
        /// </summary>
        public bool ProcessMdFiles { get; set; } = true;

        /// <summary>
        /// If true processes extensionless Urls in the the folder hierarchy as Markdown
        /// and expects a matching .md file
        /// </summary>
        public bool ProcessExtensionlessUrls { get; set; } = true;

        /// <summary>
        /// Determines whether pages try to find the Title inside of the Markdown text
        /// either from YAML content or from first # HEADER tag.
        /// </summary>
        public bool ExtractTitle { get; set; } = true;

        /// <summary>
        /// Removes script tags and javascript directives
        /// from generated HTML content
        /// </summary>
        public bool SanitizeHtml { get; set; } = false;

        //public IMarkdownParserFactory MarkdownParserFactory { get; set; }

        /// <summary>
        /// Optional theme - set during middleware initialization
        /// </summary>
        public string RenderTheme {get; set;}


        /// <summary>
        /// Optional syntaxTheme - set during middleware initialization
        /// </summary>
        public string SyntaxTheme {get; set;} 

        
        /// <summary>
        /// Function that can be set to be called before the Markdown View is fired.
        /// Use this method to potentially add additional data into the ViewBag you 
        /// might want access to in the 
        /// 
        /// </summary>
        public Action<MarkdownModel, Controller> PreProcess { get; set; }
    }
}
