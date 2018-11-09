using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.Tests
{
    class MockSource : ISource, IDisposable
    {
        private Dictionary<Uri, string> _textResourceMap = new Dictionary<Uri, string>();
        private Dictionary<Uri, List<Image>> _imageResourceMap = new Dictionary<Uri, List<Image>>();

        public void Dispose()
        {
            foreach (var imageResource in _imageResourceMap)
            {
                foreach (var image in imageResource.Value)
                    image.Dispose();
            }
        }

        public void AddTextResource(Uri uri, string contents)
        {
            _textResourceMap.Add(uri, contents);
        }

        public void AddImageResource(Uri uri, Size imageSize)
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                bitmap.SetPixel(0, 0, Color.DarkSlateBlue);
                AddImageResource(uri, new Bitmap(bitmap, imageSize));
            }
        }

        public void AddImageResource(Uri uri, Image image)
        {
            if (!_imageResourceMap.ContainsKey(uri))
                _imageResourceMap.Add(uri, new List<Image>());
            _imageResourceMap[uri].Add(image);
        }

        public IEnumerable<Image> DownloadImages(Uri uri)
        {
            if (!_imageResourceMap.ContainsKey(uri))
                return null;
            return _imageResourceMap[uri];
        }

        public StreamReader DownloadText(Uri uri)
        {
            if (!_textResourceMap.ContainsKey(uri))
                return null;
            var contents = _textResourceMap[uri];

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(contents);
            writer.Flush();
            memoryStream.Position = 0;
            return new StreamReader(memoryStream);
        }

    }
}
