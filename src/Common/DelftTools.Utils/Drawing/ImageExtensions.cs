using System.Drawing;

namespace DelftTools.Utils.Drawing
{
    public static class ImageExtensions // TODO: move to helpers / utils
    {
        public static bool PixelsEqual(this Image image1, Image image2)
        {
            if ((image1 == null && image2 != null) || (image1 != null && image2 == null))
            {
                return false;
            }

            if (image1 == image2)
            {
                return true;
            }

            if (!(image1 is Bitmap) || !(image2 is Bitmap))
            {
                return false;
            }

            var bitmap1 = (Bitmap) image1;
            var bitmap2 = (Bitmap) image2;

            if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
            {
                return false;
            }

            for (var i = 0; i < bitmap1.Width; i++)
            {
                for (var j = 0; j < bitmap1.Height; j++)
                {
                    if (!bitmap1.GetPixel(i, j).Equals(bitmap2.GetPixel(i, j)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Draws <paramref name="overlayImage"/> on top of <paramref name="originalImage"/> at the provided x and y offset
        /// </summary>
        /// <param name="originalImage">Original image</param>
        /// <param name="overlayImage">Overlay image</param>
        /// <param name="xOffSet">X offset for overlay drawing</param>
        /// <param name="yOffSet">Y offset for overlay drawing</param>
        public static Image AddOverlayImage(this Image originalImage, Image overlayImage, int xOffSet, int yOffSet)
        {
            var image = (Image) originalImage.Clone();

            using (var gfx = Graphics.FromImage(image))
            {
                gfx.DrawImage(overlayImage, new Point(xOffSet, yOffSet));
            }

            return image;
        }

        /// <summary>
        /// Draws <paramref name="overlayImage"/> on top of <paramref name="originalImage"/> at the provided x and y offset
        /// and scales <paramref name="overlayImage"/> down/up to the provided <paramref name="width"/> and <paramref name="height"/>
        /// </summary>
        /// <param name="originalImage">Original image</param>
        /// <param name="overlayImage">Overlay image</param>
        /// <param name="xOffSet">X offset for overlay drawing</param>
        /// <param name="yOffSet">Y offset for overlay drawing</param>
        /// <param name="width">Width for <paramref name="overlayImage"/></param>
        /// /// <param name="height">Height for <paramref name="overlayImage"/></param>
        public static T AddOverlayImage<T>(this T originalImage, Image overlayImage, int xOffSet, int yOffSet, int width, int height) where T : Image
        {
            var image = (T) originalImage.Clone();

            using (var gfx = Graphics.FromImage(image))
            {
                gfx.DrawImage(overlayImage, new Rectangle(new Point(xOffSet, yOffSet), new Size(width, height)));
            }

            return image;
        }
    }
}