using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    /// <summary>
    /// Information about a possible favicon.
    /// </summary>
    public struct ScanResult
    {
        /// <summary>
        /// Absolute URI of the favicon.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// Expected size of the favicon.
        /// </summary>
        public Size ExpectedSize { get; set; }
    }
}
