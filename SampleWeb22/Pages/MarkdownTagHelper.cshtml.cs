using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampleWeb.Pages
{
    public class MarkdownControlModel : PageModel
    {
        public string MarkdownText { get; set; } = 
            @"This is **Markdown Text** that was bound to a `PageModel.MarkdownText` property:

```html
<markdown markdown=""MarkdownText"" />
```
";

        public void OnGet()
        {            
        }
    }
}
