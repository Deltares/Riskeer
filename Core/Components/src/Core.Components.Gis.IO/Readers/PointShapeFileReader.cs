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
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
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
        }

        public override FeatureBasedMapData ReadShapeFile(string name = null)
        {
            List<IFeature> featureList = new List<IFeature>();
            while (readIndex != GetNumberOfLines())
            {
                featureList.Add(ReadFeatureLine());
            }

            return ConvertPointFeaturesToMapPointData(featureList, !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PointShapeFileReader_ReadLine_Points);
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

        public override FeatureBasedMapData ReadLine(string name = null)
        {
            if (readIndex == GetNumberOfLines())
            {
                return null;
            }

            try
            {
                IFeature pointFeature = GetFeature(readIndex);
                return ConvertPointFeatureToMapPointData(pointFeature, !string.IsNullOrWhiteSpace(name) ? name : GisIOResources.PointShapeFileReader_ReadLine_Points);
            }
            finally
            {
                readIndex++;
            }
        }

        public override IFeature GetFeature(int index)
        {
            IFeature pointFeature = ShapeFile.Features[index];
            return pointFeature;
        }

        private FeatureBasedMapData ConvertPointFeatureToMapPointData(IFeature pointFeature, string name)
        {
            var feature = new MapFeature(pointFeature.Coordinates.Select(c => new MapGeometry(new List<Point2D>
            {
                new Point2D(c.X, c.Y)
            })));

            return new MapPointData(new List<MapFeature>
            {
                feature
            }, name);
        }

        private FeatureBasedMapData ConvertPointFeaturesToMapPointData(List<IFeature> featureList, string name)
        {
            var mapFeatureList = new List<MapFeature>();
            foreach (var feature in featureList)
            {
                var f = new MapFeature(feature.Coordinates.Select(c => new MapGeometry(new List<Point2D>
                {
                    new Point2D(c.X, c.Y)
                })));

                mapFeatureList.Add(f);
            }

            return new MapPointData(mapFeatureList, name);
        }
    }
}