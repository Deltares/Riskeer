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
    /// The converter that converts <see cref="MapLineData"/> into a <see cref="MapLineLayer"/>.
    /// </summary>
    public class MapLineDataConverter : MapDataConverter<MapLineData, MapLineLayer>
    {
        protected override IMapFeatureLayer Convert(MapLineData data)
        {
            var layer = new MapLineLayer();

            ConvertLayerFeatures(data, layer);
            ConvertLayerProperties(data, layer);

            return layer;
        }

        protected override void ConvertLayerFeatures(MapLineData data, MapLineLayer layer)
        {
            layer.FeatureSet.Features.Clear();
            layer.FeatureSet.DataTable.Clear();

            foreach (MapFeature mapFeature in data.Features)
            {
                AddMetaDataAsAttributes(mapFeature, layer.FeatureSet, new Feature(GetGeometry(mapFeature), layer.FeatureSet));
            }

            layer.FeatureSet.InitializeVertices();
        }

        protected override void ConvertLayerProperties(MapLineData data, MapLineLayer layer)
        {
            layer.IsVisible = data.IsVisible;
            layer.Name = data.Name;
            layer.ShowLabels = data.ShowLabels;
            layer.LabelLayer = GetLabelLayer(layer.FeatureSet, data.ShowLabels, data.SelectedMetaDataAttribute);

            if (data.Style != null)
            {
                layer.Symbolizer = new LineSymbolizer(data.Style.Color, data.Style.Color, data.Style.Width, data.Style.Style, LineCap.Round);
            }
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
            return factory.CreateLineString(ConvertPoint2DElementsToCoordinates(points).ToList());
        }
    }
}