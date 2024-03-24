using System;
using FaviconFetcher.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class IconImageTests
    {
        [TestMethod]
        public void BitmapIconImage_FromFile()
        {
            var icon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");

            Assert.AreEqual(icon.Size.Width, 256);
            Assert.AreEqual(icon.Size.Height, 256);
        }

        [TestMethod]
        public void SVGIconImage_FromFile()
        {
            var icon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.svg");

            Assert.AreEqual(icon.Size.Width, 256);
            Assert.AreEqual(icon.Size.Height, 256);
        }

        [TestMethod]
        public void SVGIconImage_FromBitmapFile()
        {
            var bitmapIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png").ToSKBitmap();
            var icon = SVGIconImage.FromSKBitmap(bitmapIcon);

            Assert.AreEqual(icon.Size.Width, 256);
            Assert.AreEqual(icon.Size.Height, 256);
        }

        [TestMethod]
        public void SVGIconImage_FromFileScaled()
        {
            var icon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.svg", new IconSize(1000, 1000));

            Assert.AreEqual(icon.Size.Width, 1000);
            Assert.AreEqual(icon.Size.Height, 1000);
        }

        [TestMethod]
        public void BitmapIconImage_FromFileDisproportional()
        {
            var icon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");

            Assert.AreEqual(icon.Size.Width, 256);
            Assert.AreEqual(icon.Size.Height, 512);
        }

        [TestMethod]
        public void BitmapIconImage_SaveOriginalSize()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");
            originalIcon.Save("test.png");
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 256);
        }

        [TestMethod]
        public void SVGIconImage_SaveOriginalSize()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.svg");
            originalIcon.Save("test.png");
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 256);
        }

        [TestMethod]
        public void BitmapIconImage_SaveProportionalResizedLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");
            originalIcon.Save("test.png", new IconSize(500, 500));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 500);
            Assert.AreEqual(savedIcon.Size.Height, 500);
        }

        [TestMethod]
        public void SVGIconImage_SaveProportionalResizedLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.svg");
            originalIcon.Save("test.png", new IconSize(500, 500));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 500);
            Assert.AreEqual(savedIcon.Size.Height, 500);
        }

        [TestMethod]
        public void BitmapIconImage_SaveProportionalResizedSmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");
            originalIcon.Save("test.png", new IconSize(64, 64));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 64);
            Assert.AreEqual(savedIcon.Size.Height, 64);
        }

        [TestMethod]
        public void SVGIconImage_SaveProportionalResizedSmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.svg");
            originalIcon.Save("test.png", new IconSize(64, 64));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 64);
            Assert.AreEqual(savedIcon.Size.Height, 64);
        }

        [TestMethod]
        public void BitmapIconImage_SaveDisproportionalResizedLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(1024, 1024));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 512);
            Assert.AreEqual(savedIcon.Size.Height, 1024);
        }

        [TestMethod]
        public void SVGIconImage_SaveDisproportionalResizedLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.svg");
            originalIcon.Save("test.png", new IconSize(1024, 1024));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 512);
            Assert.AreEqual(savedIcon.Size.Height, 1024);
        }

        [TestMethod]
        public void BitmapIconImage_SaveDisproportionalResizedSmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(64, 64));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 32);
            Assert.AreEqual(savedIcon.Size.Height, 64);
        }

        [TestMethod]
        public void SVGIconImage_SaveDisproportionalResizedSmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.svg");
            originalIcon.Save("test.png", new IconSize(64, 64));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 32);
            Assert.AreEqual(savedIcon.Size.Height, 64);
        }

        [TestMethod]
        public void BitmapIconImage_SaveDisproportionalResizedOriginalHeight()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(1024, originalIcon.Size.Height));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 512);
        }

        [TestMethod]
        public void SVGIconImage_SaveDisproportionalResizedOriginalHeight()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.svg");
            originalIcon.Save("test.png", new IconSize(1024, originalIcon.Size.Height));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 512);
        }

        [TestMethod]
        public void BitmapIconImage_SaveDisproportionalResizedOriginalWidth()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(originalIcon.Size.Width, 1024));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 512);
        }

        [TestMethod]
        public void SVGIconImage_SaveDisproportionalResizedOriginalWidth()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = SVGIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.svg");
            originalIcon.Save("test.png", new IconSize(originalIcon.Size.Width, 1024));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 512);
        }

        [TestMethod]
        public void BitmapIconImage_SaveDisproportionalResizedDisproportionallyLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(2048, 768));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 384);
            Assert.AreEqual(savedIcon.Size.Height, 768);
        }

        [TestMethod]
        public void BitmapIconImage_SaveDisproportionalResizedDisproportionallySmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = BitmapIconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(64, 32));
            var savedIcon = BitmapIconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 16);
            Assert.AreEqual(savedIcon.Size.Height, 32);
        }
    }
}
