using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.Tables;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddMvc();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");            
            }            

            app.UseMarkdown();
            
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


            app.UseStaticFiles();
            app.UseMvc();


        }
    }
}
