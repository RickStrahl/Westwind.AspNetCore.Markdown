# Westwind.AspNetCore Change Log
<small>[Nuget](https://www.nuget.org/packages/Westwind.AspNetCore/) &bull; [Github](https://github.com/RickStrahl/Westwind.AspNetCore)</small>

### Version 3.0.50

* **Markdown TagHelper Filename Property**  
You can now include Markdown content from the site's file system using the `<markdown>` TagHelper. This makes it easy to break out large text blocks and edit and maintain them easily in your favorite Markdown editor vs. editing inline the HTML content.

### Version 3.0.36

* **Add better XSS Support for Markdown Parser**  
There's now optional, configurable handling for removing `<script>`,`<iframe>`,`<form>` etc. tags, `javascript:` directives, and `onXXXX` event handlers on raw HTML elements in the document. For more info see blog post: [Markdown and Cross Site Scripting](https://weblog.west-wind.com/posts/2018/Aug/31/Markdown-and-Cross-Site-Scripting).

* **WebUtils.JsonString(), WebUtils.JsonDate()**  
Added JSON helpers that facilitate encoding and decoding strings and dates to JSON without having to force use of a full JSON parser. Useful for emedding JSON values into script content.

### Version 3.0.25

* **Add Markdown Page Handler**  
Added a new page handler that allows dropping of Markdown documents into a folder to be rendered as HTML using a pre-configured HTML template.


### Version 3.0.15

* **Add Markdown TagHelper**  
Added a `<markdown />` TagHelper which allows embedding of static Markdown content into a page which is parsed into HTML at runtime. Also includes a Markdown Parser using `Markdown.Parse()` and `Markdown.ParseHtmlString()`. Uses the MarkDig Markdown Parser. Markdown features live in a separate NuGet package `Westwind.AspNetCore.Markdown`.

* **Add Markdown Parser**  
You can use the `Markdown.Parse()` and `Markdown.ParseHtmlString()` methods to render Markdown to HTML in code and Razor pages respectively.

* **AppUser Updates**   
Add additional functions to help with ClaimsPrincipal retrieval.


* **ErrorDisplay Tag Helper Updates**  
Fix Font-Awesome icon display for warning and errors. Fix `UnhandledApiExceptionFilter` bug with invalid declaration.