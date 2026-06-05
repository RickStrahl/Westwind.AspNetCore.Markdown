using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using SampleWeb.Components;
using Westwind.AspNetCore.Markdown;
using Markdown = Westwind.AspNetCore.Markdown.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMarkdown(config =>
{
    config.HtmlTagBlackList = "script|iframe|object|embed|form";

    config.MarkdownRenderExtensions.Add(new PlantUmlMarkdownRenderExtension());
    config.MarkdownRenderExtensions.Add(new FontAwesomeRenderExtension());

    //config.MarkdownPageMode = MarkdownPageModes.ControllerAndView;
    config.MarkdownPageMode = MarkdownPageModes.MiddlewareAndStaticHtmlFile;

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

builder.Services.AddRazorPages();

//builder.Services.AddMvc()
//    .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly)
//    .AddRazorRuntimeCompilation();

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


//app.Use(async (context, next) =>
//    {
//        var path = context.Request.Path.Value?.ToLower();

//        if (!(path?.StartsWith("/markdownprocessor/markdownpage") ?? false))
//        {
//            await next(context);
//            return;
//        }

//        var services = context.RequestServices;
//        var markdownConfig = services.GetRequiredService<MarkdownConfiguration>() as MarkdownConfiguration;
//        var hostingEnvironment = services.GetRequiredService<IWebHostEnvironment>() as IWebHostEnvironment;
        

//        if (markdownConfig.MarkdownPageMode != MarkdownPageModes.MiddlewareAndStaticHtmlFile)
//        {
//            var endpoints = services.GetRequiredService<EndpointDataSource>() as EndpointDataSource;
//            // check if controller method is mapped - only if controllers are enabled
//            if (endpoints.Endpoints
//                .OfType<RouteEndpoint>()
//                .Any(e => string.Equals(
//                    e.RoutePattern.RawText,
//                    "markdownprocessor/markdownpage",
//                    StringComparison.OrdinalIgnoreCase)))            
//            {
//                await next(context);
//                return;
//            }
//        }
        
//        var model = context.Items["MarkdownProcessor_Model"] as MarkdownModel;
//        if (model == null)
//            throw new InvalidOperationException(
//                "This controller is not accessible directly unless the Markdown Model is set");

//        var basePath = hostingEnvironment.WebRootPath;
//        var relativePath = model.RelativePath;
//        if (relativePath == null)
//        {
//            throw new FileNotFoundException();            
//        }

//        if (!File.Exists(model.PhysicalPath))
//        {
//            throw new FileNotFoundException("");            
//        }

//        // string markdown = await File.ReadAllTextAsync(pageFile);
//        string markdown;
//        using (var fs = new FileStream(model.PhysicalPath,
//                   FileMode.Open,
//                   FileAccess.Read))
//        using (var sr = new StreamReader(fs))
//        {
//            markdown = await sr.ReadToEndAsync();
//        }


//        // set title, raw markdown, yamlheader and rendered markdown
//        MarkdownPageProcessorController.ParseMarkdownToModel(markdown, model);

//        string html = null;

//        var staticTemplatePath = Path.Combine(hostingEnvironment.ContentRootPath, model.FolderConfiguration.StaticHtmlViewTemplate.Replace("~/", ""));
//        if (!File.Exists(staticTemplatePath))
//        {

//            string staticTemplate = null;
//            using (var fs = new FileStream(staticTemplatePath,
//                       FileMode.Open,
//                       FileAccess.Read))
//            using (var sr = new StreamReader(fs))
//            {
//                staticTemplate = await sr.ReadToEndAsync();
//            }
//            html = staticTemplate.Replace("{{ RenderedMarkdown }}", model.RenderedMarkdown.ToString());
//        }
//        else
//        {
//            html = model.RenderedMarkdown?.ToString();
//        }

//        await context.Response.WriteAsync(html);
//    });



app.UseRouting();

app.UseStaticFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapDefaultControllerRoute();
});

app.Run();




