﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    /// The converter that converts <see cref="MapPointData"/> into a <see cref="MapPointLayer"/>.
    /// </summary>
    public class MapPointDataConverter : MapDataConverter<MapPointData, MapPointLayer>
    {
        protected override IMapFeatureLayer Convert(MapPointData data)
        {
            var layer = new MapPointLayer();

            ConvertLayerFeatures(data, layer);
            ConvertLayerProperties(data, layer);

            return layer;
        }

        protected override void ConvertLayerFeatures(MapPointData data, MapPointLayer layer)
        {
            layer.FeatureSet.Features.Clear();
            layer.FeatureSet.DataTable.Clear();

            foreach (var ringtoetsMapFeature in data.Features)
            {
                foreach (var feature in GetAllMapFeatureCoordinates(ringtoetsMapFeature)
                    .Select(c => new Feature(new Point(c.X, c.Y), layer.FeatureSet)))
                {
                    AddMetaDataAsAttributes(ringtoetsMapFeature, layer.FeatureSet, feature);
                }
            }

            layer.FeatureSet.InitializeVertices();
        }

        protected override void ConvertLayerProperties(MapPointData data, MapPointLayer layer)
        {
            layer.IsVisible = data.IsVisible;
            layer.Name = data.Name;
            layer.ShowLabels = data.ShowLabels;
            layer.LabelLayer = GetLabelLayer(layer.FeatureSet, data.ShowLabels, data.SelectedMetaDataAttribute);

            if (data.Style != null)
            {
                layer.Symbolizer = new PointSymbolizer(data.Style.Color, MapDataHelper.Convert(data.Style.Symbol), data.Style.Size);
            }
        }

        private static IEnumerable<Coordinate> GetAllMapFeatureCoordinates(MapFeature feature)
        {
            return feature.MapGeometries.SelectMany(mapGeometry => ConvertPoint2DElementsToCoordinates(mapGeometry.PointCollections.First()));
        }
    }
}