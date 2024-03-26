using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaviconFetcher
{
    public class IconSize : EqualityComparer<IconSize>, IEquatable<IconSize>
    {
        /// <summary>
        /// A width or height value indicating that the size is scaleable to any size
        /// </summary>
        public static int SCALEABLE_SIZE = -1;

        private SKSizeI _size;

        public IconSize()
        {
            _size = SKSizeI.Empty;
        }

        public IconSize(int width, int height)
        {
            _size = new SKSizeI(width, height);
        }

        public int Width
        {
            get
            {
                return _size.Width;
            }
            set
            {
                _size.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return _size.Height;
            }
            set
            {
                _size.Height = value;
            }
        }

        public static IconSize Empty
        {
            get
            {
                return new IconSize();
            }
        }

        /// <summary>
        /// An IconSize representing a scaleable vector icon
        /// </summary>
        public static IconSize Scaleable
        {
            get
            {
                return new IconSize(SCALEABLE_SIZE, SCALEABLE_SIZE);
            }
        }

        public bool Equals(IconSize other)
        {
            if (other == null) return false;

            return other.Width == this.Width && other.Height == this.Height;
        }

        public static bool operator == (IconSize lhs, IconSize rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if (ReferenceEquals(rhs, null)) return false;
            if (ReferenceEquals(null, lhs)) return false;

            if (lhs == null) { return rhs == null; }
            if (rhs == null) { return lhs == null; }

            return rhs.Width == lhs.Width && rhs.Height == lhs.Height;
        }

        public static bool operator != (IconSize lhs, IconSize rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IconSize);
        }

        public override bool Equals(IconSize x, IconSize y)
        {
            return x == y;
        }

        public override int GetHashCode(IconSize obj)
        {
            throw new NotImplementedException();
        }
    }
}
