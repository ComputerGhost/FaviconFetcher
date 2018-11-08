using FaviconFetcher.SubScanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        public Scanner()
        {
            Source = new HttpSource();
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
        /// Scans a URI for references to favicons.
        /// </summary>
        /// <param name="uri">The uri of the webpage to scan for favicon references.</param>
        /// <returns>An enumerable of found favicon references.</returns>
        public IEnumerable<ScanResult> Scan(Uri uri)
        {
            var scans = new Queue<SubScanner>();
            scans.Enqueue(new DefaultScanner(Source, uri));

            var max_scans = 4;
            while (scans.Count > 0 && max_scans-- > 0)
            {
                var scan = scans.Dequeue();
                scan.Start();
                foreach (var result in scan.Results)
                    yield return result;
                foreach (var suggested in scan.SuggestedScanners)
                    scans.Enqueue(suggested);
            }
        }

    }
}
