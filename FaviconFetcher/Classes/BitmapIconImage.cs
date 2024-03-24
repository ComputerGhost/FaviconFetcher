using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace FaviconFetcher
{
    public class BitmapIconImage : IconImage
    {
        private SKBitmap _bitmap;

        public byte[] Bytes
        {
            get
            {
                return _bitmap.Bytes;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BitmapIconImage() : base() { }

        /// <summary>
        /// Creates an IconImage from a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap data for the IconImage</param>
        /// <param name="size">The size of the returned IconImage. If not provided, the 
        /// images original dimensions will be used.</param>
        /// <returns>IconImage</returns>
        public new static IconImage FromSKBitmap(SKBitmap bitmap, IconSize size = null)
        {
            var bitmapIconImage = new BitmapIconImage();
            if (bitmap != null)
            {
                if (size == null)
                    bitmapIconImage._bitmap = bitmap;
                else
                    bitmapIconImage._bitmap = bitmap.Resize(new SKSizeI(size.Width, size.Height), SKFilterQuality.High);

                bitmapIconImage.Size = new IconSize(bitmapIconImage._bitmap.Width, bitmapIconImage._bitmap.Height);
            }

            return bitmapIconImage;
        }

        /// <summary>
        /// Creates an IconImage from a stream.
        /// </summary>
        /// <param name="stream">The stream to read the image data from.</param>
        /// <param name="size">The size of the returned IconImage. If not provided, the 
        /// images original dimensions will be used.</param>
        /// <returns>SVGIconImage</returns>
        public new static IconImage FromStream(Stream stream, IconSize size = null)
        {
            SKBitmap decodedBitmap;

            using (MemoryStream memStream = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(memStream);

                memStream.Position = 0; // Reset to start for decode
                decodedBitmap = SKBitmap.Decode(memStream);
                return FromSKBitmap(decodedBitmap, size);
            }
        }

        /// <summary>
        /// Creates an IconImage from an ICO file.  Internally, simply uses FromStream.
        /// </summary>
        /// <param name="stream">The stream to read the image data from.</param>
        /// <param name="size">The size of the returned IconImage. If not provided, the 
        /// images original dimensions will be used.</param>
        /// <returns>SVGIconImage</returns>
        public static IconImage FromIco(Stream stream, IconSize size = null)
        {
            return FromStream(stream, size);
        }

        /// <summary>
        /// Creates an IconImage from a file.
        /// </summary>
        /// <param name="filename">The full file path and name of the data to be read.</param>
        /// <param name="size">The size of the returned IconImage. If not provided, the 
        /// images original dimensions will be used.</param>
        /// <returns>IconImage</returns>
        public new static IconImage FromFile(string filename, IconSize size = null)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return FromStream(fs, size);
            }
        }

        /// <summary>
        /// Saves IconImage to disk in PNG form.
        /// </summary>
        /// <param name="filename">The full path and filename to write the IconImage to.</param>
        /// <param name="size">The size of the output IconImage.  Icon will be scaled proportionally
        /// to fit.  If no size is specified, the icon will be written at its original size.</param>
        /// <returns>True if the save was successfull and the file exists.</returns>
        public override bool Save(string filename, IconSize size = null)
        {
            try
            {
                using (FileStream fs = File.Create(filename))
                {
                    if (size != null && (_bitmap.Width != size.Width || _bitmap.Height != size.Height))
                    {
                        var scaleFactor = Math.Min((double)size.Width / _bitmap.Width, (double)size.Height / _bitmap.Height);
                        var width = (int)(_bitmap.Width * scaleFactor);
                        var height = (int)(_bitmap.Height * scaleFactor);

                        _bitmap = _bitmap.Resize(new SKSizeI(width, height), SKFilterQuality.High);
                    }

                    SKData d = _bitmap.Encode(SKEncodedImageFormat.Png, 100);
                    d.SaveTo(fs);
                }

                return File.Exists(filename);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the IconImage to a bitmap at the size specified by the IconImage's Size property.
        /// </summary>
        /// <returns>SKBitmap</returns>
        public override SKBitmap ToSKBitmap()
        {
            return _bitmap;
        }

        /// <summary>
        /// Disposes the IconImage
        /// </summary>
        public override void Dispose()
        {
            _bitmap = null;

            base.Dispose();
        }
    }
}
