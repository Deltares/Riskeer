﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace Core.Common.Util.Drawing
{
    /// <summary>
    /// Helper class for <see cref="Font"/> related logic.
    /// </summary>
    public static class FontHelper
    {
        /// <summary>
        /// Creates a <see cref="Font"/> based on the provided byte array.
        /// </summary>
        /// <param name="fontData">The data to create the <see cref="Font"/> from.</param>
        /// <param name="privateFontCollection">The container that will become the owner of the created <see cref="Font"/>.</param>
        /// <returns>The created <see cref="Font"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <remarks>The <paramref name="privateFontCollection"/> should be in scope (alive) at the calling side in order to prevent unintended garbage collection.</remarks>
        public static Font CreateFont(byte[] fontData, PrivateFontCollection privateFontCollection)
        {
            if (fontData == null)
            {
                throw new ArgumentNullException(nameof(fontData));
            }

            if (privateFontCollection == null)
            {
                throw new ArgumentNullException(nameof(privateFontCollection));
            }

            uint dummy = 0;

            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            privateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
            AddFontMemResourceEx(fontPtr, (uint) fontData.Length, IntPtr.Zero, ref dummy);
            Marshal.FreeCoTaskMem(fontPtr);

            return new Font(privateFontCollection.Families.Last(), 14.0F);
        }

        /// <summary>
        /// Creates a <see cref="Bitmap"/> from the given <paramref name="font"/> and <paramref name="unicode"/>.
        /// </summary>
        /// <param name="font">The font to use.</param>
        /// <param name="unicode">The unicode to create an image from.</param>
        /// <returns>A <see cref="Bitmap"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static Bitmap CreateBitmapFromFont(Font font, string unicode)
        {
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (unicode == null)
            {
                throw new ArgumentNullException(nameof(unicode));
            }

            var bitmap = new Bitmap(4096, 4096);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawString(unicode, new Font(font.FontFamily, 2600), Brushes.Black, 0, 0);
            }

            return bitmap;
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
                                                          IntPtr pdv, [In] ref uint pcFonts);
    }
}