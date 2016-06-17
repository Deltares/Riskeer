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
        /// Disposes of the utilized <see cref="pointsShapeFileReader"/> instance.
        /// </summary>
        public void Dispose()
        {
            pointsShapeFileReader.Dispose();
        }

        /// <summary>
        /// Open a shapefile containing dike locations.
        /// </summary>
        /// <param name="shapeFilePath">Filepath of the shapefile containing dike locations.</param>
        /// <exception cref="CriticalFileReadException">Shapefile does not only contain point features.</exception>
        /// <returns></returns>
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

        /// <summary>
        /// Get the number of point features in the shapefile.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Shapefile does not contain the required attributes.</exception>
        /// <returns></returns>
        private int GetDikeProfileLocationCount()
        {
            foreach (string attribute in new[]{"ID", "Naam", "X0"})
            {
                if (!pointsShapeFileReader.HasAttribute(attribute))
                {
                    throw new CriticalFileReadException(
                        string.Format("Het bestand heeft geen attribuut '{0}' welke vereist is om de locaties van de dijkprofielen in te lezen.", attribute));
                }
            }
            return pointsShapeFileReader.GetNumberOfLines();
        }

        /// <summary>
        /// Retrieve a <see cref="DikeProfileLocation"/> for each point feature in the shapefile.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Shapefile does not contain values for all required attributes.</exception>
        /// <returns>A <see cref="List{T}"/> of <see cref="DikeProfileLocation"/> objects.</returns>
        public IList<DikeProfileLocation> GetDikeProfileLocations()
        {
            List<DikeProfileLocation> dikeProfileLocations = new List<DikeProfileLocation>();

            int dikeProfileLocationCount = GetDikeProfileLocationCount();
            for (int i = 0; i < dikeProfileLocationCount; i++)
            {
                MapPointData mapPointData = (MapPointData)pointsShapeFileReader.ReadLine();

                IDictionary<string, object> attributes = mapPointData.Features.First().MetaData;

                var attributeIdValue = attributes["ID"] as string;
                if (string.IsNullOrWhiteSpace(attributeIdValue))
                {
                    throw new CriticalFileReadException(GrasCoverErosionInwardsIoResources.DikeProfileLocationReader_GetDikeProfileLocations_Invalid_Id);
                }

                var attributeNameValue = attributes["Naam"] as string;
                if (string.IsNullOrWhiteSpace(attributeNameValue))
                {
                    throw new CriticalFileReadException(GrasCoverErosionInwardsIoResources.DikeProfileLocationReader_GetDikeProfileLocations_Invalid_Name);
                }

                var attributeX0Value = attributes["X0"] as double?;
                if (attributeX0Value == null)
                {
                    throw new CriticalFileReadException(GrasCoverErosionInwardsIoResources.DikeProfileLocationReader_GetDikeProfileLocations_Invalid_X0);
                }

                Point2D point = mapPointData.Features.First().MapGeometries.First().PointCollections.First().First();
                dikeProfileLocations.Add(new DikeProfileLocation(attributeIdValue, attributeNameValue, attributeX0Value.Value, point));
            }

            return dikeProfileLocations;
        }
    }
}