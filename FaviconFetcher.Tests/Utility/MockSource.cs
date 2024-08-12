using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                AddImageResource(uri, BitmapIconImage.FromSKBitmap(bitmap, new IconSize(imageSize.Width, imageSize.Height)));
            }
        }

        public void AddImageResource(Uri uri, IconImage image)
        {
            if (!_imageResourceMap.ContainsKey(uri))
                _imageResourceMap.Add(uri, new List<IconImage>());
            _imageResourceMap[uri].Add(image);
        }

        public Task<IEnumerable<IconImage>> DownloadImages(Uri uri, CancellationTokenSource cancelTokenSource)
        {
            ++RequestCount;
            if (!_imageResourceMap.ContainsKey(uri))
                return Task.FromResult(new IconImage[] { } as System.Collections.Generic.IEnumerable<IconImage>);
            return Task.FromResult(_imageResourceMap[uri] as System.Collections.Generic.IEnumerable<IconImage>);
        }

        public Task<StreamReader> DownloadText(Uri uri, CancellationTokenSource cancelTokenSource)
        {
            ++RequestCount;
            if (!_textResourceMap.ContainsKey(uri))
                return Task.FromResult((StreamReader)null);
            var contents = _textResourceMap[uri];

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(contents);
            writer.Flush();
            memoryStream.Position = 0;
            return Task.FromResult(new StreamReader(memoryStream));
        }

    }
}
