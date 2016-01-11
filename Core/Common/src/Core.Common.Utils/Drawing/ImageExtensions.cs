using System.Drawing;

namespace Core.Common.Utils.Drawing
{
    /// <summary>
    /// This class holds extension methods for <see cref="Image"/> and derived classes.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Draws <paramref name="overlayImage"/> on top of <paramref name="originalImage"/> at the rectangle location.
        /// </summary>
        /// <param name="originalImage">The image to be changed.</param>
        /// <param name="overlayImage">The image that should be drawn over <paramref name="originalImage"/>.</param>
        /// <param name="xOffSet">The X coordinate of the upper-left corner for defining overlay placement.</param>
        /// <param name="yOffSet">The Y coordinate of the upper-left corner for defining overlay placement.</param>
        /// <param name="width">The width of the overlay.</param>
        /// <param name="height">The height of the overlay.</param>
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