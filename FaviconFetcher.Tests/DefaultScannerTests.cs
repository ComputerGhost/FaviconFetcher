using System;
using FaviconFetcher.SubScanners;
using FaviconFetcher.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class DefaultScannerTests
    {
        [TestMethod]
        public void Results_OneLink_FindIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='shortcut icon' href='favicon.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.png"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_LinkHrefWithoutQuotes_FindIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='shortcut icon' href=favicon.png>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.png"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_LinkRelInCaps_FindIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='SHORTCUT ICON' href='favicon.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.png"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_HtmlInCaps_ParseIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <HTML><HEAD>
                    <link rel='SHORTCUT ICON' href='favicon.png'>
                </HEAD><BODY>Fake content.</BODY></HTML>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(1, scanner.Results.Count);
        }

        [TestMethod]
        public void Results_LinkHasSizes_UseSizes()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='icon' href='favicons.ico' sizes='16x16 32x32'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicons.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicons.ico"),
                ExpectedSize = new IconSize(32, 32)
            }, scanner.Results[1]);
        }

        [TestMethod]
        public void Results_MultipleLinks_FindAll()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='icon' href='favicon1.png'>
                    <link rel='icon' href='favicon2.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(2, scanner.Results.Count);
        }

        [TestMethod]
        public void Results_AppleLink_UseSize57x57()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=apple-touch-icon-precomposed href='apple_icon.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/apple_icon.png"),
                ExpectedSize = new IconSize(57, 57)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_SizedAppleLink_UseSpecifiedSize()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=apple-touch-icon-precomposed href='apple_icon.png' sizes='48x48'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/apple_icon.png"),
                ExpectedSize = new IconSize(48, 48)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_SizeInName_UseGuessedSize()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='favicon_48x48.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon_48x48.png"),
                ExpectedSize = new IconSize(48, 48)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_SizeInNameAndAttribute_UseAttributeSize()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='favicon_48x48.png' sizes='16x16'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon_48x48.png"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_BigNumberInUri_Accepted()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='icon_21474836470_48.png'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/icon_21474836470_48.png"),
                ExpectedSize = new IconSize(48, 48)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_LinkInBody_Ignored()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><body>
                    <link rel=icon href='favicon.png'>
                </body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
        }

        [TestMethod]
        public void Results_QuoteInUri_Accepted()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href=""favicon's.png"">
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon's.png"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_InvalidUri_Skip()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='///invalid'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_BaseAfterLocation_ModifiesPreviousLocation()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel=icon href='favicon.ico'>
                    <base href='http://www.other.com'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.other.com/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_AbsoluteBase_PrefixesToLocations()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href='http://www.other.com'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.other.com/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_RelativeBase_PrefixesTargetAndBaseToLocations()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href='icons/'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/icons/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_EmptyBase_IgnoresIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href=''>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void Results_InvalidBase_IgnoresIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href='..'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(new ScanResult
            {
                Location = new Uri("http://www.example.com/favicon.ico"),
                ExpectedSize = new IconSize(16, 16)
            }, scanner.Results[0]);
        }

        [TestMethod]
        public void SuggestedScanners_NoLinks_SuggestFaviconIco()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "Fake content.");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count);
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(FaviconIcoScanner));
        }

        [TestMethod]
        public void SuggestedScanners_InvalidLinks_SuggestFaviconIco()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <link rel='icon' src='favicon1.png'>
                    <link href='favicon1.png'>
                </head><body>Fake content.</body></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count);
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(FaviconIcoScanner));
        }

        [TestMethod]
        public void SuggestedScanners_NotFound_SuggestFaviconIco()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count);
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(FaviconIcoScanner));
        }

        [TestMethod]
        public void SuggestedScanners_HtmlQuoteUnterminated_SuggestFaviconIco()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link rel='");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count);
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(FaviconIcoScanner));
        }

        [TestMethod]
        public void SuggestedScanners_HtmlEndsAtValue_SuggestFaviconIco()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link rel=");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count);
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(FaviconIcoScanner));
        }

        [TestMethod]
        public void SuggestedScanners_HtmlEndsAtTag_SuggestFaviconIco()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, "<html><head><link");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count);
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(FaviconIcoScanner));
        }

        [TestMethod]
        public void SuggestedScanners_BrowserconfigXmlLinked_SuggestIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(2, scanner.SuggestedScanners.Count); // plus FaviconIcoScanner
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(BrowserconfigXmlScanner));
        }

        [TestMethod]
        public void SuggestedScanners_BrowserconfigXmlMissingUri_Skip()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta name='msapplication-config'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(1, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count); // just FaviconIcoScanner
        }

        [TestMethod]
        public void SuggestedScanners_InvalidBrowserconfigXmlUri_Skip()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta name='msapplication-config' content='///browserconfig.xml'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(1, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count); // just FaviconIcoScanner
        }

        [TestMethod]
        public void SuggestedScanners_ManifestJsonLinked_SuggestIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta rel='manifest' href='/manifest.json'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(0, scanner.Results.Count);
            Assert.AreEqual(2, scanner.SuggestedScanners.Count); // plus FaviconIcoScanner
            Assert.IsInstanceOfType(scanner.SuggestedScanners[0], typeof(ManifestJsonScanner));
        }

        [TestMethod]
        public void SuggestedScanners_ManfiestJsonMissingUri_Skip()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta rel='manifest'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(1, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count); // just FaviconIcoScanner
        }

        [TestMethod]
        public void SuggestedScanners_InvalidManifestJsonUri_Skip()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta rel='manifest' href='///manifest.json'>
                    <link rel=icon href='favicon.ico'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual(1, scanner.Results.Count);
            Assert.AreEqual(1, scanner.SuggestedScanners.Count); // just FaviconIcoScanner
        }

        [TestMethod]
        public void SuggestedScanner_BaseAfterLocation_ModifiesPreviousLocation()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                    <base href='http://www.other.com'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual("http://www.other.com/browserconfig.xml", scanner.SuggestedScanners[0].TargetUri.ToString());
        }

        [TestMethod]
        public void SuggestedScanner_AbsoluteBase_PrefixesToLocations()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href='http://www.other.com'>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual("http://www.other.com/browserconfig.xml", scanner.SuggestedScanners[0].TargetUri.ToString());
        }

        [TestMethod]
        public void SuggestedScanner_RelativeBase_PrefixesTargetAndBaseToLocations()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href='sub/'>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual("http://www.example.com/sub/browserconfig.xml", scanner.SuggestedScanners[0].TargetUri.ToString());
        }

        [TestMethod]
        public void SuggestedScanner_EmptyBase_IgnoresIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href=''>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual("http://www.example.com/browserconfig.xml", scanner.SuggestedScanners[0].TargetUri.ToString());
        }

        [TestMethod]
        public void SuggestedScanner_InvalidBase_IgnoresIt()
        {
            var uri = new Uri("http://www.example.com");
            var source = new MockSource();
            source.AddTextResource(uri, @"
                <html><head>
                    <base href='..'>
                    <meta name='msapplication-config' content='browserconfig.xml'>
                </head></html>");

            var scanner = new DefaultScanner(source, uri);
            scanner.Start().GetAwaiter();

            Assert.AreEqual("http://www.example.com/browserconfig.xml", scanner.SuggestedScanners[0].TargetUri.ToString());
        }
    }
}
