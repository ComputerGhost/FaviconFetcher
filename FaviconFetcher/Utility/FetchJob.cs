using ConcurrentPriorityQueue;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.Utility
{
    // A fetch job. Cannot be reused.
    // We do this to make disposing of Images easier.
    class FetchJob : IDisposable
    {
        // The source being used for downloading resources.
        public ISource Source { get; private set; }

        // The URI being fetched
        public Uri TargetUri;

        // Options for the fetch
        FetchOptions Options;

        ConcurrentPriorityQueue<Image, double> downloadedImages = new ConcurrentPriorityQueue<Image, double>();
        ConcurrentPriorityQueue<ScanResult, double> notVerified = new ConcurrentPriorityQueue<ScanResult, double>();

        public FetchJob(ISource source, Uri uri, FetchOptions options)
        {
            Source = source;
            TargetUri = uri;
            Options = options;
        }

        public void Dispose()
        {
            foreach (var downloadedImage in downloadedImages)
                downloadedImage.Dispose();
        }

        // Scan and fetches best icon per Options.
        public Image ScanAndFetch()
        {
            var parsedUris = new HashSet<Uri>();
            foreach (var possibleIcon in new Scanner(Source).Scan(TargetUri))
            {
                // Because the scanner can return duplicate URIs.
                if (parsedUris.Contains(possibleIcon.Location))
                    continue;
                parsedUris.Add(possibleIcon.Location);

                // Hopefully we've already found it
                if (_IsPerfect(possibleIcon.ExpectedSize))
                {
                    var image = DownloadImages_ReturnPerfect(possibleIcon.Location);
                    if (image != null)
                        return image;
                }

                // If not, we'll look at it later.
                else
                {
                    notVerified.Enqueue(possibleIcon, -1 * _GetDistance(possibleIcon.ExpectedSize));
                }
            }

            // Download them, prioritizing those closest to perfect
            foreach (var possibleIcon in notVerified)
            {
                var image = DownloadImages_ReturnPerfect(possibleIcon.Location);
                if (image != null)
                    return image;
            }

            // Since none were a perfect match, just return the closest
            if (downloadedImages.Count == 0)
                return null;
            return downloadedImages.Dequeue();
        }


        // Downloads images. If perfect found, returns it.
        private Image DownloadImages_ReturnPerfect(Uri uri)
        {
            foreach (var image in Source.DownloadImages(uri))
            {
                if (_IsPerfect(image.Size))
                    return image;
                if (!_IsInRange(image.Size))
                {
                    image.Dispose();
                    continue;
                }
                downloadedImages.Enqueue(image, -1 * _GetDistance(image.Size));
            }
            return null;
        }

        // Gets the distance between a size and the perfect size
        private double _GetDistance(Size size)
        {
            // Considering the sizes as points, we can find the distance 
            // between them by using the Pythagorean Theorem.
            var dx = size.Width - Options.PerfectSize.Width;
            var dy = size.Height - Options.PerfectSize.Height;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Is a size within the min/max range?
        private bool _IsInRange(Size size)
        {
            if (Options.RequireSquare && size.Width != size.Height)
                return false;
            if (size.Width < Options.MinimumSize.Width)
                return false;
            if (size.Width > Options.MaximumSize.Width)
                return false;
            if (size.Height < Options.MinimumSize.Height)
                return false;
            if (size.Height > Options.MaximumSize.Height)
                return false;
            return true;
        }

        // Is a size a perfect match?
        private bool _IsPerfect(Size size)
        {
            if (Options.PerfectSize == Size.Empty)
                return false;
            return size == Options.PerfectSize;
        }

    }
}
