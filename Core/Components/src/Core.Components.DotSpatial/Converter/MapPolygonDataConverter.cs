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
    /// The converter that converts <see cref="MapPolygonData"/> into a <see cref="MapPolygonLayer"/>.
    /// </summary>
    public class MapPolygonDataConverter : FeatureBasedMapDataConverter<MapPolygonData, MapPolygonLayer>
    {
        protected override IMapFeatureLayer CreateLayer()
        {
            return new MapPolygonLayer();
        }

        protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
        {
            var geometryList = new List<IPolygon>();

            foreach (var mapGeometry in mapFeature.MapGeometries)
            {
                IEnumerable<Point2D>[] pointCollections = mapGeometry.PointCollections.ToArray();

                IEnumerable<Coordinate> outerRingCoordinates = ConvertPoint2DElementsToCoordinates(pointCollections[0]);
                ILinearRing outerRing = new LinearRing(outerRingCoordinates);

                ILinearRing[] innerRings = new ILinearRing[pointCollections.Length - 1];
                for (int i = 1; i < pointCollections.Length; i++)
                {
                    IEnumerable<Coordinate> innerRingCoordinates = ConvertPoint2DElementsToCoordinates(pointCollections[i]);
                    innerRings[i - 1] = new LinearRing(innerRingCoordinates);
                }

                IPolygon polygon = new Polygon(outerRing, innerRings);
                geometryList.Add(polygon);
            }

            yield return new Feature(GetGeometry(geometryList));
        }

        protected override void ConvertLayerFeatures(MapPolygonData data, MapPolygonLayer layer)
        {
            var columnNameLookup = GetColumnNameLookup(data);

            foreach (var mapFeature in data.Features)
            {
                var geometryList = new List<IPolygon>();

                foreach (var mapGeometry in mapFeature.MapGeometries)
                {
                    IEnumerable<Point2D>[] pointCollections = mapGeometry.PointCollections.ToArray();

                    IEnumerable<Coordinate> outerRingCoordinates = ConvertPoint2DElementsToCoordinates(pointCollections[0]);
                    ILinearRing outerRing = new LinearRing(outerRingCoordinates);

                    ILinearRing[] innerRings = new ILinearRing[pointCollections.Length - 1];
                    for (int i = 1; i < pointCollections.Length; i++)
                    {
                        IEnumerable<Coordinate> innerRingCoordinates = ConvertPoint2DElementsToCoordinates(pointCollections[i]);
                        innerRings[i - 1] = new LinearRing(innerRingCoordinates);
                    }

                    IPolygon polygon = new Polygon(outerRing, innerRings);
                    geometryList.Add(polygon);
                }

                var feature = new Feature(GetGeometry(geometryList), layer.FeatureSet);

                foreach (var attribute in mapFeature.MetaData)
                {
                    feature.DataRow[columnNameLookup[attribute.Key].ToString()] = attribute.Value;
                }
            }
        }

        protected override void ConvertLayerProperties(MapPolygonData data, MapPolygonLayer layer)
        {
            layer.IsVisible = data.IsVisible;
            layer.Name = data.Name;
            layer.ShowLabels = data.ShowLabels;
            layer.LabelLayer = GetLabelLayer(GetColumnNameLookup(data), layer.FeatureSet, data.ShowLabels, data.SelectedMetaDataAttribute);

            if (data.Style != null)
            {
                layer.Symbolizer = new PolygonSymbolizer(data.Style.FillColor, data.Style.StrokeColor, data.Style.Width);
            }
        }

        private static IBasicGeometry GetGeometry(List<IPolygon> geometryList)
        {
            IBasicGeometry geometry;
            var factory = new GeometryFactory();

            if (geometryList.Count > 1)
            {
                geometry = factory.CreateMultiPolygon(geometryList.ToArray());
            }
            else
            {
                ILinearRing shell = null;
                var holes = new ILinearRing[0];

                if (geometryList.Count == 1)
                {
                    IPolygon polygon = geometryList.First();
                    shell = polygon.Shell;
                    holes = polygon.Holes;
                }

                geometry = factory.CreatePolygon(shell, holes);
            }
            return geometry;
        }
    }
}