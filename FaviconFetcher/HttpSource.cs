using System.Net;

namespace FaviconFetcher
{
    /// <summary>
    /// Default tool used by FaviconFetcher to download resources from a website.
    /// </summary>
    public class HttpSource : WebRequestSource
    {
        /// <summary>
        /// Creates a HttpSource for accessing the websites
        /// </summary>
        /// <param name="proxy">(Optional) Proxy used for getting web requests</param>
        public HttpSource(WebProxy proxy = null) : base(proxy) { }
    }
}
