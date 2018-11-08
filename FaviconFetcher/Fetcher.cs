using FaviconFetcher.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    public class Fetcher
    {
        /// <summary>
        /// The source being used for downloading resources.
        /// </summary>
        public ISource Source { get; private set; }

        /// <summary>
        /// Constructs a Fetcher that uses the default HttpSource for downloading resources.
        /// </summary>
        public Fetcher()
        {
            Source = new HttpSource();
        }

        /// <summary>
        /// Constructs a Fetcher that uses the specified source for downloading resources.
        /// </summary>
        /// <param name="source">The source to use for downloading resources.</param>
        public Fetcher(ISource source)
        {
            Source = source;
        }

        /// <summary>
        /// Fetches the closest favicon to a certain size.
        /// </summary>
        /// <param name="uri">The webpage to scan for favicons.</param>
        /// <param name="size">The target size of the favicon.</param>
        /// <returns>The closest favicon to the size, or null.</returns>
        public Image FetchClosest(Uri uri, Size size)
        {
            return Fetch(uri, new FetchOptions
            {
                PerfectSize = size
            });
        }

        /// <summary>
        /// Fetches the favicon with the exact size specified.
        /// </summary>
        /// <param name="uri">The webpage to scan for favicons.</param>
        /// <param name="size">The target size of the favicon.</param>
        /// <returns>The favicon matching the size, or null.</returns>
        public Image FetchExact(Uri uri, Size size)
        {
            return Fetch(uri, new FetchOptions
            {
                MinimumSize = size,
                MaximumSize = size,
                PerfectSize = size
            });
        }

        /// <summary>
        /// Fetches the best favicon from a webpage per the filter options.
        /// </summary>
        /// <param name="uri">The webpage to scan for favicons.</param>
        /// <param name="options">Filters for the returned result.</param>
        /// <returns>The matching favicon, or null.</returns>
        public Image Fetch(Uri uri, FetchOptions options)
        {
            var parsedUris = new HashSet<Uri>();
            var downloadedImages = new PriorityQueue<double, Image>();
            var notVerified = new PriorityQueue<double, ScanResult>();

            // Scan for icons
            foreach (var possibleIcon in new Scanner(Source).Scan(uri))
            {
                // Because the scanner can return duplicate URIs.
                if (parsedUris.Contains(possibleIcon.Location))
                    continue;
                parsedUris.Add(possibleIcon.Location);

                // Hopefully we've already found it
                if (_IsPerfect(possibleIcon.ExpectedSize, options))
                {
                    foreach (var image in Source.DownloadImages(possibleIcon.Location))
                    {
                        if (image.Size == options.PerfectSize)
                            return image;
                        if (!_IsInRange(image.Size, options))
                            continue;
                        downloadedImages.Add(_GetDistance(image.Size, options), image);
                    }
                }

                // If not, we'll look at it later.
                notVerified.Add(_GetDistance(possibleIcon.ExpectedSize, options), possibleIcon);
            }

            // Download them, prioritizing those closest to perfect
            foreach (var possibleIcon in notVerified)
            {
                foreach (var image in Source.DownloadImages(possibleIcon.Location))
                {
                    if (_IsPerfect(image.Size, options))
                        return image;
                    if (!_IsInRange(image.Size, options))
                        continue;
                    downloadedImages.Add(_GetDistance(image.Size, options), image);
                }
            }

            // Since none were a perfect match, just return the closest
            if (downloadedImages.Count == 0)
                return null;
            return downloadedImages.First();
        }


        // Gets the distance between a size and the perfect size
        private double _GetDistance(Size size, FetchOptions options)
        {
            // Considering the sizes as points, we can find the distance 
            // between them by using the Pythagorean Theorem.
            var dx = size.Width - options.PerfectSize.Width;
            var dy = size.Height - options.PerfectSize.Height;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Is a size within the min/max range?
        private bool _IsInRange(Size size, FetchOptions options)
        {
            if (options.RequireSquare && size.Width != size.Height)
                return false;
            if (size.Width < options.MinimumSize.Width)
                return false;
            if (size.Width > options.MaximumSize.Width)
                return false;
            if (size.Height < options.MinimumSize.Height)
                return false;
            if (size.Height > options.MaximumSize.Height)
                return false;
            return true;
        }

        // Is a size a perfect match?
        private bool _IsPerfect(Size size, FetchOptions options)
        {
            if (options.PerfectSize == Size.Empty)
                return false;
            return size == options.PerfectSize;
        }

    }
}
