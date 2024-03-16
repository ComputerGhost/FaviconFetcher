using FaviconFetcher.Utility;
using System;
using System.Net;
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
        /// <param name="proxy">(Optional) Proxy used for getting web requests</param>
        public Fetcher(WebProxy proxy = null)
        {
            Source = new HttpSource(proxy);
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
        public async Task<IconImage> FetchClosest(Uri uri, IconSize size)
        {
            return await Fetch(uri, new FetchOptions
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
        public async Task<IconImage> FetchExact(Uri uri, IconSize size)
        {
            return await Fetch(uri, new FetchOptions
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
        public async Task<IconImage> Fetch(Uri uri, FetchOptions options)
        {
            using (var fetch = new FetchJob(Source, uri, options))
                return await fetch.ScanAndFetch();
        }

    }
}
