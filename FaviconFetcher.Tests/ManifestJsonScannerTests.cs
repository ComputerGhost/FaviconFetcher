using System;
using FaviconFetcher.SubScanners;
using FaviconFetcher.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class ManifestJsonScannerTests
    {
        [TestMethod]
        public void Start_ValidJson_Parse()
        {
            var uri = new Uri("http://www.example.com/manifest.json");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                {'name': 'Favicon',
                'icons': [
	                {'src': '/android-chrome-36x36.png', 'sizes': '36x36'},
	                {'src': '/android-chrome-48x48.png', 'sizes': '48x48'}
                ]}".Replace('\'', '"'));

            var scanner = new ManifestJsonScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(2, scanner.Results.Count);
        }

        [TestMethod]
        public void Start_InvalidJson_Skip()
        {
            var uri = new Uri("http://www.example.com/manifest.json");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                {'name': 'Favicon',
                'icons': [
	                {'src': '/android-chrome-36x36.png', 'sizes': '36x36'},
	                {'src': '/android-chrome-48x48.png', 'sizes': '48x48'}
                ]}");

            var scanner = new ManifestJsonScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
        }

        [TestMethod]
        public void Start_ContainsInvalidUri_Skip()
        {
            var uri = new Uri("http://www.example.com/manifest.json");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                {'name': 'Favicon',
                'icons': [
	                {'src': '///invalid.png', 'sizes': '36x36'},
	                {'src': '/android-chrome-48x48.png', 'sizes': '48x48'}
                ]}".Replace('\'', '"'));

            var scanner = new ManifestJsonScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(1, scanner.Results.Count);
        }
    }
}
