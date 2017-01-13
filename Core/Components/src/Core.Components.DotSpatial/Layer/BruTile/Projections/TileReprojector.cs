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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using DotSpatialReproject = DotSpatial.Projections.Reproject;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Layer.BruTile.Projections
{
    /// <summary>
    /// Class responsible for projecting maptiles from one coordinate system to another.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/TileReprojector.cs
    /// </remarks>
    internal class TileReprojector
    {
        private readonly ProjectionInfo source;
        private readonly ProjectionInfo target;
        private readonly MapArgs mapArgs;

        /// <summary>
        /// Creates a new instance of <see cref="TileReprojector"/>.
        /// </summary>
        /// <param name="mapArgs">The graphics info of the map.</param>
        /// <param name="source">The coordinate system information of the tile source.</param>
        /// <param name="target">The coordinate system information to project to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapArgs"/>
        /// is <c>null</c>.</exception>
        public TileReprojector(MapArgs mapArgs, ProjectionInfo source, ProjectionInfo target)
        {
            if (mapArgs == null)
            {
                throw new ArgumentNullException(nameof(mapArgs));
            }

            this.source = source;
            this.target = target;
            this.mapArgs = mapArgs;
        }

        /// <summary>
        /// Reprojects a <see cref="Bitmap"/> and corresponding georeferenced metadata to
        /// a new coordinate system.
        /// </summary>
        /// <param name="sourceReference">The georeferenced metadata of <paramref name="sourceTile"/>.</param>
        /// <param name="sourceTile">The tile image.</param>
        /// <param name="targetReference">Output: The reprojected georeferenced metadata.</param>
        /// <param name="targetTile">Output: The reprojected tile image.</param>
        public void Reproject(WorldFile sourceReference, Bitmap sourceTile, out WorldFile targetReference, out Bitmap targetTile)
        {
            if (source == null || target == null || source.Equals(target))
            {
                targetReference = sourceReference;
                targetTile = sourceTile;
                return;
            }

            Rectangle ptRect = GetSourceExtentInPixelCoordinates(sourceReference, sourceTile);

            // Get the intersection with the current viewport
            ptRect.Intersect(mapArgs.ImageRectangle);

            // Is it empty, don't return anything
            if (ptRect.Width == 0 || ptRect.Height == 0)
            {
                targetTile = null;
                targetReference = null;
                return;
            }

            int offsetX = ptRect.X;
            int offsetY = ptRect.Y;

            // Prepare the result tile
            targetTile = new Bitmap(ptRect.Size.Width, ptRect.Size.Height,
                                    PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(targetTile))
            {
                g.Clear(Color.Transparent);
            }

            ColorAccess sourceTileColorAccess = ColorAccess.Create(sourceTile);
            ColorAccess targetTileColorAccess = ColorAccess.Create(targetTile);

            // Copy values to output buffer
            Size sourceTileSize = sourceTile.Size;
            for (var i = 0; i < targetTile.Height; i++)
            {
                foreach (Tuple<Point, Point> ppair in
                    GetValidPoints(offsetY + i, offsetY, offsetX, offsetX + targetTile.Width, sourceReference, sourceTileSize))
                {
                    Color c = sourceTileColorAccess[ppair.Item1.X, ppair.Item1.Y];
                    targetTileColorAccess[ppair.Item2.X, ppair.Item2.Y] = c;
                }
            }
            // Copy to output tile
            SetBitmapBuffer(targetTile, targetTileColorAccess.Buffer);

            // Compute the reference
            Extent outExtent = mapArgs.PixelToProj(ptRect);
            targetReference = new WorldFile(
                outExtent.Width/ptRect.Width, 0,
                0, -outExtent.Height/ptRect.Height,
                outExtent.X, outExtent.Y);
        }

        private Rectangle GetSourceExtentInPixelCoordinates(WorldFile sourceReference, Bitmap sourceTile)
        {
            // Bounding polygon on the ground
            IPolygon ps = sourceReference.ToGroundBounds(sourceTile.Width, sourceTile.Height);

            // Bounding polygon on the ground in target projection
            ILinearRing pt = ps.Shell.Reproject(source, target);

            // The target extent
            Extent ptExtent = pt.EnvelopeInternal.ToExtent();

            // The target extent projected to the current viewport
            Rectangle ptRect = mapArgs.ProjToPixel(ptExtent);
            return ptRect;
        }

        private IEnumerable<Tuple<Point, Point>> GetValidPoints(int y, int offsetY, int x1, int x2,
                                                                WorldFile sourceReference, Size sourceTileSize)
        {
            int len = (x2 - x1);
            double[] xy = new double[len*2];
            int i = 0;
            for (int x = x1; x < x2; x++)
            {
                Coordinate c = mapArgs.PixelToProj(new Point(x, y));
                xy[i++] = c.X;
                xy[i++] = c.Y;
            }

            DotSpatialReproject.ReprojectPoints(xy, null, target, source, 0, len);

            i = 0;
            y -= offsetY;
            for (var x = 0; x < len; x++)
            {
                Coordinate coord = new Coordinate(xy[i++], xy[i++]);
                Point sourcePixelLocation = sourceReference.ToRaster(coord);

                if (IsSourcePointInsideArea(sourceTileSize, sourcePixelLocation))
                {
                    yield return Tuple.Create(sourcePixelLocation, new Point(x, y));
                }
            }
        }

        private static void SetBitmapBuffer(Bitmap bitmap, byte[] buffer)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
            bitmap.UnlockBits(bitmapData);
        }

        private static bool IsSourcePointInsideArea(Size area, Point sourcePixelLocation)
        {
            if (sourcePixelLocation.X < 0 || sourcePixelLocation.Y < 0)
            {
                return false;
            }
            if (sourcePixelLocation.X < area.Width)
            {
                if (sourcePixelLocation.Y < area.Height)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Facade class for easy access to the color data of a <see cref="Bitmap"/>.
        /// </summary>
        private class ColorAccess
        {
            private static readonly byte[] bitMask;

            static ColorAccess()
            {
                bitMask = new[]
                {
                    (byte) 1,
                    (byte) 2,
                    (byte) 4,
                    (byte) 8,
                    (byte) 16,
                    (byte) 32,
                    (byte) 64,
                    (byte) 128
                };
            }

            private readonly int stride;

            private readonly PixelFormat format;
            private readonly int bitsPerPixel;

            private readonly ColorPalette palette;

            private ColorAccess(BitmapData data, ColorPalette palette)
            {
                stride = data.Stride;

                Buffer = new byte[stride*data.Height];
                Marshal.Copy(data.Scan0, Buffer, 0, Buffer.Length);
                format = data.PixelFormat;
                bitsPerPixel = GetPixelSize(format);
                this.palette = palette;
            }

            public Color this[int x, int y]
            {
                get
                {
                    int mod;
                    int pIndex;
                    var index = GetIndex(x, y, out mod);
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

                        case PixelFormat.Format32bppRgb:
                            return Color.FromArgb(Buffer[index + 3], Buffer[index + 2],
                                                  Buffer[index + 1], Buffer[index]);
                    }
                    return Color.Transparent;
                }

                set
                {
                    if (format != PixelFormat.Format32bppArgb)
                    {
                        throw new NotSupportedException();
                    }

                    int mod;
                    int index = GetIndex(x, y, out mod);
                    Buffer[index++] = value.B;
                    Buffer[index++] = value.G;
                    Buffer[index++] = value.R;
                    Buffer[index] = value.A;
                }
            }

            public byte[] Buffer { get; }

            public static ColorAccess Create(Bitmap bitmap, Rectangle? rect = null)
            {
                if (rect == null)
                {
                    rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                }
                BitmapData bmData = bitmap.LockBits(rect.Value, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                ColorAccess res = new ColorAccess(bmData, bitmap.Palette);
                bitmap.UnlockBits(bmData);

                return res;
            }

            private int GetIndex(int x, int y, out int mod)
            {
                int offsetRow = y*stride;
                int offsetColBits = x*bitsPerPixel;
                int offsetCol = offsetColBits/8;
                mod = offsetColBits - offsetCol*8;

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
}