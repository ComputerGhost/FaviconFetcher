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
        public void Scan_NoLinks_ReturnDefault()
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
        public void Scan_OneLink_Returned()
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
        public void Scan_LinkHasSizes_UseSizes()
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
        public void Scan_MultipleLinks_ReturnAll()
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
        public void Scan_InvalidLinks_ReturnDefault()
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
        public void Scan_NotFound_ReturnDefault()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1);
        }

        [TestMethod]
        public void Scan_AppleLink_UseSize57x57()
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
        public void Scan_SizedAppleLink_UseSpecifiedSize()
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
        public void Scan_SizeInName_UseGuessedSize()
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
        public void Scan_HtmlQuoteUnterminated_ReturnDefault()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link rel='");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void Scan_HtmlEndsAtValue_ReturnDefault()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link rel=");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void Scan_HtmlEndsAtTag_ReturnDefault()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link");

            var scanner = new Scanner(source);
            var results = scanner.Scan(uri).ToArray();

            Assert.AreEqual(results.Length, 1); // Just the default
        }

        [TestMethod]
        public void Scan_LinkInBody_Ignored()
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
        public void Scan_MixedQuotes_Returned()
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
        public void Scan_BigNumberInUri_Returned()
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
