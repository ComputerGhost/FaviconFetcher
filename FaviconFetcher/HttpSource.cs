using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    /// <summary>
    /// Default tool used by FaviconFetcher to download resources from a website.
    /// </summary>
    public class HttpSource : ISource
    {
        /// <summary>
        /// The cache policy used for web requests.
        /// </summary>
        public RequestCachePolicy CachePolicy = WebRequest.DefaultCachePolicy;

        /// <summary>
        /// The HTTP User-agent header sent for web requests.
        /// </summary>
        public string UserAgent = "FaviconFetcher/1.0";

        /// <summary>
        /// Internal use only. Downloads a text resource from a URI.
        /// </summary>
        /// <param name="uri">The uri of the resource to download.</param>
        /// <returns>A reader for the resource, or null.</returns>
        public StreamReader DownloadText(Uri uri)
        {
            var response = _GetWebResponse(uri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                response.Dispose(); // since we won't be passing on the response stream.
                return null;
            }

            // Header has priority.
            // Byte Order Mark is second priority.
            // Otherwise default to ASCII, since it'll be ASCII-compatible.
            if (response.ContentType.Contains("charset="))
            {
                try
                {
                    var encoding = Encoding.GetEncoding(response.CharacterSet);
                    return new StreamReader(response.GetResponseStream(), encoding);
                }
                catch (NotSupportedException) { }
            }
            return new StreamReader(response.GetResponseStream(), Encoding.ASCII, true);
        }

        /// <summary>
        /// Internal use only. Downloads all images from a URI.
        /// </summary>
        /// <param name="uri">The URI of the image file to download.</param>
        /// <returns>All of the images found within the file.</returns>
        public IEnumerable<Image> DownloadImages(Uri uri)
        {
            var images = new List<Image>();
            using (var response = _GetWebResponse(uri))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    return images;

                if (_IsIcoFile(response.ContentType))
                {
                    var memoryStream = new MemoryStream();
                    response.GetResponseStream().CopyTo(memoryStream);

                    foreach (var size in _ExtractIcoSizes(memoryStream))
                    {
                        memoryStream.Position = 0;
                        images.Add(new Icon(memoryStream, size).ToBitmap());
                    }
                    return images;
                }
                else
                    images.Add(Image.FromStream(response.GetResponseStream()));
            }
            return images;
        }


        // Extract image sizes from ICO file
        private IEnumerable<Size> _ExtractIcoSizes(Stream stream)
        {
            var reader = new BinaryReader(stream, Encoding.UTF8, true);

            // Skip to count
            stream.Seek(4, SeekOrigin.Begin);
            var count = reader.ReadInt16();

            var sizes = new List<Size>();
            for (var i = 0; i != count; ++i)
            {
                var offset = 6 + i * 16;
                int width = reader.ReadByte();
                if (width == 0) width = 256;
                int height = reader.ReadByte();
                if (height == 0) height = 256;
                sizes.Add(new Size(width, height));
            }

            return sizes;
        }
        
        // Setup and make a web request, returning the response.
        private HttpWebResponse _GetWebResponse(Uri uri)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.CachePolicy = CachePolicy;
            request.UserAgent = UserAgent;
            return request.GetResponse() as HttpWebResponse;
        }

        // Check whether the content type is an ico.
        private bool _IsIcoFile(string contentType)
        {
            var iconTypes = new[] {
                "image/x-icon",
                "image/vnd.microsoft.icon",
                "image/ico",
                "image/icon",
                "text/ico",
                "application/ico"
            };
            foreach (var iconType in iconTypes)
            {
                if (contentType.Contains(iconType))
                    return true;
            }
            return false;
        }

    }
}
