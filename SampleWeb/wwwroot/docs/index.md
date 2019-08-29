
# INDEX.MD

Sample Index file that's loaded of a 'default Url' - `/docs/` in this case. Using the DefaultDocuments middleware in ASP.NET Core allows automatically redirecting to a Markdown file when a root URL is accessed and a Markdown file is registered as one of the default files nad the file exists.



## To make this work
You'll need to use the default ASP.NET Core **DefaultDocuments** middleware and ensure it's registered **before** the Markdown middleware in the startup `Configure()` method:

```cs
app.UseDefaultFiles(new DefaultFilesOptions()
{
    DefaultFileNames = new List<string> { "index.md", "index.html" }
});

app.UseMarkdown();
```

Then create `/docs/index.md` and that page will then render when `~/docs/` is accessed.