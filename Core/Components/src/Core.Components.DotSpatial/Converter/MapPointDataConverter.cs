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
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Point = DotSpatial.Topology.Point;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPointData"/> data into <see cref="MapPointLayer"/> data.
    /// </summary>
    public class MapPointDataConverter : FeatureBasedMapDataConverter<MapPointData, MapPointLayer>
    {
        protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
        {
            return GetAllMapFeatureCoordinates(mapFeature).Select(c => new Feature(new Point(c.X, c.Y))).ToArray();
        }

        protected override IFeatureSymbolizer CreateSymbolizer(MapPointData mapData)
        {
            return CreatePointSymbolizer(mapData);
        }

        protected override IFeatureScheme CreateScheme()
        {
            return new PointScheme();
        }

        protected override IFeatureCategory CreateDefaultCategory(MapPointData mapData)
        {
            return CreateCategory(mapData, mapData.Style.Color);
        }

        protected override IFeatureCategory CreateCategory(MapPointData mapData, Color color)
        {
            PointSymbolizer symbolizer = CreatePointSymbolizer(mapData);

            var category = new PointCategory(symbolizer);
            category.SetColor(color);

            return category;
        }

        private static PointSymbolizer CreatePointSymbolizer(MapPointData mapData)
        {
            var symbolizer = new PointSymbolizer(mapData.Style.Color, MapDataHelper.Convert(mapData.Style.Symbol), mapData.Style.Size);
            symbolizer.SetOutline(mapData.Style.StrokeColor, mapData.Style.StrokeThickness);
            return symbolizer;
        }

        private static IEnumerable<Coordinate> GetAllMapFeatureCoordinates(MapFeature feature)
        {
            return feature.MapGeometries.SelectMany(mapGeometry => ConvertPoint2DElementsToCoordinates(mapGeometry.PointCollections.First())).ToArray();
        }
    }
}