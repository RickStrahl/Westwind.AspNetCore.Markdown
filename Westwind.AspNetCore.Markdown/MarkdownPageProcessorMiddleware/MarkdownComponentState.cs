using System;

namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Internally held references that made accessible to the static Markdown functions
    /// </summary>
    internal static class MarkdownComponentState
    {
        internal static IServiceProvider ServiceProvider { get; set; }
        internal static MarkdownConfiguration Configuration { get; set; }

        
        //internal static IHttpClientFactory HttpClientFactory
        //{
        //    get
        //    {
        //        if (_httpClientFactory == null)
        //            _httpClientFactory = ServiceProvider.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
        //        return _httpClientFactory;
        //    }
        //}
        //private static IHttpClientFactory _httpClientFactory;
    }
}
