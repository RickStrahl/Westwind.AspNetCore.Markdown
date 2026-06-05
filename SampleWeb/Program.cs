using System.Collections.Generic;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);


MarkdownConfiguration markdownConfiguration = null;

// Add services to the container.
builder.Services.AddMarkdown(config =>
{
    markdownConfiguration = config;

    config.HtmlTagBlackList = "script|iframe|object|embed|form";

    config.MarkdownRenderExtensions.Add(new PlantUmlMarkdownRenderExtension());
    config.MarkdownRenderExtensions.Add(new FontAwesomeRenderExtension());

    config.MarkdownPageMode = MarkdownPageModes.ControllerAndView;
    //config.MarkdownPageMode = MarkdownPageModes.MiddlewareAndStaticHtmlFile;

    var folderConfig = config.AddMarkdownProcessingFolder("/docs/", "~/Pages/__MarkdownPageTemplate.cshtml");
    folderConfig = config.AddMarkdownProcessingFolder("/posts/", "~/Pages/__MarkdownPageTemplate.cshtml");

    folderConfig.SanitizeHtml = false;
    folderConfig.ProcessExtensionlessUrls = true;
    folderConfig.ProcessMdFiles = true;

    folderConfig.PreProcess = (model, controller) =>
    {
        // controller.ViewBag.Model = new MyCustomModel();
    };

    config.ConfigureMarkdigPipeline = builder =>
    {
        builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
            .UsePipeTables()
            .UseGridTables()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .UseAutoLinks()
            .UseAbbreviations()
            .UseYamlFrontMatter()
            .UseEmojiAndSmiley(true)
            .UseListExtras()
            .UseFigures()
            .UseTaskLists()
            .UseCustomContainers()
            //.DisableHtml()
            .UseGenericAttributes();
    };
});


if (markdownConfiguration.MarkdownPageMode == MarkdownPageModes.MiddlewareAndStaticHtmlFile)
{
    // need Razor Pages for samples
    builder.Services.AddRazorPages();
}
else
{
    // Only required for Markdown Template using Razor Views
    builder.Services.AddMvc()
       .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly)
       .AddRazorRuntimeCompilation();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseDefaultFiles(new DefaultFilesOptions()
{
    DefaultFileNames = new List<string> { "index.md", "index.html" }
});


app.UseMarkdown();


app.UseRouting();

app.UseStaticFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    if (markdownConfiguration.MarkdownPageMode == MarkdownPageModes.ControllerAndView)
    {
        // controllers not required in MiddlewareAndStaticHtmlFile mode
        endpoints.MapDefaultControllerRoute();
    }
});

app.Run();




