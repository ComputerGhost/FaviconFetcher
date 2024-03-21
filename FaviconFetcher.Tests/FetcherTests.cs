using System;
using System.Threading.Tasks;
using FaviconFetcher.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class FetcherTests
    {
        [TestMethod]
        public void Fetcher_NoLinks_MakeTwoRequests()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "Fake content.");

            var fetcher = new Fetcher(source);
            fetcher.FetchClosest(uri, new IconSize(16, 16)).GetAwaiter();

            Assert.AreEqual(2, source.RequestCount);
        }

        [TestMethod]
        public void Fetcher_NonexistentLink_MakeThreeRequests()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='shortcut icon' href='nonexistent.png'>
                </head></html>");

            var fetcher = new Fetcher(source);
            fetcher.FetchClosest(uri, new IconSize(16, 16)).GetAwaiter();

            Assert.AreEqual(3, source.RequestCount);
        }

        [TestMethod]
        public void Fetcher_PerfectIsLastLink_MakeTwoRequests()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='shortcut icon' href='favicon_32.png' sizes='32x32'>
                    <link rel='shortcut icon' href='favicon_16.png' sizes='16x16'>
                </head></html>");
            source.AddImageResource(new Uri(uri, "/favicon_32.png"), new IconSize(32, 32));
            source.AddImageResource(new Uri(uri, "/favicon_16.png"), new IconSize(16, 16));

            var fetcher = new Fetcher(source);
            fetcher.FetchClosest(uri, new IconSize(16, 16)).GetAwaiter();

            Assert.AreEqual(2, source.RequestCount);
        }

        [TestMethod]
        public void Fetcher_MultipleIconsInFile_UseBest()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "Fake content.");
            source.AddImageResource(new Uri(uri, "/favicon.ico"), new IconSize(16, 16));
            source.AddImageResource(new Uri(uri, "/favicon.ico"), new IconSize(32, 32));
            source.AddImageResource(new Uri(uri, "/favicon.ico"), new IconSize(48, 48));

            var fetcher = new Fetcher(source);
            var image = fetcher.FetchClosest(uri, new IconSize(32, 32)).GetAwaiter().GetResult();

            Assert.AreEqual(2, source.RequestCount);
            Assert.AreEqual(new IconSize(32, 32), image.Size);
        }
    }
}
