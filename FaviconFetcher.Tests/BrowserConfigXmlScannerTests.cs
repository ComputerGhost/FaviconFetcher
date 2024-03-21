using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaviconFetcher.SubScanners;
using FaviconFetcher.Tests.Utility;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class BrowserConfigXmlScannerTests
    {
        [TestMethod]
        public void Start_ValidXml_Parse()
        {
            var uri = new Uri("http://www.example.com/browserconfig.xml");
            var source = new MockSource();
            source.AddTextResource(uri,
                @"<?xml version='1.0' encoding='utf-8'?>
                <browserconfig>
                  <msapplication>
                    <tile>
                      <square70x70logo src='/mstile-70x70.png' />
                      <square150x150logo src='/mstile-150x150.png' />
                      <square310x310logo src='/mstile-310x310.png' />
                      <wide310x150logo src='/mstile-310x150.png' />
                      <TileColor>#29aaff</TileColor>
                    </tile>
                  </msapplication>
                </browserconfig>");

            var scanner = new BrowserconfigXmlScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(4, scanner.Results.Count);
        }

        [TestMethod]
        public void Start_InvalidXml_Skip()
        {
            var uri = new Uri("http://www.example.com/browserconfig.xml");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <browserconfig>>");

            var scanner = new BrowserconfigXmlScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
        }

        [TestMethod]
        public void Start_ContainsInvalidUri_Skip()
        {
            var uri = new Uri("http://www.example.com/browserconfig.xml");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <?xml version='1.0' encoding='utf-8'?>
                <browserconfig>
                  <msapplication>
                    <tile>
                      <square70x70logo src='///invalid.png' />
                      <square150x150logo src='/mstile-150x150.png' />
                      <square310x310logo src='/mstile-310x310.png' />
                      <wide310x150logo src='/mstile-310x150.png' />
                      <TileColor>#29aaff</TileColor>
                    </tile>
                  </msapplication>
                </browserconfig>");

            var scanner = new BrowserconfigXmlScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(3, scanner.Results.Count);
        }
    }
}
