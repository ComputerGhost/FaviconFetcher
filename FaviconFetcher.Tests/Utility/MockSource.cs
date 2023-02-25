using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.Tests.Utility
{
    class MockSource : ISource, IDisposable
    {
        private Dictionary<Uri, string> _textResourceMap = new Dictionary<Uri, string>();
        private Dictionary<Uri, List<IconImage>> _imageResourceMap = new Dictionary<Uri, List<IconImage>>();

        public int RequestCount { get; private set; } = 0;

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

        public void AddImageResource(Uri uri, IconSize imageSize)
        {
            using (var bitmap = new SKBitmap(1, 1))
            {
                bitmap.SetPixel(0, 0, SKColor.FromHsl(255, 255, 255));
                AddImageResource(uri, IconImage.FromSKBitmap(bitmap, new IconSize(imageSize.Width, imageSize.Height)));
            }
        }

        public void AddImageResource(Uri uri, IconImage image)
        {
            if (!_imageResourceMap.ContainsKey(uri))
                _imageResourceMap.Add(uri, new List<IconImage>());
            _imageResourceMap[uri].Add(image);
        }

        public IEnumerable<IconImage> DownloadImages(Uri uri)
        {
            ++RequestCount;
            if (!_imageResourceMap.ContainsKey(uri))
                return new IconImage[] { };
            return _imageResourceMap[uri];
        }

        public StreamReader DownloadText(Uri uri)
        {
            ++RequestCount;
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
