namespace Westwind.AspNetCore.Markdown;

public interface IMarkdownParser
{
    /// <summary>
    /// Render Extension Manager that can be used to add custom renderers
    /// </summary>
    MarkdownRenderExtensionManager RenderExtensionManager { get; set; }

    /// <summary>
    /// Returns parsed markdown
    /// </summary>
    /// <param name="markdown"></param>
    /// <returns></returns>
    string Parse(string markdown, bool stripScriptTags = true);
}
