using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaviconFetcher
{
    /// <summary>
    /// Default tool used by FaviconFetcher to download resources from a website.
    /// </summary>
    public class HttpClientSource : ISource
    {
        /// HttpClient for performing requests for this source
        /// Static to be thread-safe
        /// 
        /// https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-net-http-httpclient
        /// HttpClient is intended to be instantiated once and reused 
        /// throughout the life of an application. This should probably
        /// be refactored to utilize a singleton outside HttpSource
        /// so that multiple Scan/Fetch doesn't instantiate new
        /// instances of HttpClient.
        static private HttpClient _httpClient = null;

        /// Handler that stores HttpClient parameters such as proxy and cookie settings
        /// Static to be thread-safe along with HttpClient
        static private HttpClientHandler _httpClientHandler = null;

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
        public HttpClientSource(WebProxy proxy = null)
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
            if (response.Content.Headers.ContentType.ToString().ToLower().Contains("charset="))
            {
                try
                {
                    var charset = response.Content.Headers.ContentType.CharSet.Replace("\"", "");
                    var encoding = Encoding.GetEncoding(charset);
                    return new StreamReader(await response.Content.ReadAsStreamAsync(), encoding);
                }
                catch (NotSupportedException) { }
            }
            return new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.ASCII, true);
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

                contentType = response.Content.Headers.ContentType.ToString().ToLower();
                await (await response.Content.ReadAsStreamAsync()).CopyToAsync(memoryStream);

                // Were we redirected and received a non-image response?
                if (response.RequestMessage != null
                    && !uri.Equals(response.RequestMessage.RequestUri)
                    && contentType.Contains("text/html"))
                {
                    responseUri = response.RequestMessage.RequestUri;
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

                    contentType = response.Content.Headers.ContentType.ToString().ToLower();
                    memoryStream = new MemoryStream();
                    await (await response.Content.ReadAsStreamAsync()).CopyToAsync(memoryStream);
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
            catch (ArgumentException) { }
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
        private async Task<HttpResponseMessage> _GetWebResponse(Uri uri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                // A number of sites are picky about incoming requests and block
                // or delay without a User-Agent, Accept-Language and/or
                // an Accept-Encoding header, so we add them here.  Note that the 
                // handler must be set to auto-decompress any specified encodings.
                // It's worth mentioning that some sites block less common UA
                // strings, so calling app may need to specify a common agent.
                _ = request.Headers.UserAgent.TryParseAdd(UserAgent);
                _ = request.Headers.AcceptLanguage.TryParseAdd("en-US,en"); // TODO : Get system locale languages
                _ = request.Headers.AcceptEncoding.TryParseAdd("deflate,gzip;q=1.0,*;q=0.5");

                response = await SourceHttpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (HttpRequestException ex)
            {
                return response != null
                    ? new HttpResponseMessage(response.StatusCode)
                    : new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
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

        /// <summary>
        /// Returns an instantiated HttpClient
        /// </summary>
        private HttpClient SourceHttpClient
        {
            get
            {
                return GetHttpClient(this);
            }
        }

        /// <summary>
        /// Returns the current static HttpClient for the source, 
        /// creating a new one when required
        /// </summary>
        private static HttpClient GetHttpClient(HttpClientSource source)
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient(GetHttpClientHandler(source));
                _httpClient.DefaultRequestHeaders.ConnectionClose = true; // Don't keep connections alive
            }

            return _httpClient;
        }

        /// <summary>
        /// Returns the current static HttpClientHandler for the source, 
        /// creating a new one when required. Settings can only be changed
        /// before requests are made.
        /// </summary>
        private static HttpClientHandler GetHttpClientHandler(HttpClientSource source)
        {
            if (_httpClientHandler == null)
            {
                _httpClientHandler = new HttpClientHandler();
                _httpClientHandler.UseProxy = false;
                _httpClientHandler.AllowAutoRedirect = true;
                _httpClientHandler.MaxAutomaticRedirections = 5;
                _httpClientHandler.MaxConnectionsPerServer = 1;
                // Request mechanism sets Allow-Encoding header to gzip/deflate, so we need to enable it
                _httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                // Some hosts require cookies to persist after redirects
                _httpClientHandler.CookieContainer = new CookieContainer(); ;
                _httpClientHandler.UseCookies = true;

                if (source.RequestsProxy != null)
                {
                    _httpClientHandler.UseProxy = true;
                    _httpClientHandler.Proxy = source.RequestsProxy;
                }
            }

            return _httpClientHandler;
        }

    }
}
