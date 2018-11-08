using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    /// <summary>
    /// Filter options for use with a Fetcher.
    /// </summary>
    public class FetchOptions
    {
        /// <summary>
        /// The result will be at least this size.
        /// </summary>
        public Size MinimumSize { get; set; } = Size.Empty;

        /// <summary>
        /// The result will not exceed this size.
        /// </summary>
        public Size MaximumSize { get; set; } = new Size(4096, 4096);

        /// <summary>
        /// The result will be the closest to this size.
        /// </summary>
        public Size PerfectSize { get; set; } = Size.Empty;

        /// <summary>
        /// Whether to require that the favicon be a square.
        /// </summary>
        public bool RequireSquare { get; set; } = false;
    }
}
