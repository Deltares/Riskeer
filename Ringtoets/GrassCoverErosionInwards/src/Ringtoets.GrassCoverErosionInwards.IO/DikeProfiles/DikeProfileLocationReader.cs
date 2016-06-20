// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GrasCoverErosionInwardsIoResources = Ringtoets.GrassCoverErosionInwards.IO.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles
{
    /// <summary>
    /// This class is responsible for reading map locations for <see cref="DikeProfile"/>
    /// instances.
    /// </summary>
    public class DikeProfileLocationReader : IDisposable
    {
        private const string idAttributeName = "ID";
        private const string nameAttributeName = "Naam";
        private const string offsetAttributeName = "X0";
        private readonly PointShapeFileReader pointsShapeFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DikeProfileLocationReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException"><paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException"><paramref name="shapeFilePath"/> points to a file that does not exist.</exception>
        public DikeProfileLocationReader(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            pointsShapeFileReader = OpenPointsShapeFile(shapeFilePath);
        }

        /// <summary>
        /// Retrieve a <see cref="DikeProfileLocation"/> for each point feature in the shapefile.
        /// </summary>
        /// <exception cref="CriticalFileReadException"><list type="bullet">
        /// <item>Shapefile does not contain the required attributes</item>
        /// <item>Shapefile misses values for required attributes</item>
        /// <item>Shapefile has an attribute whose type is incorrect</item>
        /// </list></exception>
        /// <returns>A <see cref="List{T}"/> of <see cref="DikeProfileLocation"/> objects.</returns>
        public IList<DikeProfileLocation> GetDikeProfileLocations()
        {
            List<DikeProfileLocation> dikeProfileLocations = new List<DikeProfileLocation>();

            CheckRequiredAttributePresence();

            int dikeProfileLocationCount = pointsShapeFileReader.GetNumberOfLines();
            for (int i = 0; i < dikeProfileLocationCount; i++)
            {
                MapPointData mapPointData = (MapPointData) pointsShapeFileReader.ReadLine();

                IDictionary<string, object> attributes = mapPointData.Features.First().MetaData;

                var attributeIdValue = GetIdAttributeValue(attributes);
                var attributeNameValue = GetNameAttributeValue(attributes);
                var attributeX0Value = GetOffsetAttributeValue(attributes);

                Point2D point = mapPointData.Features.First().MapGeometries.First().PointCollections.First().First();
                try
                {
                    dikeProfileLocations.Add(new DikeProfileLocation(attributeIdValue, attributeNameValue, attributeX0Value, point));
                }
                catch (ArgumentException exception)
                {
                    throw new CriticalFileReadException(exception.Message);
                }
            }

            return dikeProfileLocations;
        }

        public void Dispose()
        {
            pointsShapeFileReader.Dispose();
        }

        /// <summary>
        /// Open a shapefile containing dike locations.
        /// </summary>
        /// <param name="shapeFilePath">Filepath of the shapefile containing dike locations.</param>
        /// <exception cref="CriticalFileReadException">Shapefile does not only contain point features.</exception>
        /// <returns>Return an instance of <see cref="PointShapeFileReader"/>.</returns>
        private PointShapeFileReader OpenPointsShapeFile(string shapeFilePath)
        {
            try
            {
                return new PointShapeFileReader(shapeFilePath);
            }
            catch (CriticalFileReadException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(GrasCoverErosionInwardsIoResources.DikeProfileLocationReader_OpenPointsShapeFile_File_can_only_contain_points);
                throw new CriticalFileReadException(message, e);
            }
        }

        private static double GetOffsetAttributeValue(IDictionary<string, object> attributes)
        {
            var attributeX0Value = attributes[offsetAttributeName] as double?;
            if (attributeX0Value == null)
            {
                throw new CriticalFileReadException(GrasCoverErosionInwardsIoResources.DikeProfileLocationReader_GetDikeProfileLocations_Invalid_X0);
            }
            return attributeX0Value.Value;
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
                        string.Format(GrasCoverErosionInwardsIoResources.DikeProfileLocationReader_CheckRequiredAttributePresence_Missing_attribute_0_, attribute));
                }
            }
        }
    }
}