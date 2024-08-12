using FaviconFetcher.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if DEBUG
[assembly: InternalsVisibleTo("FaviconFetcher.Tests")]
#endif
namespace FaviconFetcher.SubScanners
{
    class DefaultScanner : SubScanner
    {
        private Uri _baseUri;

        public DefaultScanner(ISource source, Uri uri) : base(source, uri)
        {
        }

        public async override Task Start(CancellationTokenSource cancelTokenSource = null)
        {
            using (var reader = await Source.DownloadText(TargetUri, cancelTokenSource))
            {
                if (reader != null)
                    _ParsePage(new TextParser(reader));
            }

            // We add the old standard as the lowest priority.
            SuggestedScanners.Add(new FaviconIcoScanner(Source, TargetUri));
        }


        private void _AddResult(IconSize expectedSize, string href)
        {
            try
            {
                Results.Add(new ScanResult
                {
                    ExpectedSize = expectedSize,
                    Location = _GetAbsoluteLocationUri(href)
                });
            }
            catch (UriFormatException) { }
        }

        private Uri _GetAbsoluteLocationUri(string href)
        {
            if (_baseUri == null)
                return new Uri(TargetUri, href);
            else if (_baseUri.IsAbsoluteUri)
                return new Uri(_baseUri, href);
            else
                return new Uri(new Uri(TargetUri, _baseUri), href);
        }


        private void _ParsePage(TextParser parser)
        {
            parser.CaseInsensitiveSkipUntil("<html");
            if (parser.CaseInsensitiveSkipUntil("<head", "<body") != "<head")
                return;

            while (!parser.EndOfStream)
            {
                switch (parser.CaseInsensitiveSkipUntil("<base", "<link", "<meta", "</body"))
                {
                    case "<base": _ParseBase(parser); break;
                    case "<link": _ParseLink(parser); break;
                    case "<meta": _ParseMeta(parser); break;
                    default: return;
                }
            }
        }

        private void _ParseBase(TextParser parser)
        {
            // We need the base href value, which modifies all our urls
            var attributes = _ParseAttributes(parser);
            if (attributes.TryGetValue("href", out var href))
            {
                if (Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out var baseUri))
                    _baseUri = baseUri;
            }

            // Fix existing urls
            for (var i = 0; i != Results.Count; i++)
            {
                var result = Results[i];
                var relativeUri = result.Location.ToString().Substring(TargetUri.ToString().Length);
                result.Location = _GetAbsoluteLocationUri(relativeUri);
                Results[i] = result;
            }

            // Fix existing scanner targets
            for (var i = 0; i != SuggestedScanners.Count; i++)
            {
                var scanner = SuggestedScanners[i];
                var relativeUri = scanner.TargetUri.ToString().Substring(TargetUri.ToString().Length);
                scanner.TargetUri = _GetAbsoluteLocationUri(relativeUri);
                SuggestedScanners[i] = scanner;
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
            var rel = attributes["rel"].ToLower();
            if (!rel.Contains("icon"))
                return;

            // Get the sizes specified
            var sizes = new List<IconSize>();
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

                    sizes.Add(new IconSize(width, height));
                }
            }

            // If SVG, assign a scaleable size
            if (attributes.ContainsKey("type") && attributes["type"].ToLower() == "image/svg+xml")
            {
                sizes.Add(IconSize.Scaleable);
            }

            // If no valid sizes, try some deduction...
            if (sizes.Count == 0) {
                if (rel.Contains("apple"))
                    sizes.Add(new IconSize(57, 57));
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
                        sizes.Add(new IconSize(finalSize, finalSize));
                }
            }

            // If still no valid size, just use a default 16x16.
            if (sizes.Count == 0)
                sizes.Add(new IconSize(16, 16));

            // Now we can finally add the favicons, one instance for each size
            foreach (var size in sizes)
            {
                _AddResult(size, href);
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
                    var uri = _GetAbsoluteLocationUri(attributes["href"]);
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
                    var uri = _GetAbsoluteLocationUri(attributes["content"]);
                    SuggestedScanners.Add(new BrowserconfigXmlScanner(Source, uri));
                }
                catch (UriFormatException) { }
            }
        }

        // Returns dictionary of (attribute.ToLower, value)
        private Dictionary<string, string> _ParseAttributes(TextParser parser)
        {
            var keyvalues = new Dictionary<string, string>();
            while (!parser.EndOfStream && parser.Peek() != '>')
            {
                var key = _ParseKeyLowercased(parser);
                if (key.Length == 0)
                    break;
                keyvalues[key] = _ParseValue(parser);
            }
            return keyvalues;
        }

        // Returns lowercase version of key
        private string _ParseKeyLowercased(TextParser parser)
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

            int quoteChar = _ReadQuoteChar(parser);

            var builder = new StringBuilder();
            while (!parser.EndOfStream)
            {
                if (_IsQuoteChar(parser, quoteChar))
                {
                    parser.Read();
                    break;
                }

                builder.Append((char)parser.Read());
            }
            return builder.ToString();
        }

        private int _ReadQuoteChar(TextParser parser)
        {
            parser.SkipWhitespace();

            switch (parser.Peek())
            {
                case '"': case '\'':
                    return parser.Read();
                default:
                    return -1;
            }
        }

        private bool _IsQuoteChar(TextParser parser, int quoteChar)
        {
            var nextChar = parser.Peek();
            return
                (nextChar == quoteChar) ||
                (quoteChar == -1 && char.IsWhiteSpace((char)nextChar)) ||
                (quoteChar == -1 && nextChar == '>')
            ;
        }

    }
}
