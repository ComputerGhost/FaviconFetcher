using System;
using System.Linq;
using FaviconFetcher.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class ScannerTests
    {
        [TestMethod]
        public void Scan_AllResources_FindAll()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta rel='manifest' href='/manifest.json'>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                    <link rel=icon href='favicon.png'>
                </head><body>Fake content.</body></html>");
            source.AddTextResource(new Uri(uri, "/manifest.json"), @"
                {'icons': [
                {'src': '/android-chrome-36x36.png', 'sizes': '36x36', 'type': 'image/png', 'density': 0.75},
                {'src': '/android-chrome-48x48.png', 'sizes': '48x48', 'type': 'image/png', 'density': 1},
                {'src': '/android-chrome-72x72.png', 'sizes': '72x72', 'type': 'image/png', 'density': 1.5},
                {'src': '/android-chrome-96x96.png', 'sizes': '96x96', 'type': 'image/png', 'density': 2},
                {'src': '/android-chrome-144x144.png', 'sizes': '144x144', 'type': 'image/png', 'density': 3},
                {'src': '/android-chrome-192x192.png', 'sizes': '192x192', 'type': 'image/png', 'density': 4}
                ]}".Replace('\'', '"'));
            source.AddTextResource(new Uri(uri, "/browserconfig.xml"),
                @"<?xml version='1.0' encoding='utf-8'?>
                <browserconfig><msapplication><tile>
                    <square70x70logo src='/mstile-70x70.png' />
                    <square150x150logo src='/mstile-150x150.png' />
                    <square310x310logo src='/mstile-310x310.png' />
                    <wide310x150logo src='/mstile-310x150.png' />
                    <TileColor>#29aaff</TileColor>
                </tile></msapplication></browserconfig>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).GetAwaiter().GetResult().ToArray();

            Assert.AreEqual(12, results.Length);
        }
    }
}
