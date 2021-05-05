// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing.Text;
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
        /// <returns>The created <see cref="Font"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fontData"/> is <c>null</c>.</exception>
        public static Font CreateFont(byte[] fontData)
        {
            if (fontData == null)
            {
                throw new ArgumentNullException(nameof(fontData));
            }

            uint dummy = 0;

            using (var fonts = new PrivateFontCollection())
            {
                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                fonts.AddMemoryFont(fontPtr, fontData.Length);
                AddFontMemResourceEx(fontPtr, (uint) fontData.Length, IntPtr.Zero, ref dummy);
                Marshal.FreeCoTaskMem(fontPtr);

                return new Font(fonts.Families[0], 14.0F);
            }
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
                                                          IntPtr pdv, [In] ref uint pcFonts);
    }
}