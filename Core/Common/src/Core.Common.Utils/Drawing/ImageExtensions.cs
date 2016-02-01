// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

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