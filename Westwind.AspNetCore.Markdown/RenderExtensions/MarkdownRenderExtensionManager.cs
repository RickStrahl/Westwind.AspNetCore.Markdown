using System.Collections.Generic;

namespace Westwind.AspNetCore.Markdown;

/// <summary>
/// Manages RenderExtensions that are executed during Markdown rendering.
/// You can intercept markdown on pre-rendering and html after rendering
/// and modify by implementing custom IRenderExtension implementations
/// and adding to MarkdownRenderExtensionManager.Current.RenderExtensions.
/// </summary>
public class MarkdownRenderExtensionManager
{
    public static MarkdownRenderExtensionManager Current { get; set; } = new();

    public List<IMarkdownRenderExtension> RenderExtensions { get; set; } = new();    
}
