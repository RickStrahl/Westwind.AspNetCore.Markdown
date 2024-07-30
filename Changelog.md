# Westwind.AspNetCore.Markdown Change Log
<small>[Nuget](https://www.nuget.org/packages/Westwind.AspNetCore.Markdown/) &bull; [Github](https://github.com/RickStrahl/Westwind.AspNetCore.Markdown)</small>


### Change Log

### v3.17

* **Add Support for MarkdownRenderExtensions**  
Adding support for extending the rendering process via external overrides pre-rendering via Markdown text, and post rendering via HTML text. Implemented using the `IMarkdownRenderExtension` interface and added to `RenderExtensionsManager.Current.RenderExtensions`.

* **Add PlantUMLRenderExtension**  
Add a Markdown Render Extension for PlantUML as an example for a Markdown Render Extension.

#### v3.15
* **Add .NET 8.0 target, drop netcoreapp3.1**  
Updated project to add .NET 8.0 native target and update to MarkDig 0.33.

### Version 3.10

* **Add .NET 7.0 Target** 
Package now supports .NET 7.0, .NET 6.0, .NET 3.1. Dropped support for 5.0 since it's out of support and throws NuGet and validation warnings.

* **Update Markdig Dependency**
Update Markdig to `0.30.4`.

### Version 3.8

* **Add .NET 6.0 Target**   
Updated the target frameworks to support .NET 6.0 and leave 5.0 and 3.1 in place as well.

* **Update Markdig 0.28**  
Update Markdig dependency to the latest version.

### Version 3.6

* **Add .NET 5.0 Target removed .NET Core 2.1 Target**   
Updated the target frameworks to support .NET 5.0 and at the same time remove support for netcoreapp2.1. 1 version back compatibilty update schedule if there are feature differences as there are with 2.1. Removed 2.1 specific code.

* **Add ParseFile() overloads for IFileInfo**   
There are now overloads that allow the various `ParseFile` and`ParseFileAsync()` to work with `IFileInfo` objects to allow loading of virtual resources. ([#22](https://github.com/RickStrahl/Westwind.AspNetCore.Markdown/pull/22))

* **Updated to MarkDig 0.24**  
Update to latest version of MarkDig.

#### v3.4

* **Add BasePath support for Markdown Templates in Middleware**  
If you're using external URLs for pulling in Markdown content you may need to reference content relative to a non-local URL. This fix adds a new `FolderConfiguration.BasePath` property and the ability to specify a `basePath: <url>` in the Markdown document's YAML header to embed the basePath into the document. The value is exposed on the `MarkdownModel.BasePath` and can be embedded into the Markdown page layout template.

* **Add highlightjs-badge to Code Snippet Rendering**   
Added optional highlightjs-badge component to allow code-snippets to be easily copied and display the syntax language in a small badge.


### Version 3.3.8

* **Add Theme and SyntaxTheme to MarkdownModel**  
Added optional `RenderTheme` and `SyntaxTheme` properties to the folder configuration as optional values that are then passed into the page model for rendering. The target application can then optionally take advantage of the themes via code configuration or dynamically at runtime.


### Version 3.3.6

* **Fix: Disk based Page Parsing Bug**   
Fix issue with header parsing.

* **Update to MarkDig 1.18.1**  
Updating to the latest version of Markdig

* **Project Targeting Cleanup**  
Clean up project targeting for the library project for .NET Core 2.1 and 3.0.

### Version 3.3.0

* **Add support for .NET Core 3.0**  
Added support for .NET Core 3.0 by re-configuring the projects and conditionally compiling parts of the application. Thanks to Phil Haack for finding and reporting the incompatibility initially.

* **Multi-Targeted for .NET Core 2.1  and 3.0**  
The package now works for both .NET Core 2.1 and .NET Core 3.0. Unfortunately due to some fundamental type changes in .NET Core 3.0 this means the packages no longer can target .NET Standard so the targets now are `netcoreapp2.1` and `netcoreapp3.0`.

### Version 3.2.5

* **Update to latest version of MarkDig**  
Update to MarkDig 1.17.1 due to breaking API changes in earlier versions in some of the the MarkDig processor configuration signatures.

* **Add NoHttpException to Parse Methods and TagHelper**  
Add optional parameter/property to Parse Methods and TagHelper to control how URL retrieved pages are handled if there's no internet connection or the URL isn't available. With NoHttpExceptions set the parser returns null instead of throwing an exception and into the page.

### Version 3.2.4

* **Update Markdig Dependency**  
Update MarkDig version due to breaking change with configuration with the newer version (0.16).

* **Remove HtmlAgilityPack Dependency**  
Removed the HtmlAgilityPack dependency which was only used for the Markdown URL fixup helper. Replaced with manual parsing code to avoid extra dependency for such a small feature.

### Version 3.2.2

* **Add Support for BaseUrlProcessing for Url Links and Images**  
When using the TagHelper with the `url=` option or `Markdown.ParseFromUrl()` you can now optionally specify to fix up relative Markdown links and images using the `url-fixup-baseurl=` and `fixupBaseUrl` property respectively. The default behavior is to fix up paths.

* **Add Support for Title Detection with === underlines**  
Added support for title detection when using a double underscore titles within the first 10 lines of text. Title detection uses Yaml title header first, `#` headers next, and double underscore `===` headers last.

### Version 3.2

* **Add support for replacing Markdown Engine**  
Added support for an `IMarkdownParserFactory` to create a custom `IMarkdownParserFactory` and `IMarkdownParser` implementation.
The factory can be applied in the configuration via `config.MarkdownParserFactory` in `Startup.ConfigureServices()` of the application.

* **Add support for loading Markdown from a URL**  
The Markdown static class and TagHelper now have support for loading Markdown from a URL. `Markdown.ParseFromUrl()` along with async and HtmlStringVersions as well as a `url=` attribute on the TagHelper allow for loading Markdown from a URL.

* **Fix: Trailing Slash Handling for Markdown Extensionless Urls**  
Fixed issue where a trailing slash would not render Markdown document if a matching .md file is found.   
**Note:** while pages with backslashes now render, the resulting page may not render properly due to an invalid base path.  This is not related to the component but to HTML pathing - the same is true for MVC views for example with releative paths. I highly recommend you don't use a trailinig slash for Markdown Urls to preserve the proper basepath in your document or you ensure `~/` paths in your document.

* **Markdown TagHelper Filename Property**  
You can now include Markdown content from the site's file system using the `<markdown>` TagHelper file the `filename` attribute. This makes it easy to break out large text blocks and edit and maintain them easily in your favorite Markdown editor vs. editing inline the HTML content. Note that related resources are **host page relative**, not relative to the Markdown page so plan accordingly for links and image refs.

* **Markdown.ParseFromFile(), ParseFromFileAsync() and .ParseHtmlStringFromFile()**  
Like the TagHelper functions above these static methods allow you to load Markdown from a file within the Web site using virtual path syntax (`~/MarkdownFiles/MarkdownPartial.md`). Note that related resources are **host page relative** rather than markdown page relative.

### Version 3.0.36

* **Add better XSS Support for Markdown Parser**  
There's now optional, configurable handling for removing `<script>`,`<iframe>`,`<form>` etc. tags, `javascript:` directives, and `onXXXX` event handlers on raw HTML elements in the document. For more info see blog post: [Markdown and Cross Site Scripting](https://weblog.west-wind.com/posts/2018/Aug/31/Markdown-and-Cross-Site-Scripting).

### Version 3.0.25

* **Add Markdown Page Handler**  
Added a new page handler that allows dropping of Markdown documents into a folder to be rendered as HTML using a pre-configured HTML template.