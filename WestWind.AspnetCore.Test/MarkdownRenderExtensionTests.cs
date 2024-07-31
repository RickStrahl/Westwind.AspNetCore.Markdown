using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Westwind.AspNetCore.Markdown;

namespace WestWind.AspnetCore.Test
{
    [TestFixture]
    public class MarkdownRenderExtensionTests
    {
        [SetUp]
        public void Setup()
        {
            MarkdownRenderExtensionManager.Current.AddRenderExtensions(
            [
                new PlantUmlMarkdownRenderExtension(),
                new FontAwesomeRenderExtension()
            ]);
        }


        [Test]
        public void PlantUMLEmbeddingRenderExtensionTest()
        {
            // Ensure that PlantUML Render Extension is added
            // requires [Setup]: MarkdownRenderExtensionManager.Current.AddRenderExtension( new PlantUmlMarkdownRenderExtension() );

            var md =
                """
                # PlantUml Diagrams

                ```plantuml
                @startuml
                skinparam monochrome true
                left to right direction
                User1 --> (Story1)
                (Story1) --> (Story2)
                (Story2) --> (Story3)
                @enduml
                ```
                """;

            var html = Markdown.Parse(md);

            Console.WriteLine(html);

            Assert.That(html.Contains("<img "));

        }
        [Test]
        public void FontAwesomeRenderExtensionTest()
        {
            var md = """
# FontAwesome Icons

> #### @icon-icon-circle Warning: This is a warning message
> Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

@icon-duotone-certificate
""";

            var html = Markdown.Parse(md);

            Console.WriteLine(html);

            Assert.IsTrue(html.Contains("""<i class="fas fa-icon-circle" """));
            Assert.IsTrue(html.Contains("""<i class="fa fa-duotone-certificate">"""));
        }

        [Test]
        public void CustomRenderExtensionTest()
        {
            var ext = new MySampleRenderExtension();

            MarkdownRenderExtensionManager.Current.AddRenderExtension(ext);

            var md =
                """
# Basic Markdown Test

This is **basic** Markdown `code` converted to *HTML*.

* Item 1
* Item 2

Thank you for your time.
""";

            var html = Markdown.Parse(md);

            Console.WriteLine(html);

            var extensions = MarkdownRenderExtensionManager.Current.GetRenderExtensions();
            Assert.IsTrue(extensions.Count() == 2);

            var ext2 = MarkdownRenderExtensionManager.Current["MySampleRenderExtension"];
            Assert.IsNotNull(ext2);

            ext2 = MarkdownRenderExtensionManager.Current[ext];
            Assert.IsNotNull(ext2);

            MarkdownRenderExtensionManager.Current.RemoveRenderExtension(ext);
            Assert.IsTrue(extensions.Count() == 1);
        }

    }

    public class MySampleRenderExtension : IMarkdownRenderExtension
    {
        public string Name { get; set; } = "MySampleRenderExtension";

        public void BeforeMarkdownRendered(ModifyMarkdownArguments args)
        {
            // do something
        }

        public void AfterMarkdownRendered(ModifyHtmlAndHeadersArguments args)
        {
            args.Html += $"\n\n<div>&copy; My Great Sample Company {DateTime.Now.Year}</div>";
        }

        public void AfterDocumentRendered(ModifyHtmlArguments args)
        {
        }
    }
}
