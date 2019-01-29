// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using Riskeer.Common.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.DikeProfiles
{
    /// <summary>
    /// This class is responsible for reading map locations for <see cref="ProfileLocation"/> instances.
    /// </summary>
    public class ProfileLocationReader : IDisposable
    {
        private const string idAttributeName = "ID";
        private const string nameAttributeName = "Naam";
        private const string offsetAttributeName = "X0";
        private readonly PointShapeFileReader pointsShapeFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileLocationReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="shapeFilePath"/> does not only contain point features.</item>
        /// <item><paramref name="shapeFilePath"/> does not contain all of the required attributes.</item>
        /// </list></exception>
        public ProfileLocationReader(string shapeFilePath)
        {
            IOUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            pointsShapeFileReader = OpenPointsShapeFile(shapeFilePath);

            CheckRequiredAttributePresence();
        }

        /// <summary>
        /// Gets the number of profile locations present in the shapefile.
        /// </summary>
        public int GetLocationCount
        {
            get
            {
                return pointsShapeFileReader.GetNumberOfFeatures();
            }
        }

        /// <summary>
        /// Retrieve a <see cref="ProfileLocation"/> based on the next point feature in the shapefile.
        /// </summary>
        /// <exception cref="LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// </list></exception>
        /// <returns>A <see cref="ProfileLocation"/> based on the next point feature in the shapefile.</returns>
        public ProfileLocation GetNextProfileLocation()
        {
            var mapPointData = (MapPointData) pointsShapeFileReader.ReadFeature();

            IDictionary<string, object> attributes = mapPointData.Features.First().MetaData;

            string attributeIdValue = GetIdAttributeValue(attributes);
            string attributeNameValue = GetNameAttributeValue(attributes);
            double attributeX0Value = GetOffsetAttributeValue(attributes);

            Point2D point = mapPointData.Features.First().MapGeometries.First().PointCollections.First().First();
            try
            {
                return new ProfileLocation(attributeIdValue, attributeNameValue, attributeX0Value, point);
            }
            catch (ArgumentException exception)
            {
                throw new LineParseException(exception.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                pointsShapeFileReader.Dispose();
            }
        }

        /// <summary>
        /// Open a shapefile containing dike locations as point features.
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
                if (e.InnerException != null && e.InnerException.GetType() == typeof(ApplicationException))
                {
                    string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                        .Build(Resources.PointShapefileReader_File_can_only_contain_points);
                    throw new CriticalFileReadException(message, e);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the offset value in <paramref name="attributes"/>.
        /// </summary>
        /// <param name="attributes">The attributes containing the offset value.</param>
        /// <returns>The read offset value.</returns>
        /// <exception cref="LineParseException">Thrown when <paramref name="attributes"/> does 
        /// not a valid value for offset.</exception>
        private static double GetOffsetAttributeValue(IDictionary<string, object> attributes)
        {
            object value = attributes[offsetAttributeName];
            if (value == null)
            {
                throw new LineParseException(Resources.ProfileLocationReader_GetProfileLocations_Invalid_X0);
            }

            try
            {
                return Convert.ToDouble(value);
            }
            catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException)
            {
                throw new LineParseException(Resources.ProfileLocationReader_GetProfileLocations_Invalid_X0, e);
            }
        }

        private static string GetNameAttributeValue(IDictionary<string, object> attributes)
        {
            var attributeNameValue = attributes[nameAttributeName] as string;
            return attributeNameValue;
        }

        private static string GetIdAttributeValue(IDictionary<string, object> attributes)
        {
            var attributeIdValue = attributes[idAttributeName] as string;
            return attributeIdValue;
        }

        /// <summary>
        /// Validates that the shape file has the required attributes.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the shape file does not have 
        /// the required attributes.</exception>
        private void CheckRequiredAttributePresence()
        {
            IEnumerable<string> requiredAttributes = new[]
            {
                idAttributeName,
                nameAttributeName,
                offsetAttributeName
            };
            foreach (string attribute in requiredAttributes)
            {
                if (!pointsShapeFileReader.HasAttribute(attribute))
                {
                    throw new CriticalFileReadException(
                        string.Format(Resources.ProfileLocationReader_CheckRequiredAttributePresence_Missing_attribute_0_, attribute));
                }
            }
        }
    }
}