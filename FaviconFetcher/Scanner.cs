using FaviconFetcher.SubScanners;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    public class Scanner
    {
        /// <summary>
        /// The source being used for downloading resources.
        /// </summary>
        public ISource Source { get; private set; }

        /// <summary>
        /// Constructs a Scanner that uses the default HttpSource for downloading resources.
        /// </summary>
        /// <param name="proxy">(Optional) Proxy used for getting web requests</param>
        public Scanner(WebProxy proxy = null)
        {
            Source = new HttpSource(proxy);
        }

        /// <summary>
        /// Constructs a Scanner that uses the specified source for downloading resources.
        /// </summary>
        /// <param name="source">The source to use for downloading resources.</param>
        public Scanner(ISource source)
        {
            Source = source;
        }

        /// <summary>
        /// Scans a URI for references to favicons asynchronously.
        /// </summary>
        /// <param name="uri">The uri of the webpage to scan for favicon references.</param>
        /// <param name="cancelTokenSource">An optional flag for cancelling the scan.</param>
        /// <returns>An enumerable of found favicon references.</returns>
        public async Task<List<ScanResult>> Scan(Uri uri, CancellationTokenSource cancelTokenSource = null)
        {
            var scanResults = new List<ScanResult>();

            var scans = new Queue<SubScanner>();
            scans.Enqueue(new DefaultScanner(Source, uri));

            // While we have subscanners queued
            var max_scans = 4;
            while (scans.Count > 0 && max_scans-- > 0 
                && (cancelTokenSource == null || !cancelTokenSource.IsCancellationRequested))
            {
                var scan = scans.Dequeue();

                await scan.Start(cancelTokenSource);
                scanResults.AddRange(scan.Results);

                // Add all subscanners that are suggested
                foreach (var suggested in scan.SuggestedScanners)
                    scans.Enqueue(suggested);
            }

            return scanResults;
        }

    }
}
