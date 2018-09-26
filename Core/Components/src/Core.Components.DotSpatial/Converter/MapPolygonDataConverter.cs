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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPolygonData"/> data into <see cref="MapPolygonLayer"/> data.
    /// </summary>
    public class MapPolygonDataConverter : FeatureBasedMapDataConverter<MapPolygonData, MapPolygonLayer>
    {
        protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
        {
            var geometryList = new List<IPolygon>();

            foreach (MapGeometry mapGeometry in mapFeature.MapGeometries)
            {
                IEnumerable<Point2D>[] pointCollections = mapGeometry.PointCollections.ToArray();

                IEnumerable<Coordinate> outerRingCoordinates = ConvertPoint2DElementsToCoordinates(pointCollections[0]);
                ILinearRing outerRing = new LinearRing(outerRingCoordinates);

                var innerRings = new ILinearRing[pointCollections.Length - 1];
                for (var i = 1; i < pointCollections.Length; i++)
                {
                    IEnumerable<Coordinate> innerRingCoordinates = ConvertPoint2DElementsToCoordinates(pointCollections[i]);
                    innerRings[i - 1] = new LinearRing(innerRingCoordinates);
                }

                IPolygon polygon = new Polygon(outerRing, innerRings);
                geometryList.Add(polygon);
            }

            yield return new Feature(GetGeometry(geometryList));
        }

        protected override IFeatureSymbolizer CreateSymbolizer(MapPolygonData mapData)
        {
            return new PolygonSymbolizer(mapData.Style.FillColor, GetStrokeColor(mapData), mapData.Style.StrokeThickness);
        }

        protected override IFeatureScheme CreateScheme()
        {
            return new PolygonScheme();
        }

        protected override IFeatureCategory CreateDefaultCategory(MapPolygonData mapData)
        {
            return CreateCategory(mapData, mapData.Style.FillColor);
        }

        protected override IFeatureCategory CreateCategory(MapPolygonData mapData, Color color)
        {
            return new PolygonCategory(color, GetStrokeColor(mapData), mapData.Style.StrokeThickness);
        }

        private static Color GetStrokeColor(MapPolygonData mapData)
        {
            Color strokeColor = mapData.Style.StrokeColor;
            if (mapData.Style.StrokeThickness == 0)
            {
                strokeColor = Color.Transparent;
            }

            return strokeColor;
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