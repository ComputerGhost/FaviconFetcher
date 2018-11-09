using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class ScannerTests
    {

        [TestMethod]
        public void ShouldUseFaviconIcoWhenNoneSpecified()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "Fake content.");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1);
            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.ico"),
                ExpectedSize = new Size(16, 16)
            });
        }

        [TestMethod]
        public void ShouldHandleIconLink()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='shortcut icon' href='favicon.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.png"),
                ExpectedSize = new Size(16, 16)
            });
        }

        [TestMethod]
        public void ShouldHandleIconLinkWithSizes()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='icon' href='favicons.ico' sizes='16x16 32x32'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/favicons.ico"),
                ExpectedSize = new Size(16, 16)
            });
            Assert.AreEqual(results[1], new ScanResult
            {
                Location = new Uri("http://www.example.com/favicons.ico"),
                ExpectedSize = new Size(32, 32)
            });
        }

        [TestMethod]
        public void ShouldHandleMultipleIconLinks()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='icon' href='favicon1.png'>
                    <link rel='icon' href='favicon2.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 3); // 2 above plus the default
        }

        [TestMethod]
        public void ShouldSkipInvalidIconLink()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='icon' src='favicon1.png'>
                    <link href='favicon1.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void ShouldUseFaviconIcoWhenNotFound()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1);
        }

        [TestMethod]
        public void ShouldDefaultSize57x57WhenApple()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=apple-touch-icon-precomposed href='apple_icon.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/apple_icon.png"),
                ExpectedSize = new Size(57, 57)
            });
        }

        [TestMethod]
        public void ShouldUseSpecifiedSizeWhenApple()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=apple-touch-icon-precomposed href='apple_icon.png' sizes='48x48'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/apple_icon.png"),
                ExpectedSize = new Size(48, 48)
            });
        }

        [TestMethod]
        public void ShouldDeduceSizeFromName()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='favicon_48x48.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon_48x48.png"),
                ExpectedSize = new Size(48, 48)
            });
        }

        [TestMethod]
        public void ShouldHandleInvalidHtml_UnterminatedQuote()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link rel='");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void ShouldHandleInvalidHtml_CutAtValue()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link rel=");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void ShouldHandleInvalidHtml_CutAfterLink()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void ShouldIgnoreIconLinkInBody()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><body>
                    <link rel=icon href='favicon.png'>
                </body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void ShouldHandleMixedQuotes()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><body>
                    <link rel=icon href=""favicon's.png"">
                </body></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void ShouldHandleBigNumbersInUri()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='icon_21474836470_48.png'>
                </head></html>");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results[0], new ScanResult
            {
                Location = new Uri("http://www.example.com/icon_21474836470_48.png"),
                ExpectedSize = new Size(48, 48)
            });
        }

    }
}
