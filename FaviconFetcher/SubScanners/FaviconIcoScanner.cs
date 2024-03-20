using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if DEBUG
[assembly: InternalsVisibleTo("FaviconFetcher.Tests")]
#endif
namespace FaviconFetcher.SubScanners
{
    class FaviconIcoScanner : SubScanner
    {

        public FaviconIcoScanner(ISource source, Uri uri) : base(source, uri)
        {
        }

        public override Task Start(CancellationTokenSource cancelTokenSource = null)
        {
            Results.Add(new ScanResult
            {
                Location = new Uri(TargetUri, "/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            });

            return Task.CompletedTask;
        }

    }
}
