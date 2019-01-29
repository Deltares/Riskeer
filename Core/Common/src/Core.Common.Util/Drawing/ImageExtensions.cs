// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Core.Common.Util.Drawing
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
        /// <param name="xOffset">The X-coordinate of the upper-left corner for defining overlay placement.</param>
        /// <param name="yOffset">The Y-coordinate of the upper-left corner for defining overlay placement.</param>
        /// <param name="width">The width of the overlay.</param>
        /// <param name="height">The height of the overlay.</param>
        public static T AddOverlayImage<T>(this T originalImage, Image overlayImage, int xOffset, int yOffset, int width, int height) where T : Image
        {
            var image = (T) originalImage.Clone();

            using (Graphics gfx = Graphics.FromImage(image))
            {
                gfx.DrawImage(overlayImage, new Rectangle(new Point(xOffset, yOffset), new Size(width, height)));
            }

            return image;
        }

        /// <summary>
        /// Gets a <see cref="BitmapImage"/> representation of an <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image to get a <see cref="BitmapImage"/> for.</param>
        /// <returns>A <see cref="BitmapImage"/>.</returns>
        public static BitmapImage AsBitmapImage(this Image image)
        {
            var memoryStream = new MemoryStream();

            ((Bitmap) image).Save(memoryStream, ImageFormat.Png);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}