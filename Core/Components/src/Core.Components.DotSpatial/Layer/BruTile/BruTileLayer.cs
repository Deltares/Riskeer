// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BruTile;
using Core.Common.Base.Data;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.DotSpatial.Projections;
using Core.Components.DotSpatial.Properties;
using Core.Components.Gis.Exceptions;
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
        private static readonly ProjectionInfo defaultProjection = new ProjectionInfo();

        private static readonly Range<float> transparencyValidityRange = new Range<float>(0f, 1f);
        private readonly IConfiguration configuration;

        private readonly ITileFetcher tileFetcher;

        private readonly ImageAttributes imageAttributes;

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
        /// <param name="configuration">The tile source configuration. If the configuration
        /// hasn't been initialized yet, <see cref="IConfiguration.Initialize"/> will
        /// be called.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CannotFindTileSourceException">Thrown when <paramref name="configuration"/>
        /// is not initialized and during initialization no tile source can be found.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when <paramref name="configuration"/>
        /// is not initialized and during initialization a critical error prevents the creation
        ///  of the persistent tile cache.</exception>
        /// <exception cref="CannotReceiveTilesException">Thrown when <paramref name="configuration"/>
        /// is not initialized and during initialization no tiles can be received from the tile source.</exception>
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

            ITileSchema tileSchema = configuration.TileSchema;
            sourceProjection = GetTileSourceProjectionInfo(tileSchema.Srs);

            Projection = sourceProjection;
            BruTileExtent extent = tileSchema.Extent;
            MyExtent = new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);

            IsVisible = true;

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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value outside
        /// the [0.0, 1.0] range.</exception>
        public float Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                if (!transparencyValidityRange.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          string.Format(Resources.BruTileLayer_Transparency_Value_out_of_Range_0_,
                                                                        transparencyValidityRange.ToString("0.0", CultureInfo.CurrentCulture)));
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

                    OnItemChanged(this);
                    MapFrame?.Invalidate(MapFrame.ViewExtents);
                }
            }
        }

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
            if (targetProjectionInfo != null)
            {
                targetProjection = targetProjectionInfo.Equals(sourceProjection)
                                       ? null
                                       : targetProjectionInfo;
            }
            else
            {
                targetProjection = null;
            }

            Projection = targetProjection ?? sourceProjection;
        }

        public void DrawRegions(MapArgs args, List<DotSpatialExtent> regions)
        {
            if (!IsVisible)
            {
                return;
            }

            IEnumerable<DotSpatialExtent> regionsToDraw = regions.Any()
                                                              ? (IEnumerable<DotSpatialExtent>) regions
                                                              : new[]
                                                              {
                                                                  args.GeographicExtents
                                                              };

            ITileSchema schema = configuration.TileSchema;
            foreach (DotSpatialExtent region in regionsToDraw)
            {
                DrawRegion(args, region, schema);
            }
        }

        protected override void Dispose(bool managedResources)
        {
            if (IsDisposeLocked)
            {
                return;
            }

            if (managedResources)
            {
                configuration.Dispose();
                imageAttributes.Dispose();
            }

            base.Dispose(managedResources);
        }

        private void DrawRegion(MapArgs args, DotSpatialExtent region, ITileSchema schema)
        {
            DotSpatialExtent geoExtent = GetExtentInTargetCoordinateSystem(region);

            BruTileExtent extent;
            if (GetBruTileExtentToRender(geoExtent, out extent))
            {
                double distancePerPixel = extent.Width / args.ImageRectangle.Width;
                level = Utilities.GetNearestLevel(schema.Resolutions, distancePerPixel);

                tileFetcher.DropAllPendingTileRequests();

                IEnumerable<TileInfo> tiles = Sort(schema.GetTileInfos(extent, level), geoExtent.Center);
                DrawTilesAtCurrentLevel(args, tiles, schema);
            }
        }

        private DotSpatialExtent GetExtentInTargetCoordinateSystem(DotSpatialExtent region)
        {
            return targetProjection == null
                       ? region
                       : region.Intersection(Extent)
                               .Reproject(targetProjection, sourceProjection);
        }

        private static bool GetBruTileExtentToRender(DotSpatialExtent geoExtent, out BruTileExtent extent)
        {
            if (geoExtent.IsEmpty())
            {
                extent = new BruTileExtent();
                return false;
            }

            extent = ToBrutileExtent(geoExtent);
            return true;
        }

        private void DrawTilesAtCurrentLevel(MapArgs args, IEnumerable<TileInfo> tiles, ITileSchema schema)
        {
            var tilesNotImmediatelyDrawn = new List<TileInfo>();

            var reprojector = new TileReprojector(args, sourceProjection, targetProjection);
            Resolution resolution = schema.Resolutions[level];
            foreach (TileInfo info in tiles)
            {
                byte[] imageData = tileFetcher.GetTile(info);
                if (imageData != null)
                {
                    DrawTile(args, info, resolution, imageData, reprojector);
                }
                else
                {
                    tilesNotImmediatelyDrawn.Add(info);
                }
            }

            // Draw the tiles that were not present at the moment requested
            foreach (TileInfo tileInfo in tilesNotImmediatelyDrawn)
            {
                DrawTile(args, tileInfo, resolution, tileFetcher.GetTile(tileInfo), reprojector);
            }
        }

        private static ProjectionInfo GetTileSourceProjectionInfo(string spatialReferenceSystemString)
        {
            ProjectionInfo projectionInfo;
            if (!TryParseProjectionEsri(spatialReferenceSystemString, out projectionInfo)
                && !TryParseProjectionProj4(spatialReferenceSystemString, out projectionInfo))
            {
                // For WMTS, 'spatialReferenceSystemString' might be some crude value (urn-string):
                string authorityCode = ToAuthorityCode(spatialReferenceSystemString);
                projectionInfo = !string.IsNullOrWhiteSpace(authorityCode)
                                     ? AuthorityCodeHandler.Instance[authorityCode]
                                     : null;
            }

            if (projectionInfo == null)
            {
                projectionInfo = AuthorityCodeHandler.Instance[webMercatorEpsgIdentifier];
            }

            // WebMercator: set datum to WGS1984 for better accuracy 
            if (projectionInfo.Name == webMercatorEpsgIdentifier)
            {
                projectionInfo.GeographicInfo.Datum = KnownCoordinateSystems.Geographic.World.WGS1984.GeographicInfo.Datum;
            }

            return projectionInfo;
        }

        private static bool TryParseProjectionProj4(string proj4, out ProjectionInfo projectionInfo)
        {
            return TryParseString(proj4, ProjectionInfo.FromProj4String, out projectionInfo);
        }

        private static bool TryParseProjectionEsri(string esriWkt, out ProjectionInfo projectionInfo)
        {
            return TryParseString(esriWkt, ProjectionInfo.FromEsriString, out projectionInfo);
        }

        private static bool TryParseString(string text, Func<string, ProjectionInfo> parseText, out ProjectionInfo projectionInfo)
        {
            try
            {
                projectionInfo = parseText(text);
            }
            catch
            {
                projectionInfo = null;
            }

            projectionInfo = GetCompensatedValueForDefaultConstructedProjectionInfo(projectionInfo);

            return projectionInfo != null;
        }

        private static ProjectionInfo GetCompensatedValueForDefaultConstructedProjectionInfo(ProjectionInfo returnedProjection)
        {
            return defaultProjection.Equals(returnedProjection) ? null : returnedProjection;
        }

        /// <summary>
        /// Attempts to get the EPSG:IdNumber part from a spatial reference system string.
        /// </summary>
        /// <param name="srs">The spatial reference system string.</param>
        /// <returns>The EPSG authority code, or an empty string if one couldn't be found.</returns>
        private static string ToAuthorityCode(string srs)
        {
            if (!string.IsNullOrWhiteSpace(srs))
            {
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

                // More than 1 colon => assume format urn:ogc:def:crs:EPSG:6.18.3:3857
                if (srsParts.Length > 4)
                {
                    return $"{srsParts[4]}:{srsParts.Last()}";
                }
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

            DotSpatialExtent tileExtent = FromBruTileExtent(e.TileInfo.Extent);
            if (MapFrame.ViewExtents.Intersects(tileExtent))
            {
                MapFrame.Invalidate(tileExtent);
            }
        }

        private void HandleQueueEmpty(object sender, EventArgs empty)
        {
            MapFrame.Invalidate();
        }

        private static DotSpatialExtent FromBruTileExtent(BruTileExtent extent)
        {
            return new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }

        /// <summary>
        /// Converts from <see cref="DotSpatialExtent"/> (Dotspatial) to <see cref="BruTileExtent"/>
        /// (BruTile).
        /// </summary>
        /// <param name="extent">The extent to convert from.</param>
        /// <returns>The converted extent.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="extent"/>
        /// has <see cref="DotSpatialExtent.MinX"/> greater than <see cref="DotSpatialExtent.MaxX"/>
        /// or <see cref="DotSpatialExtent.MinY"/> greater than <see cref="DotSpatialExtent.MaxY"/>.
        /// </exception>
        private static BruTileExtent ToBrutileExtent(DotSpatialExtent extent)
        {
            return new BruTileExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }

        /// <summary>
        /// Sorts the available <see cref="TileInfo"/> with respect to distance to a given
        /// <see cref="Coordinate"/> in ascending order. 
        /// </summary>
        /// <param name="tileInfos">The <see cref="TileInfo"/> objects to be sorted.</param>
        /// <param name="focusPoint">The point used to order based on distance.</param>
        /// <returns></returns>
        /// <remarks>Subsequent equidistant tiles are ordered to occur first.</remarks>
        private static IEnumerable<TileInfo> Sort(IEnumerable<TileInfo> tileInfos, Coordinate focusPoint)
        {
            var sortResult = new SortedList<double, TileInfo>();
            foreach (TileInfo tileInfo in tileInfos)
            {
                BruTileExtent tileInfoExtent = tileInfo.Extent;
                var tileCenterCoordinate = new Coordinate(tileInfoExtent.CenterX, tileInfoExtent.CenterY);
                double distance = focusPoint.Distance(tileCenterCoordinate);
                while (sortResult.ContainsKey(distance))
                {
                    distance *= 1e-12;
                }

                sortResult.Add(distance, tileInfo);
            }

            return sortResult.Values;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DrawTile(MapArgs args, TileInfo info, Resolution resolution, byte[] buffer, TileReprojector reprojector)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return;
            }

            try
            {
                using (var bitmap = (Bitmap) Image.FromStream(new MemoryStream(buffer)))
                {
                    var inWorldFile = new WorldFile(resolution.UnitsPerPixel, 0,
                                                    0, -resolution.UnitsPerPixel,
                                                    info.Extent.MinX, info.Extent.MaxY);

                    WorldFile outWorldFile;
                    Bitmap outBitmap;

                    reprojector.Reproject(inWorldFile, bitmap, out outWorldFile, out outBitmap);
                    if (outWorldFile == null)
                    {
                        return;
                    }

                    Point leftTopPoint = args.ProjToPixel(outWorldFile.ToWorldCoordinates(0, 0));
                    Point rightBottomPoint = args.ProjToPixel(outWorldFile.ToWorldCoordinates(outBitmap.Width, outBitmap.Height));
                    var imageScreenBounds = new Rectangle(leftTopPoint, new Size(rightBottomPoint.X - leftTopPoint.X,
                                                                                 rightBottomPoint.Y - leftTopPoint.Y));

                    args.Device.DrawImage(outBitmap, imageScreenBounds, 0, 0, outBitmap.Width, outBitmap.Height,
                                          GraphicsUnit.Pixel, imageAttributes);

                    if (outBitmap != bitmap)
                    {
                        outBitmap.Dispose();
                    }
                }
            }
            catch (ArgumentException)
            {
                // Silently drop the corrupt image
            }
        }
    }
}