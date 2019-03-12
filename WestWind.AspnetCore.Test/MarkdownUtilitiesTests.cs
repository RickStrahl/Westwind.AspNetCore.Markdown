using System;
using NUnit.Framework;
using Westwind.AspNetCore.Markdown;

namespace Tests
{
    [TestFixture]
    public class MarkdownUtilitiesTests
    {

        [Test]
        public void ParseGithubUrl()
        {
            var url = "https://github.com/RickStrahl/MarkdownMonster";

            var fixedupUrl = MarkdownUtilities.ParseMarkdownUrl(url);
            Assert.IsTrue(fixedupUrl.Contains("readme.md", StringComparison.InvariantCultureIgnoreCase));
            Console.WriteLine(fixedupUrl);
        }


        // UI:  https://github.com/RickStrahl/MarkdownMonster/blob/master/Todo.md
        // Raw: https://github.com/RickStrahl/MarkdownMonster/raw/master/Todo.mdslack
        [Test]
        public void ParseGithubUrlAlreadyRaw()
        {
            var url = "https://github.com/RickStrahl/MarkdownMonster/blob/master/Todo.md";
            var fixedupUrl = MarkdownUtilities.ParseMarkdownUrl(url);
            Console.WriteLine(fixedupUrl);            

            // shouldn't be updated as that's already a raw url with different syntax
            Assert.IsTrue(fixedupUrl.Contains("/raw/") || fixedupUrl.Contains("raw.githubusercontent.com"));            
        }

        


        [Test]
        public void ParseMicrosoftDocsUrl()
        {
            var url = "https://docs.microsoft.com/en-us/dotnet/csharp/getting-started/";

            var fixedupUrl = MarkdownUtilities.ParseMarkdownUrl(url);
            Assert.IsTrue(fixedupUrl.Contains("index.md", StringComparison.InvariantCultureIgnoreCase));
            Console.WriteLine(fixedupUrl);
        }

    }
}
