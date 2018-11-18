
### Markdown loaded from File into TagHelper
This text has been loaded into the tag helper via the `Filename` attribute. You can use virtual path mappings like `~/Folder/MarkdownToEmbed.md`, which is great for embedding large blocks of content without having to edit it directly inside of the taghelper's content, but rather use a [proper Markdown editor](https://markdownmonster.west-wind.com).

> **Note:** Relative content inside of the Markdown file will be relative to the **host page** not relative to the Markdown file if it's in a different location. Ideally you'll place the embedded markdown content in the same folder as the host page to ensure content base paths are the same

#### Examples:

```html
<markdown Filename="~/EmbeddedMarkdownContent.md"></markdown>

<markdown Filename="../EmbeddedMarkdownContent2.md"></markdown>

<markdown Filename="EmbeddedMarkdownContent.md"></markdown>
```
