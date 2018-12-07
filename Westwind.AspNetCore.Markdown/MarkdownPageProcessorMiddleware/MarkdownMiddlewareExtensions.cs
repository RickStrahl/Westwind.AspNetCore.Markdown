using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown.Utilities;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// The Middleware Hookup extensions.
    /// </summary>
    public static class MarkdownMiddlewareExtensions
    {
        /// <summary>
        /// Configure the MarkdownPageProcessor in Startup.ConfigureServices.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMarkdown(this IServiceCollection services,
            Action<MarkdownConfiguration> configAction = null)
        {
            var config = new MarkdownConfiguration();

            if (configAction != null)
                configAction.Invoke(config);

            MarkdownComponentState.Configuration = config;

            MarkdownParserBase.HtmlSanitizeTagBlackList = config.HtmlTagBlackList;

            if (config.ConfigureMarkdigPipeline != null)
                MarkdownParserMarkdig.ConfigurePipelineBuilder = config.ConfigureMarkdigPipeline;

            config.MarkdownProcessingFolders =
                config.MarkdownProcessingFolders
                    .OrderBy(f => f.RelativePath)
                    .ToList();

            services.AddSingleton(config);

            // We need access to the HttpContext for Filename resolution
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            return services;
        }


        /// <summary>
        /// Hook up the Markdown Page Processing functionality in the Startup.Configure method
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMarkdown(this IApplicationBuilder builder)
        {
            // cache so we can access it from static helpers
            MarkdownComponentState.ServiceProvider = builder.ApplicationServices;

            return builder.UseMiddleware<MarkdownPageProcessorMiddleware>();
        }

    }
}

