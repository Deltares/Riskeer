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
using LineStyle = Core.Components.Gis.Style.LineStyle;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapLineData"/> into a <see cref="IMapFeatureLayer"/>
    /// containing a <see cref="LineString"/>.
    /// </summary>
    public class MapLineDataConverter : MapDataConverter<MapLineData>
    {
        protected override IMapFeatureLayer Convert(MapLineData data)
        {
            var featureSet = new FeatureSet(FeatureType.Line);

            foreach (MapFeature mapFeature in data.Features)
            {
                AddMetaDataAsAttributes(mapFeature, featureSet, new Feature(GetGeometry(mapFeature), featureSet));
            }

            featureSet.InitializeVertices();

            var layer = new MapLineLayer(featureSet)
            {
                IsVisible = data.IsVisible,
                Name = data.Name,
                ShowLabels = data.ShowLabels,
                LabelLayer = GetLabelLayer(featureSet, data.ShowLabels, data.SelectedMetaDataAttribute)
            };

            CreateStyle(layer, data.Style);

            return layer;
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

            Point2D[] pointsToConvert =
            {};
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

        private static void CreateStyle(MapLineLayer layer, LineStyle style)
        {
            if (style != null)
            {
                layer.Symbolizer = new LineSymbolizer(style.Color, style.Color, style.Width, style.Style, LineCap.Round);
            }
        }
    }
}