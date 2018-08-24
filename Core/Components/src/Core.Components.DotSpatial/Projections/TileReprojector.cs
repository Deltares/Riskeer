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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Common.Util.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using DotSpatialReproject = DotSpatial.Projections.Reproject;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Projections
{
    /// <summary>
    /// Class responsible for projecting maptiles from one coordinate system to another.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/TileReprojector.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
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

            Rectangle targetTileExtentInPixelCoordinates = GetTargetExtentInPixelCoordinates(sourceReference, sourceTile);

            // Get the intersection with the current viewport
            targetTileExtentInPixelCoordinates.Intersect(mapArgs.ImageRectangle);

            // Is it empty, don't return anything
            if (targetTileExtentInPixelCoordinates.Width == 0 || targetTileExtentInPixelCoordinates.Height == 0)
            {
                targetTile = null;
                targetReference = null;
                return;
            }

            int offsetX = targetTileExtentInPixelCoordinates.X;
            int offsetY = targetTileExtentInPixelCoordinates.Y;

            // Prepare the result tile
            targetTile = new Bitmap(targetTileExtentInPixelCoordinates.Size.Width, targetTileExtentInPixelCoordinates.Size.Height,
                                    PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(targetTile))
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
            targetTileColorAccess.SetBufferToImageAtOriginalLocation(targetTile);

            // Compute the reference
            Extent outExtent = mapArgs.PixelToProj(targetTileExtentInPixelCoordinates);
            targetReference = new WorldFile(
                outExtent.Width / targetTileExtentInPixelCoordinates.Width, 0,
                0, -outExtent.Height / targetTileExtentInPixelCoordinates.Height,
                outExtent.X, outExtent.Y);
        }

        private Rectangle GetTargetExtentInPixelCoordinates(WorldFile sourceReference, Bitmap sourceTile)
        {
            IPolygon sourceTileCircumference = sourceReference.BoundingOrdinatesToWorldCoordinates(sourceTile.Width, sourceTile.Height);
            ILinearRing targetTileCircumference = sourceTileCircumference.Shell.Reproject(source, target);
            Extent targetTileExtent = targetTileCircumference.EnvelopeInternal.ToExtent();
            return mapArgs.ProjToPixel(targetTileExtent);
        }

        private IEnumerable<Tuple<Point, Point>> GetValidPoints(int y, int offsetY, int x1, int x2,
                                                                WorldFile sourceReference, Size sourceTileSize)
        {
            int len = x2 - x1;
            var xy = new double[len * 2];
            var i = 0;
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
                var coord = new Coordinate(xy[i++], xy[i++]);
                Point sourcePixelLocation = sourceReference.ToScreenCoordinates(coord);

                if (IsSourcePointInsideArea(sourceTileSize, sourcePixelLocation))
                {
                    yield return Tuple.Create(sourcePixelLocation, new Point(x, y));
                }
            }
        }

        private static bool IsSourcePointInsideArea(Size area, Point sourcePixelLocation)
        {
            if (sourcePixelLocation.X < 0 || sourcePixelLocation.Y < 0)
            {
                return false;
            }

            if (sourcePixelLocation.X < area.Width && sourcePixelLocation.Y < area.Height)
            {
                return true;
            }

            return false;
        }
    }
}