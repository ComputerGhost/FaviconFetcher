using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaviconFetcher.SubScanners
{
    // A scan for favicons of a webpage.
    abstract class SubScanner
    {
        // URI to scan.
        public Uri TargetUri { get; internal set; }

        // What to use to download it
        public ISource Source { get; private set; }

        // Found favicon locations from the scan.
        public List<ScanResult> Results = new List<ScanResult>();

        // Additional scanners suggested.
        public List<SubScanner> SuggestedScanners = new List<SubScanner>();


        public SubScanner(ISource source, Uri uri)
        {
            Source = source;
            TargetUri = uri;
        }

        // Start the scan for favicons.
        public abstract Task Start(CancellationTokenSource cancelTokenSource = null);

    }
}
