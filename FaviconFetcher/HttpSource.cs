using FaviconFetcher.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
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
        /// The HTTP User-agent header sent for web requests.  The "e" is in
        /// "fetch" is swapped out with the number "3" here because a number
        /// of sites block requests with "fetch" in the userAgent.
        /// </summary>
        public string UserAgent = "FaviconF3tcher/1.2";

        /// <summary>
        /// Proxy used for getting web requests
        /// </summary>
        public WebProxy RequestsProxy = null;

        /// <summary>
        /// Creates a HttpSource for accessing the websites
        /// </summary>
        /// <param name="proxy">(Optional) Proxy used for getting web requests</param>
        public HttpSource(WebProxy proxy = null)
        {
            RequestsProxy = proxy;
        }

        /// <summary>
        /// Internal use only. Downloads a text resource from a URI.
        /// </summary>
        /// <param name="uri">The uri of the resource to download.</param>
        /// <param name="cancelTokenSource">An optional flag for cancelling the download.</param>
        /// <returns>A reader for the resource, or null.</returns>
        public async Task<StreamReader> DownloadText(Uri uri, CancellationTokenSource cancelTokenSource)
        {
            var cancelToken = cancelTokenSource != null 
                ? cancelTokenSource.Token 
                : CancellationToken.None;

            var response = await _GetWebResponse(uri, cancelToken);
            if (cancelToken.IsCancellationRequested
                || response == null
                || response.StatusCode != HttpStatusCode.OK)
            {
                response?.Dispose(); // since we won't be passing on the response stream.
                return null;
            }

            // Header has priority.
            // Byte Order Mark is second priority.
            // Otherwise default to ASCII, since it'll be ASCII-compatible.
            if (response.ContentType.Contains("charset="))
            {
                try
                {
                    var charset = response.CharacterSet.Replace("\"", "");
                    var encoding = Encoding.GetEncoding(charset);
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
        /// <param name="cancelTokenSource">An optional flag for cancelling the download.</param>
        /// <returns>All of the images found within the file.</returns>
        public async Task<IEnumerable<IconImage>> DownloadImages(Uri uri, CancellationTokenSource cancelTokenSource)
        {
            var cancelToken = cancelTokenSource != null
                ? cancelTokenSource.Token
                : CancellationToken.None;

            var images = new List<IconImage>();
            var contentType = string.Empty;
            var memoryStream = new MemoryStream();
            Uri responseUri = null;

            using (var response = await _GetWebResponse(uri, cancelToken))
            {
                if (cancelToken.IsCancellationRequested
                    || response == null
                    || response.StatusCode != HttpStatusCode.OK)
                    return images;

                contentType = response.ContentType.ToLower();
                response.GetResponseStream().CopyTo(memoryStream);

                // Were we redirected and received a non-image response?
                if (!uri.Equals(response.ResponseUri) 
                    && contentType.Contains("text/html"))
                {
                    responseUri = response.ResponseUri;
                }
            }

            if (responseUri != null)
            {
                var redirectedUri = new Uri(responseUri.GetLeftPart(UriPartial.Authority).ToString() + uri.PathAndQuery);
                // Try fetching same resource at the root of the redirected URI
                using (var response = await _GetWebResponse(redirectedUri, cancelToken))
                {
                    if (cancelToken.IsCancellationRequested
                        || response == null
                        || response.StatusCode != HttpStatusCode.OK)
                        return images;

                    contentType = response.ContentType;
                    memoryStream = new MemoryStream();
                    response.GetResponseStream().CopyTo(memoryStream);
                }
            }

            // Ico file
            if (_IsContentTypeIco(contentType))
            {
                try
                {
                    foreach (var size in _ExtractIcoSizes(memoryStream))
                    {
                        memoryStream.Position = 0;
                        images.Add(IconImage.FromIco(memoryStream, size));
                    }
                    return images;
                }

                // Sometimes a website lies about "ico".
                catch (EndOfStreamException) { }
                catch (ArgumentException) { }
                // We'll let this fall through to try another image type.
                memoryStream.Position = 0;
            }

            // Other image type
            try
            {
                images.Add(IconImage.FromStream(memoryStream));
            }
            catch (ArgumentException) {}
            return images;
        }


        // Extract image sizes from ICO file
        private IEnumerable<IconSize> _ExtractIcoSizes(Stream stream)
        {
            var reader = new BinaryReader(stream, Encoding.UTF8, true);

            // Skip to count
            stream.Seek(4, SeekOrigin.Begin);
            var count = reader.ReadInt16();

            var sizes = new List<IconSize>();
            for (var i = 0; i != count; ++i)
            {
                var offset = 6 + i * 16;
                stream.Seek(offset, SeekOrigin.Begin);
                int width = reader.ReadByte();
                if (width == 0) width = 256;
                int height = reader.ReadByte();
                if (height == 0) height = 256;
                sizes.Add(new IconSize(width, height));
            }

            return sizes;
        }
        
        // Setup and make a web request, returning the response.
        private async Task<HttpWebResponse> _GetWebResponse(Uri uri, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("Fetching");

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var request = WebRequest.Create(uri) as HttpWebRequest;
#pragma warning restore SYSLIB0014 // Type or member is obsolete

            request.CachePolicy = CachePolicy;
            request.UserAgent = UserAgent;

            if (RequestsProxy != null)
                request.Proxy = RequestsProxy;

            // GetResponse returns response in exception if error code...
            // so we need to handle it in a try-catch.
            try
            {
                return await request.GetResponseAsync().WithCancellation(cancellationToken, request.Abort, true) as HttpWebResponse;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw;
                return ex.Response as HttpWebResponse;
            }
        }

        // Check whether the file is an ico.
        private bool _IsContentTypeIco(string contentType)
        {
            // Check content type
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
