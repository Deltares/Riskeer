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
using System.Data;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using DotSpatial.Data;
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

        public override PointBasedMapData ReadLine(string name = null)
        {
            if (readIndex == GetNumberOfLines())
            {
                return null;
            }

            try
            {
                IFeature polygonFeature = GetFeature(readIndex);
                return ConvertPolygonFeatureToMapPolygonData(polygonFeature, name ?? GisIOResources.PolygonShapeFileReader_ReadLine_Polygon);
            }
            finally
            {
                readIndex++;
            }
        }

        public override IFeature GetFeature(int index)
        {
            IFeature polygonFeature = ShapeFile.Features[index];
            return polygonFeature;
        }

        private PointBasedMapData ConvertPolygonFeatureToMapPolygonData(IFeature polygonFeature, string name)
        {
            return new MapPolygonData(polygonFeature.Coordinates.Select(c => new Point2D(c.X, c.Y)), name);
        }
    }
}