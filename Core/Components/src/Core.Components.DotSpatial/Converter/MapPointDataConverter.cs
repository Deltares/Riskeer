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

using System.Collections.Generic;
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Style;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPointData"/> into a <see cref="IMapFeatureLayer"/>
    /// containing one or more <see cref="Coordinate"/>.
    /// </summary>
    public class MapPointDataConverter : MapDataConverter<MapPointData>
    {
        protected override IMapFeatureLayer Convert(MapPointData data)
        {
            var featureSet = new FeatureSet(FeatureType.Point);

            foreach (var ringtoetsMapFeature in data.Features)
            {
                foreach (var feature in GetAllMapFeatureCoordinates(ringtoetsMapFeature)
                    .Select(c => new Feature(new Point(c.X, c.Y), featureSet)))
                {
                    AddMetaDataAsAttributes(ringtoetsMapFeature, featureSet, feature);
                }
            }

            featureSet.InitializeVertices();

            var layer = new MapPointLayer(featureSet)
            {
                IsVisible = data.IsVisible,
                Name = data.Name,
                ShowLabels = data.ShowLabels,
                LabelLayer = GetLabelLayer(featureSet, data.ShowLabels, data.SelectedMetaDataAttribute)
            };

            CreateStyle(layer, data.Style);

            return layer;
        }

        private static IEnumerable<Coordinate> GetAllMapFeatureCoordinates(MapFeature feature)
        {
            return feature.MapGeometries.SelectMany(mapGeometry => ConvertPoint2DElementsToCoordinates(mapGeometry.PointCollections.First()));
        }

        private static void CreateStyle(MapPointLayer layer, PointStyle style)
        {
            if (style != null)
            {
                layer.Symbolizer = new PointSymbolizer(style.Color, MapDataHelper.Convert(style.Symbol), style.Size);
            }
        }
    }
}