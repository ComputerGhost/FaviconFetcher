using System;
using System.Drawing;
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
            fetcher.FetchClosest(uri, new Size(16, 16));

            Assert.AreEqual(source.RequestCount, 2);
        }
    }
}
