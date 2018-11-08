using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.Tests
{
    class MockSource : ISource
    {
        private Dictionary<Uri, string> _resourceMap = new Dictionary<Uri, string>();

        public void AddTextResource(Uri uri, string contents)
        {
            _resourceMap.Add(uri, contents);
        }

        public IEnumerable<Image> DownloadImages(Uri uri)
        {
            throw new NotImplementedException();
        }

        public StreamReader DownloadText(Uri uri)
        {
            if (!_resourceMap.ContainsKey(uri))
                return null;
            var contents = _resourceMap[uri];

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(contents);
            writer.Flush();
            memoryStream.Position = 0;
            return new StreamReader(memoryStream);
        }
    }
}
