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

// ********************************************************************************************************
// Product Name: DotSpatial.Plugins.BruTileLayer
// Description:  Adds BruTile functionality to DotSpatial
// ********************************************************************************************************
// The contents of this file are subject to the GNU Library General Public License (LGPL). 
// You may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://dotspatial.codeplex.com/license
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and 
// limitations under the License. 
//
// The Initial Developer of this Original Code is Felix Obermaier. Created 2011.01.05 
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
// |      Name            |    Date     |                                Comments
// |----------------------|-------------|-----------------------------------------------------------------
// | Felix Obermaier      |  2011.01.05 | Initial commit
// | Felix Obermaier      |  2013.02.15 | All sorts of configuration code and user interfaces,
// |                      |             | making BruTileLayer work in sync- or async mode
// | Felix Obermaier      |  2013.09.19 | Making deserialization work by adding AssemblyResolve event,
// |                      |             | handler
// -------------------------------------------------------------------------------------------------------
// Changes upon original work have been implemented to suit functional requirements for Ringtoets.
// These changes have been implemented as of 13 Januari 2017.
// ********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using BruTile;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.DotSpatial.Layer.BruTile.Projections;
using Core.Components.DotSpatial.Layer.BruTile.TileFetching;
using DotSpatial.Controls;
using DotSpatial.Projections;
using DotSpatial.Projections.AuthorityCodes;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using BruTileExtent = BruTile.Extent;
using DotSpatialExtent = DotSpatial.Data.Extent;
using DotSpatialLayer = DotSpatial.Symbology.Layer;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Layer.BruTile
{
    /// <summary>
    /// A <see cref="BruTileLayer"/> based on a images from some source (on- or offline).
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/BruTileLayer.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public class BruTileLayer : DotSpatialLayer, IMapLayer
    {
        private const string webMercatorEpsgIdentifier = "EPSG:3857";
        private readonly IConfiguration configuration;

        private readonly AsyncTileFetcher tileFetcher;

        private readonly Stopwatch stopwatch = new Stopwatch();

        private readonly ImageAttributes imageAttributes;

        private readonly object drawLock = new object();

        /// <summary>
        /// The projection information of the data source captured by <see cref="configuration"/>.
        /// </summary>
        private readonly ProjectionInfo sourceProjection;

        /// <summary>
        /// The projection information of to which the data has been projected to. This value
        /// is <c>null</c> if the layer hasn't been reprojected.
        /// </summary>
        private ProjectionInfo targetProjection;

        private float transparency;

        private string level;

        /// <summary>
        /// Creates an instance of this class using some tile source configuration.
        /// </summary>
        /// <param name="configuration">The tile source configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CannotFindTileSourceException">Thrown when no tile source
        /// can be found based on <paramref name="configuration"/>.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// prevents the creation of the persistent tile cache.</exception>
        /// <exception cref="CannotReceiveTilesException">Thrown when it no tiles can be
        /// received from the tile source.</exception>
        public BruTileLayer(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!configuration.Initialized)
            {
                configuration.Initialize();
            }
            this.configuration = configuration;

            ITileSchema tileSchema = configuration.TileSource.Schema;
            sourceProjection = GetTileSourceProjectionInfo(tileSchema.Srs);

            Projection = sourceProjection;
            BruTileExtent extent = tileSchema.Extent;
            MyExtent = new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);

            LegendText = configuration.LegendText;
            LegendItemVisible = true;
            LegendSymbolMode = SymbolMode.Symbol;
            LegendType = LegendType.Custom;

            IsVisible = Checked = true;

            tileFetcher = configuration.TileFetcher;
            tileFetcher.TileReceived += HandleTileReceived;
            tileFetcher.QueueEmpty += HandleQueueEmpty;

            // Set the wrap mode
            imageAttributes = new ImageAttributes();
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
        }

        /// <summary>
        /// Gets or sets the transparency of the layer.
        /// </summary>
        /// <remarks>A value of 0 means that the layer's fully visible and a value of 1
        /// means that the layer to no longer visible.</remarks>
        public float Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                if (value < 0f || value > 1f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          "Transparanties moet in het bereik [0.0, 1.0] liggen.");
                }

                if (!Equals(value, transparency))
                {
                    transparency = value;
                    float alphaValue = 1f - transparency; // Remember: Alpha == opacity!
                    var ca = new ColorMatrix
                    {
                        Matrix33 = alphaValue
                    };
                    imageAttributes.SetColorMatrix(ca, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    OnItemChanged(EventArgs.Empty);
                    MapFrame?.Invalidate(MapFrame.ViewExtents);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="DotSpatialExtent"/> in world coordinates of this layer.
        /// </summary>
        public override DotSpatialExtent Extent
        {
            get
            {
                return targetProjection != null
                           ? MyExtent.Reproject(sourceProjection, targetProjection)
                           : MyExtent;
            }
        }

        public object Clone()
        {
            return new BruTileLayer(configuration.Clone())
            {
                Transparency = Transparency
            };
        }

        public override void Reproject(ProjectionInfo targetProjectionInfo)
        {
            base.Reproject(targetProjectionInfo);

            if (targetProjectionInfo != null)
            {
                targetProjection = targetProjectionInfo.Matches(sourceProjection) ?
                                       null :
                                       targetProjectionInfo;
            }
            else
            {
                targetProjection = null;
            }

            Projection = targetProjection ?? sourceProjection;
        }

        public void DrawRegions(MapArgs args, List<DotSpatialExtent> regions)
        {
            // If this layer is not marked visible, exit
            if (!IsVisible)
            {
                return;
            }

            stopwatch.Reset();

            DotSpatialExtent region = regions.FirstOrDefault() ?? args.GeographicExtents;

            if (!Monitor.TryEnter(drawLock))
            {
                return;
            }

            // If the layer is reprojected, we should take that into account for the geographic region:
            DotSpatialExtent geoExtent = targetProjection == null
                                             ? region
                                             : region.Intersection(Extent)
                                                     .Reproject(targetProjection, sourceProjection);

            if (geoExtent.IsEmpty())
            {
                Monitor.Exit(drawLock);
                return;
            }

            BruTileExtent extent;
            try
            {
                extent = ToBrutileExtent(geoExtent);
            }
            catch (Exception ex)
            {
                Monitor.Exit(drawLock);
                return;
            }

            if (double.IsNaN(extent.Area))
            {
                Monitor.Exit(drawLock);
                return;
            }

            double distancePerPixel = extent.Width/args.ImageRectangle.Width;

            ITileSource tileSource = configuration.TileSource;
            ITileSchema schema = tileSource.Schema;
            string level = this.level = Utilities.GetNearestLevel(schema.Resolutions, distancePerPixel);

            tileFetcher.DropAllPendingTileRequests();
            var tiles = new List<TileInfo>(Sort(schema.GetTileInfos(extent, level), geoExtent.Center));
            var tilesNotImmediatelyDrawn = new List<TileInfo>();

            // Set up Tile reprojector
            var reprojector = new TileReprojector(args, sourceProjection, targetProjection);

            // Store the current transformation
            Matrix transform = args.Device.Transform;
            Resolution resolution = schema.Resolutions[level];
            foreach (TileInfo info in tiles)
            {
                byte[] imageData = tileFetcher.GetTile(info);
                if (imageData != null)
                {
                    DrawTile(args, info, resolution, imageData, reprojector);
                    continue;
                }

                tilesNotImmediatelyDrawn.Add(info);
            }

            // Draw the tiles that were not present at the moment requested
            foreach (var tileInfo in tilesNotImmediatelyDrawn)
            {
                DrawTile(args, tileInfo, resolution, tileFetcher.GetTile(tileInfo), reprojector);
            }

            // Restore the transform
            args.Device.Transform = transform;

            Monitor.Exit(drawLock);
        }

        protected override void Dispose(bool managedResources)
        {
            if (IsDisposeLocked)
            {
                return;
            }

            if (managedResources)
            {
                configuration.TileFetcher.Dispose();
                imageAttributes.Dispose();
            }

            base.Dispose(managedResources);
        }

        private static ProjectionInfo GetTileSourceProjectionInfo(string spatialReferenceSystemString)
        {
            ProjectionInfo projectionInfo;
            // For WMTS, 'spatialReferenceSystemString' might be some crude value (urn-string):
            string authorityCode = ToAuthorityCode(spatialReferenceSystemString);
            if (!string.IsNullOrWhiteSpace(authorityCode))
            {
                projectionInfo = AuthorityCodeHandler.Instance[authorityCode];
            }
            else
            {
                ProjectionInfo p;
                if (!TryParseProjectionEsri(spatialReferenceSystemString, out p))
                {
                    if (!TryParseProjectionProj4(spatialReferenceSystemString, out p))
                    {
                        p = null;
                    }
                }
                projectionInfo = p;
            }

            if (projectionInfo == null)
            {
                projectionInfo = AuthorityCodeHandler.Instance[webMercatorEpsgIdentifier];
            }

            // WebMercator: set datum to WGS1984 for better accuracy 
            if (spatialReferenceSystemString == webMercatorEpsgIdentifier)
            {
                projectionInfo.GeographicInfo.Datum = KnownCoordinateSystems.Geographic.World.WGS1984.GeographicInfo.Datum;
            }

            return projectionInfo;
        }

        private static bool TryParseProjectionProj4(string proj4, out ProjectionInfo projectionInfo)
        {
            try
            {
                projectionInfo = ProjectionInfo.FromProj4String(proj4);
            }
            catch
            {
                projectionInfo = null;
            }
            return projectionInfo != null;
        }

        private static bool TryParseProjectionEsri(string esriWkt, out ProjectionInfo projectionInfo)
        {
            try
            {
                projectionInfo = ProjectionInfo.FromEsriString(esriWkt);
            }
            catch
            {
                projectionInfo = null;
            }
            return projectionInfo != null;
        }

        /// <summary>
        /// Attempts to get the EPSG:IdNumber part from a spatial reference system string.
        /// </summary>
        /// <param name="srs">The spatial reference system string.</param>
        /// <returns>The EPSG authority code, or an empty string if one couldn't be found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="srs"/> is
        /// <c>null</c> or contains only whitespace.</exception>
        private static string ToAuthorityCode(string srs)
        {
            if (string.IsNullOrWhiteSpace(srs))
            {
                throw new ArgumentNullException(nameof(srs));
            }

            const char srsSeparatorChar = ':';
            if (!srs.Contains(srsSeparatorChar))
            {
                int value;
                if (!int.TryParse(srs, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value))
                {
                    return "";
                }
                return $"EPSG:{value}";
            }

            string[] srsParts = srs.Split(srsSeparatorChar);

            // One colon => assume Authority:Code
            if (srsParts.Length == 2)
            {
                return srs;
            }

            // More than 1 colon => assume urn:ogc:def:crs:EPSG:6.18.3:3857
            if (srsParts.Length > 4)
            {
                return $"{srsParts[4]}:{srsParts.Last()}";
            }

            return "";
        }

        private void HandleTileReceived(object sender, TileReceivedEventArgs e)
        {
            TileIndex i = e.TileInfo.Index;
            if (i.Level != level)
            {
                return;
            }

            // Some timed refreshes if the server becomes slooow...
            if (stopwatch.Elapsed.Milliseconds > 250 && !tileFetcher.IsReady())
            {
                stopwatch.Reset();
                MapFrame.Invalidate();
                stopwatch.Restart();
                return;
            }

            Extent ext = ToBrutileExtent(MapFrame.ViewExtents);
            if (ext.Intersects(e.TileInfo.Extent))
            {
                MapFrame.Invalidate(FromBruTileExtent(e.TileInfo.Extent));
            }
        }

        private void HandleQueueEmpty(object sender, EventArgs empty)
        {
            stopwatch.Reset();
            MapFrame.Invalidate();
        }

        private static DotSpatialExtent FromBruTileExtent(BruTileExtent extent)
        {
            return new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }

        private static BruTileExtent ToBrutileExtent(DotSpatialExtent extent)
        {
            return new BruTileExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }

        private static IEnumerable<TileInfo> Sort(IEnumerable<TileInfo> enumerable, Coordinate coordinate)
        {
            var res = new SortedList<double, TileInfo>();
            foreach (var tileInfo in enumerable)
            {
                double dx = coordinate.X - tileInfo.Extent.CenterX;
                double dy = coordinate.Y - tileInfo.Extent.CenterY;
                double d = Math.Sqrt(dx*dx + dy*dy);
                while (res.ContainsKey(d))
                {
                    d *= 1e-12;
                }
                res.Add(d, tileInfo);
            }
            return res.Values;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DrawTile(MapArgs args, TileInfo info, Resolution resolution, byte[] buffer, TileReprojector reprojector = null)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return;
            }

            reprojector = reprojector ?? new TileReprojector(args, sourceProjection, targetProjection);

            using (var bitmap = (Bitmap) Image.FromStream(new MemoryStream(buffer)))
            {
                WorldFile inWorldFile = new WorldFile(resolution.UnitsPerPixel, 0,
                                                      0, -resolution.UnitsPerPixel,
                                                      info.Extent.MinX, info.Extent.MaxY);

                WorldFile outWorldFile;
                Bitmap outBitmap;

                reprojector.Reproject(inWorldFile, bitmap, out outWorldFile, out outBitmap);
                if (outWorldFile == null)
                {
                    return;
                }

                Point lt = args.ProjToPixel(outWorldFile.ToWorldCoordinates(0, 0));
                Point rb = args.ProjToPixel(outWorldFile.ToWorldCoordinates(outBitmap.Width, outBitmap.Height));
                var rect = new Rectangle(lt, Size.Subtract(new Size(rb), new Size(lt)));

                args.Device.DrawImage(outBitmap, rect, 0, 0, outBitmap.Width, outBitmap.Height,
                                      GraphicsUnit.Pixel, imageAttributes);

                if (outBitmap != bitmap)
                {
                    outBitmap.Dispose();
                }
            }
        }
    }
}