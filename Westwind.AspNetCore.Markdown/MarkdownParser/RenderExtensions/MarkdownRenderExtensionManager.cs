using System.Collections.Generic;
using Westwind.AspNetCore.Markdown;

namespace Westwind.AspNetCore.Markdown;

/// <summary>
/// Manages RenderExtensions that are executed during Markdown rendering.
/// You can intercept markdown on pre-rendering and html after rendering
/// and modify by implementing custom IRenderExtension implementations
/// and adding to MarkdownRenderExtensionManager.Current.RenderExtensions.
/// </summary>
public class MarkdownRenderExtensionManager
{
    public static MarkdownRenderExtensionManager Current { get; set; } = new MarkdownRenderExtensionManager();

    public List<IMarkdownRenderExtension> RenderExtensions { get; set; }

    public MarkdownRenderExtensionManager()
    {
        RenderExtensions = new List<IMarkdownRenderExtension>() {
            new PlantUmlMarkdownRenderExtension(),            
        };
    }
}
