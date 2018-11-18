namespace Westwind.AspNetCore.Markdown
{
    /// <summary>
    /// Retrieves an instance of a markdown parser
    /// </summary>
    public static class MarkdownParserFactory
    {
        public static string DefaultMarkdownParserName { get;  } = "MarkDig";

        /// <summary>
        /// Use a cached instance of the Markdown Parser to keep alive
        /// </summary>
        public static IMarkdownParser CurrentParser;


        /// <summary>
        /// Retrieves a cached instance of the markdown parser
        /// </summary>                
        /// <param name="forceLoad">Forces the parser to be reloaded - otherwise previously loaded instance is used</param>
        /// <param name="usePragmaLines">If true adds pragma line ids into the document that the editor can sync to</param>
        /// <param name="parserAddinId">optional addin id that checks for a registered Markdown parser</param>
        /// <returns>Mardown Parser Interface</returns>
        public static IMarkdownParser GetParser(bool usePragmaLines = false,
                                                bool forceLoad = false)                                                
        {
            if (!forceLoad && CurrentParser != null)
                return CurrentParser;
            
            CurrentParser = new MarkdownParserMarkdig(usePragmaLines, forceLoad);
            return CurrentParser;
        }
        
        
    }

    
}
