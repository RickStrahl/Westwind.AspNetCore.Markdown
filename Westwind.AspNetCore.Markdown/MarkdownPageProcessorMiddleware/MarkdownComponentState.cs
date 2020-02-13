using System;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Internally held references that made accessible to the static Markdown functions
    /// </summary>
    internal static class MarkdownComponentState
    {
        internal static IServiceProvider ServiceProvider { get; set; }
        internal static MarkdownConfiguration Configuration { get; set; } = new MarkdownConfiguration();


        /// <summary>
        /// Retrieves an HTTP Context instance
        /// </summary>
        /// <returns></returns>
        internal static HttpContext GetHttpContext()
        {
            IHttpContextAccessor contextAccessor;
            try
            {
                if (ServiceProvider == null)
                    throw new InvalidOperationException("ServiceProvider is not available. Please add `app.ConfigureMarkdown()` in `Startup.ConfigureServices()`.");

                contextAccessor = ServiceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            }
            catch 
            {
                throw new InvalidOperationException("IHttpContextAccessor is not available. Please add `app.ConfigureMarkdown()` in `Startup.ConfigureServices()`.");
            }
            return contextAccessor.HttpContext;
        }
    }
}
