using System;
using Microsoft.AspNetCore.Html;
using NUnit.Framework;
using Westwind.AspNetCore.Markdown;

namespace Tests
{
    public class MarkdownTests
    {

        [Test]
        public void BasicMarkdown()
        {
            string html = Markdown.Parse("This is **basic** Markdown `code` converted to *HTML*.");
            Assert.That(html.Contains("<strong>") && html.Contains("<em>") && html.Contains("<code>"));
        }


        [Test]
        public void BasicMarkdownHtmlString()
        {
            HtmlString htmlString = Markdown.ParseHtmlString("This is **basic** Markdown `code` converted to *HTML*.");
            var html = htmlString.ToString();
            Assert.That(html.Contains("<strong>") && html.Contains("<em>") && html.Contains("<code>"));
        }

        [Test]
        public void BasicMarkdownFromUrl()
        {
            var html = Markdown.ParseFromUrl(                
                "https://github.com/RickStrahl/BlogPosts/raw/master/2018-11/Updating-Westwind.AspNetCore.Markdown-with-Markdown-from-Files-and-URLs/UpdatingWestwindAspnetcoreMarkdownWithMarkdownFromFilesAndUrls.md",
                fixupBaseUrl: true);

            Westwind.Utilities.ShellUtils.ShowHtml(html);


        }
    }
}
