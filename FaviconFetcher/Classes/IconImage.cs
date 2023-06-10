using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace FaviconFetcher
{
    public class IconImage : IDisposable
    {
        private SKBitmap _bitmap;

        public IconImage() {
            Size = IconSize.Empty;
        }

        public SKBitmap ToSKBitmap()
        {
            return _bitmap;
        }

        public static IconImage FromSKBitmap(SKBitmap bitmap, IconSize size = null)
        {
            var icon = new IconImage();

            if (bitmap != null)
            {
                if (size == null)
                    icon._bitmap = bitmap;
                else
                    icon._bitmap = bitmap.Resize(new SKSizeI(size.Width, size.Height), SKFilterQuality.High);

                icon.Size = new IconSize(icon._bitmap.Width, icon._bitmap.Height);
            }

            return icon;
        }

        public static IconImage FromStream(Stream stream, IconSize size = null)
        {
            var icon = new IconImage();

            using (MemoryStream memStream = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(memStream);

                memStream.Position = 0; // Reset to start for decode
                var deccodedBitmap = SKBitmap.Decode(memStream);

                return FromSKBitmap(deccodedBitmap, size);
            }
        }

        public static IconImage FromIco(Stream stream, IconSize size = null)
        {
            return FromStream(stream, size);
        }

        public static IconImage FromFile(string filename, IconSize size = null)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return FromStream(fs, size);
            }
        }

        public void Dispose()
        {
            _bitmap = null;
            Size = null;
        }

        public IconSize Size { get; set; }

        public byte[] Bytes
        {
            get
            {
                return _bitmap.Bytes;
            }
        }

        public bool Save(string filename)
        {
            try
            {
                using (FileStream fs = File.Create(filename))
                {
                    SKData d = SKImage.FromBitmap(_bitmap).Encode(SKEncodedImageFormat.Png, 100);
                    d.SaveTo(fs);
                }

                return File.Exists(filename);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
