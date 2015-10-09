using System.Collections.Generic;
using System.Drawing;

namespace SharpMap.Editors
{
    public static class TrackerSymbolHelper
    {
        private static readonly Dictionary<TrackerSymbolHelperArgumentKey, Bitmap> BitmapCache = new Dictionary<TrackerSymbolHelperArgumentKey, Bitmap>();

        /// <summary>
        /// GenerateSimpleTrackerImage creates a rectangular image. Please note
        /// the offset of 2 pixels to counter a mismath in sharpmap?
        /// </summary>
        public static Bitmap GenerateSimple(Pen pen, Brush brush, int width, int height)
        {
            Bitmap bmp;
            var key = new TrackerSymbolHelperArgumentKey(pen, brush, -1, -1, width, height);
            if (!BitmapCache.TryGetValue(key, out bmp))
            {
                bmp = GenerateSimpleCore(pen, brush, width, height);
                BitmapCache.Add(key, bmp);
            }
            return bmp;
        }

        public static Bitmap GenerateComposite(Pen pen, Brush brush, int totalWidth, int totaHeight, int width,
                                               int height)
        {
            Bitmap bmp;
            var key = new TrackerSymbolHelperArgumentKey(pen, brush, totalWidth, totaHeight, width, height);
            if (!BitmapCache.TryGetValue(key, out bmp))
            {
                bmp = GenerateCompositeCore(pen, brush, totalWidth, totaHeight, width, height);
                BitmapCache.Add(key, bmp);
            }
            return bmp;
        }

        private static Bitmap GenerateSimpleCore(Pen pen, Brush brush, int width, int height)
        {
            var bm = new Bitmap(width + 2, height + 2);

            using (var graphics = Graphics.FromImage(bm))
            {
                graphics.FillRectangle(brush, new Rectangle(2, 2, width, height));
                graphics.DrawRectangle(pen, new Rectangle(2, 2, width - 1, height - 1));
            }

            return bm;
        }

        private static Bitmap GenerateCompositeCore(Pen pen, Brush brush, int totalWidth, int totaHeight, int width, int height)
        {
            var bm = new Bitmap(totalWidth + 2, totaHeight + 2);

            using (var graphics = Graphics.FromImage(bm))
            {
                graphics.DrawRectangle(pen, new Rectangle(2, 2, totalWidth - 1, totaHeight - 1));

                graphics.FillRectangle(brush, new Rectangle(2, 2, width, height));
                graphics.DrawRectangle(pen, new Rectangle(2, 2, width - 1, height - 1));

                graphics.FillRectangle(brush, new Rectangle(totalWidth - width + 2, 2, width, height));
                graphics.DrawRectangle(pen, new Rectangle(totalWidth - width + 2, 2, width - 1, height - 1));

                graphics.FillRectangle(brush, new Rectangle(totalWidth - width + 2, totaHeight - height + 2, width, height));
                graphics.DrawRectangle(pen, new Rectangle(totalWidth - width + 2, totaHeight - height + 2, width - 1, height - 1));

                graphics.FillRectangle(brush, new Rectangle(2, totaHeight - height + 2, width, height));
                graphics.DrawRectangle(pen, new Rectangle(2, totaHeight - height + 2, width - 1, height - 1));
            }

            return bm;
        }

        private class TrackerSymbolHelperArgumentKey
        {
            private readonly int totalWidth;
            private readonly int totaHeight;
            private readonly int width;
            private readonly int height;
            private readonly string penString;
            private readonly string brushString;

            public TrackerSymbolHelperArgumentKey(Pen pen, Brush brush, int totalWidth, int totaHeight, int width, int height)
            {
                this.totalWidth = totalWidth;
                this.totaHeight = totaHeight;
                this.width = width;
                this.height = height;
                penString = pen.Color.ToString();
                brushString = brush is SolidBrush ? ((SolidBrush) brush).Color.ToString() : brush.ToString();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((TrackerSymbolHelperArgumentKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = totalWidth;
                    hashCode = (hashCode*397) ^ totaHeight;
                    hashCode = (hashCode*397) ^ width;
                    hashCode = (hashCode*397) ^ height;
                    hashCode = (hashCode*397) ^ (penString != null ? penString.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (brushString != null ? brushString.GetHashCode() : 0);
                    return hashCode;
                }
            }

            private bool Equals(TrackerSymbolHelperArgumentKey other)
            {
                return totalWidth == other.totalWidth &&
                       totaHeight == other.totaHeight &&
                       width == other.width &&
                       height == other.height &&
                       string.Equals(penString, other.penString) &&
                       string.Equals(brushString, other.brushString);
            }
        }
    }
}