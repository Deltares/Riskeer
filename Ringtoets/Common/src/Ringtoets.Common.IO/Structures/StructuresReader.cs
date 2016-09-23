﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
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
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using Ringtoets.Common.IO.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// This class is responsible for reading structures for <see cref="Structure"/> instances.
    /// </summary>
    public class StructuresReader : IDisposable
    {
        private const string idAttributeName = "KBWIDENT";
        private const string nameAttributeName = "KUNST_OMSC";
        private readonly PointShapeFileReader pointsShapeFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuresReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException"><paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException"><list type="Bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="shapeFilePath"/> does not only contain point features.</item>
        /// <item><paramref name="shapeFilePath"/> does not contain all of the required attributes.</item>
        /// </list></exception>
        public StructuresReader(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            pointsShapeFileReader = OpenPointsShapeFile(shapeFilePath);

            CheckRequiredAttributePresence();
        }

        /// <summary>
        /// Retrieve a <see cref="Structure"/> based on the next point feature in the shapefile.
        /// </summary>
        /// <exception cref="LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// </list></exception>
        /// <returns>A <see cref="Structure"/> based on the next point feature in the shapefile.</returns>
        public Structure GetNextStructure()
        {
            MapPointData mapPointData = (MapPointData)pointsShapeFileReader.ReadFeature();

            IDictionary<string, object> attributes = mapPointData.Features.First().MetaData;

            string attributeIdValue = GetIdAttributeValue(attributes);
            string attributeNameValue = GetNameAttributeValue(attributes, attributeIdValue);

            Point2D point = mapPointData.Features.First().MapGeometries.First().PointCollections.First().First();
            try
            {
                return new Structure(attributeIdValue, attributeNameValue, point);
            }
            catch (ArgumentNullException exception)
            {
                throw new LineParseException(exception.Message);
            }
        }

        private static string GetIdAttributeValue(IDictionary<string, object> attributes)
        {
            var attributeIdValue = attributes[idAttributeName] as string;
            return attributeIdValue;
        }

        private string GetNameAttributeValue(IDictionary<string, object> attributes, string defaultName)
        {
            if (!pointsShapeFileReader.HasAttribute(nameAttributeName))
            {
                return defaultName;
            }
            var attributeNameValue = attributes[nameAttributeName] as string;
            return string.IsNullOrWhiteSpace(attributeNameValue) ? defaultName : attributeNameValue;
        }

        /// <summary>
        /// Gets the number of structures present in the shapefile.
        /// </summary>
        public int GetStructureCount
        {
            get
            {
                return pointsShapeFileReader.GetNumberOfFeatures();
            }
        }

        public void Dispose()
        {
            pointsShapeFileReader.Dispose();
        }

        /// <summary>
        /// Open a shapefile containing structures as point features.
        /// </summary>
        /// <param name="shapeFilePath">Shape file path.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-point geometries in it.</item>
        /// <item>An unexpected error occurred when reading the shapefile.</item>
        /// </list></exception>
        /// <returns>Return an instance of <see cref="PointShapeFileReader"/>.</returns>
        private static PointShapeFileReader OpenPointsShapeFile(string shapeFilePath)
        {
            try
            {
                return new PointShapeFileReader(shapeFilePath);
            }
            catch (CriticalFileReadException e)
            {
                if (e.InnerException.GetType() == typeof(ApplicationException))
                {
                    string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                        .Build(Resources.PointShapefileReader_File_can_only_contain_points);
                    throw new CriticalFileReadException(message, e);
                }

                throw;
            }
        }

        private void CheckRequiredAttributePresence()
        {
            if (!pointsShapeFileReader.HasAttribute(idAttributeName))
            {
                throw new CriticalFileReadException(
                    string.Format(Resources.ProfileLocationReader_CheckRequiredAttributePresence_Missing_attribute_0_, idAttributeName));
            }
        }
    }
}