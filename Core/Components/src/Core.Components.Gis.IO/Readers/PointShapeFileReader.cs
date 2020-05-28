// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Util.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Data;
using GeoAPI.Geometries;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;
using GisIOResources = Core.Components.Gis.IO.Properties.Resources;

namespace Core.Components.Gis.IO.Readers
{
    /// <summary>
    /// Class to be used to read points from a shapefile.
    /// </summary>
    public class PointShapeFileReader : ShapeFileReaderBase
    {
        private int readIndex;

        /// <summary>
        /// Creates a new instance of <see cref="PointShapeFileReader"/>.
        /// </summary>
        /// <param name="filePath">The path to the shape file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-point geometries in it.</item>
        /// <item>An unexpected error occurred when reading the shapefile.</item>
        /// </list>
        /// </exception>
        public PointShapeFileReader(string filePath) : base(filePath)
        {
            try
            {
                ShapeFile = new PointShapefile(filePath);
            }
            catch (ApplicationException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(GisIOResources.PointShapeFileReader_File_contains_geometries_not_points);
                throw new CriticalFileReadException(message, e);
            }
            catch (ArgumentException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(GisIOResources.PointShapeFileReader_File_contains_geometries_not_points);
                throw new CriticalFileReadException(message, e);
            }
            catch (IOException exception)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(CoreCommonUtilResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, exception);
            }
        }

        public override FeatureBasedMapData ReadShapeFile(string name = null)
        {
            var featureList = new List<IFeature>();
            while (readIndex != GetNumberOfFeatures())
            {
                featureList.Add(ReadFeatureLine());
            }

            string mapFeatureName = !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PointShapeFileReader_ReadLine_Points;
            return ConvertPointFeaturesToMapPointData(featureList, mapFeatureName);
        }

        public override FeatureBasedMapData ReadFeature(string name = null)
        {
            if (readIndex == GetNumberOfFeatures())
            {
                return null;
            }

            try
            {
                IFeature pointFeature = GetFeature(readIndex);
                string mapFeatureName = !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PointShapeFileReader_ReadLine_Points;
                return ConvertPointFeatureToMapPointData(pointFeature, mapFeatureName);
            }
            finally
            {
                readIndex++;
            }
        }

        public override IFeature GetFeature(int index)
        {
            return ShapeFile.Features[index];
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

        private FeatureBasedMapData ConvertPointFeatureToMapPointData(IFeature pointFeature, string name)
        {
            MapFeature feature = CreateMapFeatureForPointFeature(pointFeature);

            CopyMetaDataIntoFeature(feature, readIndex);

            return new MapPointData(name)
            {
                Features = new[]
                {
                    feature
                }
            };
        }

        private FeatureBasedMapData ConvertPointFeaturesToMapPointData(IEnumerable<IFeature> featureList, string name)
        {
            int featureListCount = featureList.Count();
            var mapFeatures = new MapFeature[featureListCount];

            for (var i = 0; i < featureListCount; i++)
            {
                IFeature feature = featureList.ElementAt(i);
                MapFeature mapFeature = CreateMapFeatureForPointFeature(feature);

                CopyMetaDataIntoFeature(mapFeature, i);

                mapFeatures[i] = mapFeature;
            }

            var mapPointData = new MapPointData(name)
            {
                Features = mapFeatures
            };
            mapPointData.SelectedMetaDataAttribute = mapPointData.MetaData.FirstOrDefault();

            return mapPointData;
        }

        private static MapFeature CreateMapFeatureForPointFeature(IFeature pointFeature)
        {
            IEnumerable<MapGeometry> mapGeometries = pointFeature.Geometry.Coordinates.Select(c => new MapGeometry(GetMapGeometryPointCollections(c))).ToArray();
            return new MapFeature(mapGeometries);
        }

        private static IEnumerable<IEnumerable<Point2D>> GetMapGeometryPointCollections(Coordinate c)
        {
            return new[]
            {
                new[]
                {
                    new Point2D(c.X, c.Y)
                }
            };
        }
    }
}