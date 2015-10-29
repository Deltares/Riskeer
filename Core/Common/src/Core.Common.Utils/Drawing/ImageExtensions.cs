using System.Drawing;

namespace Core.Common.Utils.Drawing
{
    public static class ImageExtensions
    {
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