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

using System.Collections.Generic;
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPointData"/> data into <see cref="MapPointLayer"/> data.
    /// </summary>
    public class MapPointDataConverter : FeatureBasedMapDataConverter<MapPointData, MapPointLayer>
    {
        protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
        {
            return GetAllMapFeatureCoordinates(mapFeature).Select(c => new Feature(new Point(c.X, c.Y)));
        }

        protected override IFeatureSymbolizer CreateSymbolizer(MapPointData mapData)
        {
            return mapData.Style != null
                       ? new PointSymbolizer(mapData.Style.Color, MapDataHelper.Convert(mapData.Style.Symbol), mapData.Style.Size)
                       : new PointSymbolizer();
        }

        private static IEnumerable<Coordinate> GetAllMapFeatureCoordinates(MapFeature feature)
        {
            return feature.MapGeometries.SelectMany(mapGeometry => ConvertPoint2DElementsToCoordinates(mapGeometry.PointCollections.First()));
        }
    }
}