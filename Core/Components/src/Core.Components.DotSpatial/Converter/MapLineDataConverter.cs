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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapLineData"/> data into <see cref="MapLineLayer"/> data.
    /// </summary>
    public class MapLineDataConverter : FeatureBasedMapDataConverter<MapLineData, MapLineLayer>
    {
        protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
        {
            yield return new Feature(GetGeometry(mapFeature));
        }

        protected override IFeatureSymbolizer CreateSymbolizer(MapLineData mapData)
        {
            return new LineSymbolizer(mapData.Style.Color,
                                      mapData.Style.Color,
                                      mapData.Style.Width,
                                      MapDataHelper.Convert(mapData.Style.DashStyle),
                                      LineCap.Round);
        }

        protected override IFeatureScheme CreateScheme()
        {
            return new LineScheme();
        }

        protected override IFeatureCategory CreateDefaultCategory(MapLineData mapData)
        {
            return CreateCategory(mapData, mapData.Style.Color);
        }

        protected override IFeatureCategory CreateCategory(MapLineData mapData, Color color)
        {
            return new LineCategory(color,
                                    color,
                                    mapData.Style.Width,
                                    MapDataHelper.Convert(mapData.Style.DashStyle),
                                    LineCap.Round);
        }

        private static IBasicGeometry GetGeometry(MapFeature mapFeature)
        {
            var factory = new GeometryFactory();

            if (mapFeature.MapGeometries.Count() > 1)
            {
                return factory.CreateMultiLineString(mapFeature.MapGeometries
                                                               .Select(mg => GetLineString(factory, mg.PointCollections.First()))
                                                               .ToArray());
            }

            var pointsToConvert = new Point2D[0];
            if (mapFeature.MapGeometries.Count() == 1)
            {
                pointsToConvert = mapFeature.MapGeometries.First().PointCollections.First().ToArray();
            }

            return GetLineString(factory, pointsToConvert);
        }

        private static IBasicLineString GetLineString(IGeometryFactory factory, IEnumerable<Point2D> points)
        {
            return factory.CreateLineString(ConvertPoint2DElementsToCoordinates(points).ToArray());
        }
    }
}