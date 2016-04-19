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
using System.Drawing.Drawing2D;
using System.Linq;

using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPolygonData"/> into a <see cref="IMapFeatureLayer"/> containing a <see cref="Polygon"/>.
    /// </summary>
    public class MapPolygonDataConverter : MapDataConverter<MapPolygonData>
    {
        protected override IList<IMapFeatureLayer> Convert(MapPolygonData data)
        {
            var featureSet = new FeatureSet(FeatureType.Polygon);

            foreach (var mapFeature in data.Features)
            {
                var feature = new Feature();
                var geometryList = new List<IPolygon>();
                
                foreach (var mapGeometry in mapFeature.MapGeometries)
                {
                    var coordinates = mapGeometry.PointCollections.First().Select(p => new Coordinate(p.X, p.Y));
                    IPolygon polygon = new Polygon(coordinates);
                    geometryList.Add(polygon);
                }

                GeometryFactory factory = new GeometryFactory();
                feature.BasicGeometry = factory.CreateMultiPolygon(geometryList.ToArray());
               
                featureSet.Features.Add(feature);
            }

            var layer = new MapPolygonLayer(featureSet)
            {
                IsVisible = data.IsVisible,
                Name = data.Name
            };

            CreateStyle(layer, data.Style);

            return new List<IMapFeatureLayer>
            {
                layer
            };
        }

        private void CreateStyle(MapPolygonLayer layer, PolygonStyle style)
        {
            if (style != null)
            {
                layer.Symbolizer = new PolygonSymbolizer(style.FillColor, style.StrokeColor, style.Width);
            }
        }
    }
}