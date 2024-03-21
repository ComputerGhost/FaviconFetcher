using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    /// <summary>
    /// Tool used by FaviconFetcher to download resources from a website.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Downloads a text-based resource from a URI.
        /// </summary>
        /// <param name="uri">The URI of the resource to download.</param>
        /// <param name="cancelTokenSource">An optional flag for cancelling the download.</param>
        /// <returns>A reader for the resource, or null.</returns>
        Task<StreamReader> DownloadText(Uri uri, CancellationTokenSource cancelTokenSource);

        /// <summary>
        /// Downloads all images from a URI.
        /// </summary>
        /// <remarks>
        /// Multiple images are returned, because some file formats allow multiple images.
        /// </remarks>
        /// <param name="cancelTokenSource">An optional flag for cancelling the download.</param>
        /// <param name="cancelTokenSource"></param>
        /// <returns>All of the images found within the file, or an empty list.</returns>
        Task<IEnumerable<IconImage>> DownloadImages(Uri uri, CancellationTokenSource cancelTokenSource);
    }
}
