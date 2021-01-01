using System.Collections.Generic;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Westwind.AspNetCore.Markdown;

namespace SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMarkdown(config =>
            {
                // optional Tag BlackList
                config.HtmlTagBlackList = "script|iframe|object|embed|form"; // default

                // Simplest: Use all default settings
                var folderConfig = config.AddMarkdownProcessingFolder("/docs/", "~/Pages/__MarkdownPageTemplate.cshtml");

                // Simple example with url masked folders
                folderConfig = config.AddMarkdownMaskedProcessingFolder("/HelloWorld/Superman/","/docs/", "~/Pages/__MarkdownPageTemplate.cshtml");

                // Customized Configuration: Set FolderConfiguration options
                folderConfig = config.AddMarkdownProcessingFolder("/posts/", "~/Pages/__MarkdownPageTemplate.cshtml");

                // Optionally strip script/iframe/form/object/embed tags ++
                folderConfig.SanitizeHtml = false;  //  default
                
                // Optional configuration settings
                folderConfig.ProcessExtensionlessUrls = true;  // default
                folderConfig.ProcessMdFiles = true; // default

                // Optional pre-processing - with filled model
                folderConfig.PreProcess = (model, controller) =>
                {
                    // controller.ViewBag.Model = new MyCustomModel();
                };

                // folderConfig.BasePath = "https://github.com/RickStrahl/Westwind.AspNetCore.Markdow/raw/master";

                // Create your own IMarkdownParserFactory and IMarkdownParser implementation
                // to replace the default Markdown Processing
                //config.MarkdownParserFactory = new CustomMarkdownParserFactory();                 

                // optional custom MarkdigPipeline (using MarkDig; for extension methods)
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UsePipeTables()
                        .UseGridTables()
                        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                        .UseAutoLinks() // URLs are parsed into anchors
                        .UseAbbreviations()
                        .UseYamlFrontMatter()
                        .UseEmojiAndSmiley(true)
                        .UseListExtras()
                        .UseFigures()
                        .UseTaskLists()
                        .UseCustomContainers()
                        //.DisableHtml()   // renders HTML tags as text including script
                        .UseGenericAttributes();
                };
            });

            // We need to use MVC so we can use a Razor Configuration Template
            // for the Markdown Processing Middleware
            services.AddMvc()
                // have to let MVC know we have a controller otherwise it won't be found
                .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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




            // Ultra-simplistic Markdown router
            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path.Value.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
            //    {

            //        context.Items["MarkdownPath_OriginalPath"] = context.Request.Path.Value;
            //        // rewrite path to our controller so we can use _layout page
            //        context.Request.Path = "/markdown/markdownpage";
            //    }

            //    await next();
            //});


            app.UseMarkdown();

            app.UseRouting();

            app.UseStaticFiles();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();

            });


        }
    }
}
