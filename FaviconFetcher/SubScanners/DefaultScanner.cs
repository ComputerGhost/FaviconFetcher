using FaviconFetcher.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if DEBUG
[assembly: InternalsVisibleTo("FaviconFetcher.Tests")]
#endif
namespace FaviconFetcher.SubScanners
{
    class DefaultScanner : SubScanner
    {
        public DefaultScanner(ISource source, Uri uri) : base(source, uri)
        {
        }

        public override void Start()
        {
            using (var reader = Source.DownloadText(TargetUri))
            {
                if (reader != null)
                    _ParsePage(new TextParser(reader));
            }

            // We add the old standard as the lowest priority.
            SuggestedScanners.Add(new FaviconIcoScanner(Source, TargetUri));
        }


        private void _ParsePage(TextParser parser)
        {
            parser.SkipUntil("<html");
            if (parser.SkipUntil("<head", "<body") != "<head")
                return;

            while (!parser.EndOfStream)
            {
                switch (parser.SkipUntil("</head", "<link", "<meta"))
                {
                    case "<link": _ParseLink(parser); break;
                    case "<meta": _ParseMeta(parser); break;
                    default: return;
                }
            }
        }

        private void _ParseLink(TextParser parser)
        {
            var attributes = _ParseAttributes(parser);

            // If there's no href, then there's definitely no icon.
            if (!attributes.ContainsKey("href"))
                return;
            var href = attributes["href"];

            // The relation needs to be an icon.
            if (!attributes.ContainsKey("rel"))
                return;
            if (!attributes["rel"].Contains("icon"))
                return;
            var rel = attributes["rel"];

            // Get the sizes specified
            var sizes = new List<Size>();
            if (attributes.ContainsKey("sizes"))
            {
                foreach (var size in attributes["sizes"].Split(' '))
                {
                    var parts = size.Split('x');
                    if (parts.Length != 2)
                        continue;

                    if (!int.TryParse(parts[0], out int width))
                        continue;
                    if (!int.TryParse(parts[1], out int height))
                        continue;

                    sizes.Add(new Size(width, height));
                }
            }

            // If no valid sizes, try some deduction...
            if (sizes.Count == 0) {
                if (rel.Contains("apple"))
                    sizes.Add(new Size(57, 57));
                else
                {
                    int finalSize = 0, currentSize = 0;
                    foreach (char currentChar in href)
                    {
                        if (char.IsDigit(currentChar))
                            currentSize = currentSize * 10 + (currentChar - '0');
                        else
                            currentSize = 0;
                        if (currentSize > 0)
                            finalSize = currentSize;
                    }
                    if (finalSize > 0)
                        sizes.Add(new Size(finalSize, finalSize));
                }
            }

            // If still no valid size, just use a default 16x16.
            if (sizes.Count == 0)
                sizes.Add(new Size(16, 16));

            // Now we can finally add the favicons, one instance for each size
            foreach (var size in sizes)
            {
                try
                {
                    Results.Add(new ScanResult
                    {
                        ExpectedSize = size,
                        Location = new Uri(TargetUri, href)
                    });
                }
                catch (UriFormatException) { }
            }
        }

        private void _ParseMeta(TextParser parser)
        {
            var attributes = _ParseAttributes(parser);

            // <meta rel="manifest" href="manifest.json">
            if (attributes.ContainsKey("rel") && attributes["rel"] == "manifest")
            {
                if (!attributes.ContainsKey("href"))
                    return;
                try
                {
                    var uri = new Uri(TargetUri, attributes["href"]);
                    SuggestedScanners.Add(new ManifestJsonScanner(Source, uri));
                }
                catch (UriFormatException) { }
            }

            // <meta name="msapplication-config" content="browserconfig.xml">
            else if (attributes.ContainsKey("name") && attributes["name"] == "msapplication-config")
            {
                if (!attributes.ContainsKey("content"))
                    return;
                try
                {
                    var uri = new Uri(TargetUri, attributes["content"]);
                    SuggestedScanners.Add(new BrowserconfigXmlScanner(Source, uri));
                }
                catch (UriFormatException) { }
            }
        }

        private Dictionary<string, string> _ParseAttributes(TextParser parser)
        {
            var keyvalues = new Dictionary<string, string>();
            while (!parser.EndOfStream && parser.Peek() != '>')
            {
                var key = _ParseKey(parser);
                if (key.Length == 0)
                    break;
                keyvalues[key] = _ParseValue(parser);
            }
            return keyvalues;
        }

        private string _ParseKey(TextParser parser)
        {
            var builder = new StringBuilder();
            parser.SkipWhitespace();
            while (!parser.EndOfStream)
            {
                if (parser.Peek() == '>')
                    break;
                if (char.IsWhiteSpace((char)parser.Peek()))
                    break;
                if (parser.Peek() == '=')
                    break;
                builder.Append((char)parser.Read());
            }
            return builder.ToString().ToLower();
        }

        private string _ParseValue(TextParser parser)
        {
            if (parser.Peek() != '=')
                return null;
            parser.Read();

            parser.SkipWhitespace();

            int quoteChar = -1;
            if (parser.Peek() == '\'')
            {
                quoteChar = '\'';
                parser.Read();
            }
            else if (parser.Peek() == '"')
            {
                quoteChar = '"';
                parser.Read();
            }

            var builder = new StringBuilder();
            while (!parser.EndOfStream)
            {
                if (parser.Peek() == quoteChar) {
                    parser.Read();
                    break;
                }
                if (quoteChar == -1 && char.IsWhiteSpace((char)parser.Peek()))
                    break;
                if (parser.Peek() == quoteChar) {
                    parser.Read();
                    break;
                }
                builder.Append((char)parser.Read());
            }
            return builder.ToString();
        }

    }
}
