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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Core.Common.Utils.Properties;

namespace Core.Common.Utils.Drawing
{
    /// <summary>
    /// Facade class for easy access to the color data of a <see cref="Bitmap"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/TileReprojector.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </para>
    /// <para>
    /// This class uses a copy of the image buffer. Therefore it isn't possible to
    /// use this class directly to change the source image.
    /// </para>
    /// </remarks>
    public class ColorAccess
    {
        private static readonly byte[] bitMask =
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128
        };

        private readonly int stride;

        private readonly PixelFormat format;
        private readonly int bitsPerPixel;

        private readonly ColorPalette palette;
        private readonly Rectangle validRange;

        private ColorAccess(BitmapData data, ColorPalette palette, Rectangle validRange)
        {
            stride = data.Stride;

            Buffer = new byte[stride * data.Height];
            Marshal.Copy(data.Scan0, Buffer, 0, Buffer.Length);
            format = data.PixelFormat;
            bitsPerPixel = GetPixelSize(format);
            this.palette = palette;
            this.validRange = validRange;
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> of the pixel at the given coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The color of the given pixel.</returns>
        public Color this[int x, int y]
        {
            get
            {
                ValidateIndices(x, y);

                int mod;
                int pIndex;
                int index = GetIndex(x, y, out mod);
                switch (format)
                {
                    case PixelFormat.Format1bppIndexed:
                        return palette.Entries[(Buffer[index] & bitMask[mod]) == 0 ? 0 : 1];

                    case PixelFormat.Format4bppIndexed:
                        pIndex = Buffer[index];
                        mod /= 4;
                        if (mod != 0)
                        {
                            pIndex >>= 4;
                        }
                        return palette.Entries[pIndex & 0x7];

                    case PixelFormat.Format8bppIndexed:
                        pIndex = Buffer[index];
                        return palette.Entries[pIndex];

                    case PixelFormat.Format24bppRgb:
                        return Color.FromArgb(Buffer[index + 2], Buffer[index + 1],
                                              Buffer[index]);

                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppRgb:
                        return Color.FromArgb(Buffer[index + 3], Buffer[index + 2],
                                              Buffer[index + 1], Buffer[index]);
                    default:
                        throw new InvalidOperationException($"Indexing image for image format '{format}' isn't supported.");
                }
            }
            set
            {
                if (format != PixelFormat.Format32bppArgb)
                {
                    throw new InvalidOperationException($"Setting image color for image format '{format}' isn't supported.");
                }
                ValidateIndices(x, y);

                int mod;
                int index = GetIndex(x, y, out mod);
                Buffer[index++] = value.B;
                Buffer[index++] = value.G;
                Buffer[index++] = value.R;
                Buffer[index] = value.A;
            }
        }

        /// <summary>
        /// The image data buffer. This can be changed with <see cref="Item(int,int)"/> and
        /// can be used to create a new image with the changed data (for example using
        /// <see cref="SetBufferToImageAtOriginalLocation"/>).
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Sets the current image <see cref="Buffer"/> back at the original location in
        /// an image.
        /// </summary>
        /// <param name="bitmap">The image to update.</param>
        /// <exception cref="Exception">Thrown when unable to access the image data of <paramref name="bitmap"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bitmap"/>
        /// is <c>null</c>.</exception>
        public void SetBufferToImageAtOriginalLocation(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            BitmapData bitmapData = bitmap.LockBits(validRange, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(Buffer, 0, bitmapData.Scan0, Buffer.Length);
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ColorAccess"/> for a given image.
        /// </summary>
        /// <param name="bitmap">The image to gain access to.</param>
        /// <param name="accessibleArea">Optional: The area of <paramref name="bitmap"/>
        /// to provide access to. When not specified, the whole image is accessible.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bitmap"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="accessibleArea"/>
        /// is specified and does not fully fit within the bounds of <paramref name="bitmap"/>.</exception>
        /// <exception cref="Exception">Thrown when unable to connect to the data buffers
        /// of <paramref name="bitmap"/>.</exception>
        public static ColorAccess Create(Bitmap bitmap, Rectangle? accessibleArea = null)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var fullBitmapArea = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            if (accessibleArea == null)
            {
                accessibleArea = fullBitmapArea;
            }
            else if (!fullBitmapArea.Contains(accessibleArea.Value))
            {
                throw new ArgumentException(Resources.ColorAccess_Create_Accessible_area_outside_image_bounds_error,
                                            nameof(accessibleArea));
            }

            BitmapData imageData = bitmap.LockBits(accessibleArea.Value, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var res = new ColorAccess(imageData, bitmap.Palette, accessibleArea.Value);
            bitmap.UnlockBits(imageData);

            return res;
        }

        private void ValidateIndices(int x, int y)
        {
            if (!IsInValidRange(x, y))
            {
                throw new ArgumentOutOfRangeException("indices", $"Index must be in range x:[{validRange.Left},{validRange.Right - 1}], y:[{validRange.Top},{validRange.Bottom - 1}].");
            }
        }

        private bool IsInValidRange(int x, int y)
        {
            return validRange.Contains(x, y);
        }

        private int GetIndex(int x, int y, out int mod)
        {
            int offsetRow = y * stride;
            int offsetColBits = x * bitsPerPixel;
            int offsetCol = offsetColBits / 8;
            mod = offsetColBits - offsetCol * 8;

            return offsetRow + offsetCol;
        }

        private static int GetPixelSize(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    return 8;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    return 16;
                case PixelFormat.Format24bppRgb:
                    return 24;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    return 32;
                case PixelFormat.Format48bppRgb:
                    return 48;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    return 64;
                case PixelFormat.Format4bppIndexed:
                    return 4;
                case PixelFormat.Format1bppIndexed:
                    return 1;
                default:
                    throw new InvalidEnumArgumentException(nameof(pixelFormat), (int) pixelFormat, typeof(PixelFormat));
            }
        }
    }
}