namespace Westwind.AspNetCore.Markdown;

public interface IMarkdownParserFactory
{        
    IMarkdownParser GetParser(bool usePragmaLines = false, bool forceLoad = false);
}
