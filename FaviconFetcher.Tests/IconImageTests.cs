using System;
using FaviconFetcher.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FaviconFetcher.Tests
{
    [TestClass]
    public class IconImageTests
    {
        [TestMethod]
        public void IconImage_FromFile()
        {
            var icon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");

            Assert.AreEqual(icon.Size.Width, 256);
            Assert.AreEqual(icon.Size.Height, 256);
        }
        [TestMethod]
        public void IconImage_FromFileDisproportional()
        {
            var icon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");

            Assert.AreEqual(icon.Size.Width, 256);
            Assert.AreEqual(icon.Size.Height, 512);
        }

        [TestMethod]
        public void IconImage_SaveOriginalSize()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");
            originalIcon.Save("test.png");
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 256);
        }

        [TestMethod]
        public void IconImage_SaveProportionalImageResizedLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");
            originalIcon.Save("test.png", new IconSize(500, 500));
            var savedIcon = IconImage.FromFile("test.png");
            System.IO.File.Delete("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 500);
            Assert.AreEqual(savedIcon.Size.Height, 500);
        }

        [TestMethod]
        public void IconImage_SaveProportionalImageResizedSmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Proportional.png");
            originalIcon.Save("test.png", new IconSize(64, 64));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 64);
            Assert.AreEqual(savedIcon.Size.Height, 64);
        }

        [TestMethod]
        public void IconImage_SaveDisproportionalImageResizedLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(1024, 1024));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 512);
            Assert.AreEqual(savedIcon.Size.Height, 1024);
        }

        [TestMethod]
        public void IconImage_SaveDisproportionalImageResizedSmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(64, 64));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 32);
            Assert.AreEqual(savedIcon.Size.Height, 64);
        }

        [TestMethod]
        public void IconImage_SaveDisproportionalImageResizedOriginalHeight()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(1024, originalIcon.Size.Height));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 512);
        }

        [TestMethod]
        public void IconImage_SaveDisproportionalImageResizedOriginalWidth()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(originalIcon.Size.Width, 1024));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 256);
            Assert.AreEqual(savedIcon.Size.Height, 512);
        }

        [TestMethod]
        public void IconImage_SaveDisproportionalImageResizedDisproportionallyLarger()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(2048, 768));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 384);
            Assert.AreEqual(savedIcon.Size.Height, 768);
        }

        [TestMethod]
        public void IconImage_SaveDisproportionalImageResizedDisproportionallySmaller()
        {
            System.IO.File.Delete("test.png");
            var originalIcon = IconImage.FromFile(filename: "..\\..\\..\\Resources\\TestIcon-Disproportional.png");
            originalIcon.Save("test.png", new IconSize(64, 32));
            var savedIcon = IconImage.FromFile("test.png");

            Assert.AreEqual(savedIcon.Size.Width, 16);
            Assert.AreEqual(savedIcon.Size.Height, 32);
        }
    }
}
