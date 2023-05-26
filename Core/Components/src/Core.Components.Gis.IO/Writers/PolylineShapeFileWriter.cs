﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Properties;
using DotSpatial.Data;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace Core.Components.Gis.IO.Writers
{
    /// <summary>
    /// Class to be used to write polylines to a shapefile.
    /// </summary>
    public class PolylineShapeFileWriter : ShapeFileWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineShapeFileWriter"/> class.
        /// </summary>
        public PolylineShapeFileWriter()
        {
            ShapeFile = new LineShapefile
            {
                FeatureType = FeatureType.Line
            };
        }

        protected override IFeature AddFeature(MapFeature mapFeature)
        {
            LineString lineString = CreateLineStringFromMapFeature(mapFeature);

            return ShapeFile.AddFeature(lineString);
        }

        private static LineString CreateLineStringFromMapFeature(MapFeature mapFeature)
        {
            if (mapFeature.MapGeometries.Count() != 1)
            {
                throw new ArgumentException(Resources.PointShapeFileWriter_CreatePointFromMapFeature_A_feature_can_only_contain_one_geometry);
            }

            MapGeometry geometry = mapFeature.MapGeometries.First();

            IEnumerable<Point2D> mapGeometryPointCollection = geometry.PointCollections.First();

            Coordinate[] coordinates = mapGeometryPointCollection.Select(p => new Coordinate(p.X, p.Y)).ToArray();

            return new LineString(coordinates);
        }
    }
}