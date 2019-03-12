# Westwind.AspNetCore Change Log
<small>[Nuget](https://www.nuget.org/packages/Westwind.AspNetCore.Markdown/) &bull; [Github](https://github.com/RickStrahl/Westwind.AspNetCore.Markdown)</small>

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