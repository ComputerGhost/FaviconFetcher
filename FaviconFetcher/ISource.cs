using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        /// <returns>A reader for the resource, or null.</returns>
        StreamReader DownloadText(Uri uri);

        /// <summary>
        /// Downloads all images from a URI.
        /// </summary>
        /// <remarks>
        /// Multiple images are returned, because some file formats allow multiple images.
        /// </remarks>
        /// <param name="uri">The URI of the image file to download.</param>
        /// <returns>All of the images found within the file.</returns>
        IEnumerable<Image> DownloadImages(Uri uri);
    }
}
