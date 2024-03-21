using FaviconFetcher.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#if DEBUG
[assembly: InternalsVisibleTo("FaviconFetcher.Tests")]
#endif
namespace FaviconFetcher.SubScanners
{
    class BrowserconfigXmlScanner : SubScanner
    {
        public BrowserconfigXmlScanner(ISource source, Uri uri) : base(source, uri)
        {
        }

        public async override Task Start(CancellationTokenSource cancelTokenSource = null)
        {
            using (var reader = await Source.DownloadText(TargetUri, cancelTokenSource))
            {
                if (reader != null)
                    _ParseContent(new TextParser(reader));
            }
        }


        [DataContract]
        class IconEntry :IXmlSerializable
        {
            public string src { get; set; }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                src = reader.GetAttribute("src");
            }

            public void WriteXml(XmlWriter writer)
            {
                throw new NotImplementedException();
            }
        }


        private void _ParseContent(TextParser parser)
        {
            parser.SkipUntil("<tile");

            while (!parser.EndOfStream)
            {
                var found = parser.SkipUntil(
                    "<square70x70logo", "<square150x150logo",
                    "<wide310x150logo", "<square310x310logo");
                var attributes = _ParseAttributes(parser);
                if (!attributes.ContainsKey("src"))
                    continue;

                switch (found)
                {
                    case "<square70x70logo":
                        _AddResult(attributes["src"], new IconSize(70, 70));
                        break;
                    case "<square150x150logo":
                        _AddResult(attributes["src"], new IconSize(150, 150));
                        break;
                    case "<wide310x150logo":
                        _AddResult(attributes["src"], new IconSize(310, 150));
                        break;
                    case "<square310x310logo":
                        _AddResult(attributes["src"], new IconSize(310, 310));
                        break;
                }
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
                if (parser.Peek() == quoteChar)
                {
                    parser.Read();
                    break;
                }
                if (quoteChar == -1 && char.IsWhiteSpace((char)parser.Peek()))
                    break;
                if (parser.Peek() == quoteChar)
                {
                    parser.Read();
                    break;
                }
                builder.Append((char)parser.Read());
            }
            return builder.ToString();
        }

        private void _AddResult(string uri, IconSize size)
        {
            try {
                Results.Add(new ScanResult
                {
                    ExpectedSize = size,
                    Location = new Uri(TargetUri, uri)
                });
            }
            catch (UriFormatException) { }
        }
    }
}
