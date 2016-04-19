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

using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;

using DotSpatial.Data;
using DotSpatial.Topology;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GisIOResources = Core.Components.Gis.IO.Properties.Resources;

namespace Core.Components.Gis.IO.Readers
{
    /// <summary>
    /// Class to be used to read polygons from a shapefile.
    /// </summary>
    public class PolygonShapeFileReader : ShapeFileReaderBase
    {
        private int readIndex;

        /// <summary>
        /// Creates a new instance of <see cref="PolygonShapeFileReader"/>.
        /// </summary>
        /// <param name="filePath">The path to the shape file.</param>
        /// <exception cref="ArgumentException">When <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">When either:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-polygon geometries in it.</item>
        /// </list>
        /// </exception>
        public PolygonShapeFileReader(string filePath) : base(filePath)
        {
            try
            {
                ShapeFile = new PolygonShapefile(filePath);
            }
            catch (ArgumentException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(GisIOResources.PointShapeFileReader_File_contains_geometries_not_polygons);
                throw new CriticalFileReadException(message, e);
            }
        }

        public override FeatureBasedMapData ReadLine(string name = null)
        {
            if (readIndex == GetNumberOfLines())
            {
                return null;
            }

            try
            {
                IFeature polygonFeature = GetFeature(readIndex);
                string featureName = GetFeatureName(name);
                return ConvertPolygonFeatureToMapPolygonData(polygonFeature, featureName);
            }
            finally
            {
                readIndex++;
            }
        }

        public override FeatureBasedMapData ReadShapeFile(string name = null)
        {
            List<IFeature> featureList = new List<IFeature>();
            while (readIndex != GetNumberOfLines())
            {
                featureList.Add(ReadFeatureLine());
            }

            string featureName = GetFeatureName(name);
            return ConvertPolygonFeaturesToMapPointData(featureList, featureName);
        }

        public override IFeature GetFeature(int index)
        {
            return ShapeFile.Features[index];
        }

        private static string GetFeatureName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PolygonShapeFileReader_ReadLine_Polygon;
        }

        private IFeature ReadFeatureLine()
        {
            try
            {
                return GetFeature(readIndex);
            }
            finally
            {
                readIndex++;
            }
        }

        private FeatureBasedMapData ConvertPolygonFeatureToMapPolygonData(IFeature polygonFeature, string name)
        {
            var mapFeature = CreateMapFeatureForPolygonFeature(polygonFeature);
            IEnumerable<MapFeature> mapFeatures = new[]
            {
                mapFeature
            };
            return new MapPolygonData(mapFeatures, name);
        }

        private FeatureBasedMapData ConvertPolygonFeaturesToMapPointData(IEnumerable<IFeature> featureList, string name)
        {
            var mapFeatures = featureList.Select(CreateMapFeatureForPolygonFeature);
            return new MapPolygonData(mapFeatures, name);
        }

        private static MapFeature CreateMapFeatureForPolygonFeature(IFeature polygonFeature)
        {
            var geometries = new List<MapGeometry>();

            for (int i = 0; i < polygonFeature.BasicGeometry.NumGeometries; i++)
            {
                var basicPolygon = (IBasicPolygon)polygonFeature.BasicGeometry.GetBasicGeometryN(i);

                MapGeometry mapGeometry = new MapGeometry(GetMapGeometryPointCollections(basicPolygon));
                geometries.Add(mapGeometry);
            }

            return new MapFeature(geometries);
        }

        private static IEnumerable<IEnumerable<Point2D>> GetMapGeometryPointCollections(IBasicPolygon polygon)
        {
            yield return polygon.Shell.Coordinates.Select(c => new Point2D(c.X, c.Y));
            foreach (IBasicLineString hole in polygon.Holes)
            {
                yield return hole.Coordinates.Select(c => new Point2D(c.X, c.Y));
            }
        }
    }
}