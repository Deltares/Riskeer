// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.Common.Util.Drawing
{
    /// <summary>
    /// This class defines extension methods for <see cref="Graphics"/>.
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Draws the specified image, using its original physical size at a given opacity,
        /// at the location specified by a coordinate pair.
        /// </summary>
        /// <param name="g">The graphics handle.</param>
        /// <param name="image">The image to be draw.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.</param>
        /// <param name="opacity">The opacity for the image.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="g"/> or
        /// <paramref name="image"/> is <c>null</c>.</exception>
        /// <seealso cref="Graphics.DrawImage(Image,int,int)"/>
        public static void DrawImageTransparent(this Graphics g, Image image, int x, int y, float opacity)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            int width = image.Width;
            int height = image.Height;
            g.DrawImage(image, new Rectangle(0, 0, width, height), x, y,
                        width, height, GraphicsUnit.Pixel,
                        CalculateOpacityImageAttributes(opacity));
        }

        private static ImageAttributes CalculateOpacityImageAttributes(float opacity)
        {
            ColorMatrix colorMatrix = CreateColorMatrixForOpacityChange(opacity);

            var imgAttributes = new ImageAttributes();
            imgAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            return imgAttributes;
        }

        private static ColorMatrix CreateColorMatrixForOpacityChange(float opacity)
        {
            var clippedOpacity = (float) Math.Min(1.0, Math.Max(0.0, opacity));

            // Define the color matrix to change the alpha value of the image, where 'x' is the opacity-factor.
            // 1 0 0 0 0
            // 0 1 0 0 0
            // 0 0 1 0 0
            // 0 0 0 x 0
            // 0 0 0 0 1
            float[][] ptsArray =
            {
                new[]
                {
                    1f,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    1f,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    1f,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    clippedOpacity,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    1f
                }
            };

            return new ColorMatrix(ptsArray);
        }
    }
}