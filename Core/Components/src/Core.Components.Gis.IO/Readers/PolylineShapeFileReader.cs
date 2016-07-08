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
    /// Class to be used to read polylines from a shapefile.
    /// </summary>
    public class PolylineShapeFileReader : ShapeFileReaderBase
    {
        private int readIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineShapeFileReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shapefile path.</param>
        /// <exception cref="ArgumentException">When <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">When either:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-line geometries in it.</item>
        /// </list>
        /// </exception>
        public PolylineShapeFileReader(string shapeFilePath) : base(shapeFilePath)
        {
            try
            {
                ShapeFile = new LineShapefile(shapeFilePath);
            }
            catch (ArgumentException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(GisIOResources.LineShapeFileReader_File_contains_geometries_not_line);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Reads a line shape from the file.
        /// </summary>
        /// <returns>The <see cref="FeatureBasedMapData"/> representing the read line shape, or 
        /// <c>null</c> when at the end of the shapefile.</returns>
        public override FeatureBasedMapData ReadLine(string name = null)
        {
            if (readIndex == GetNumberOfLines())
            {
                return null;
            }

            try
            {
                IFeature lineFeature = GetFeature(readIndex);
                return ConvertSingleLineFeatureToMapLineData(lineFeature, !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PolylineShapeFileReader_ReadLine_Line);
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

            return ConvertMultiLineFeatureToMapLineData(featureList, !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PolylineShapeFileReader_ReadLine_Line);
        }

        /// <summary>
        /// Gets the single line feature at the given index.
        /// </summary>
        /// <param name="index">The index of which feature to retrieve.</param>
        /// <returns>The feature that consists out of 1 whole polyline.</returns>
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

        private MapLineData ConvertSingleLineFeatureToMapLineData(IFeature lineFeature, string name)
        {
            MapFeature feature = CreateMapFeatureForLineFeature(lineFeature);
            CopyMetaDataIntoFeature(feature, readIndex);

            IEnumerable<MapFeature> mapFeatures = new[]
            {
                feature
            };
            return new MapLineData(mapFeatures, name);
        }

        private MapLineData ConvertMultiLineFeatureToMapLineData(List<IFeature> lineFeatures, string name)
        {
            var mapFeatureList = new List<MapFeature>();
            for (int featureIndex = 0; featureIndex < lineFeatures.Count; featureIndex++)
            {
                IFeature lineFeature = lineFeatures[featureIndex];
                MapFeature feature = CreateMapFeatureForLineFeature(lineFeature);
                CopyMetaDataIntoFeature(feature, featureIndex);

                mapFeatureList.Add(feature);
            }

            return new MapLineData(mapFeatureList, name);
        }

        private static MapFeature CreateMapFeatureForLineFeature(IFeature lineFeature)
        {
            var geometries = new List<MapGeometry>();

            for (int i = 0; i < lineFeature.BasicGeometry.NumGeometries; i++)
            {
                var polylineGeometry = lineFeature.BasicGeometry.GetBasicGeometryN(i);

                MapGeometry mapGeometry = new MapGeometry(GetMapGeometryPointCollections(polylineGeometry.Coordinates));
                geometries.Add(mapGeometry);
            }

            return new MapFeature(geometries);
        }

        private static IEnumerable<Point2D>[] GetMapGeometryPointCollections(IEnumerable<Coordinate> lineCoordinates)
        {
            return new[]
            {
                lineCoordinates.Select(c => new Point2D(c.X, c.Y))
            };
        }
    }
}