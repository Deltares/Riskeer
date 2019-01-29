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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.IO.DikeProfiles;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Properties;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// Abstract class for profile importers, providing an implementation of importing point shapefiles 
    /// containing dike profile locations and text files containing dike schematizations.
    /// </summary>
    /// <seealso cref="FileImporterBase{T}"/>
    public abstract class ProfilesImporter<T> : FileImporterBase<T>
    {
        private readonly ReferenceLine referenceLine;
        private readonly string typeDescriptor;

        /// <summary>
        /// Initializes a new instance of <see cref="ProfilesImporter{T}"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line used to check if the imported profiles are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="importTarget">The import target.</param>
        /// <param name="messageProvider">The message provider to provide messages during the import.</param>
        /// <param name="typeDescriptor">The description of the profiles that are imported.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        protected ProfilesImporter(ReferenceLine referenceLine,
                                   string filePath,
                                   T importTarget,
                                   IImporterMessageProvider messageProvider,
                                   string typeDescriptor) : base(filePath, importTarget)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            if (typeDescriptor == null)
            {
                throw new ArgumentNullException(nameof(typeDescriptor));
            }

            this.referenceLine = referenceLine;
            this.typeDescriptor = typeDescriptor;
            MessageProvider = messageProvider;
        }

        /// <summary>
        /// Gets the message provider to provide messages during the import.
        /// </summary>
        protected IImporterMessageProvider MessageProvider { get; }

        protected override bool OnImport()
        {
            ReadResult<ProfileLocation> importDikeProfilesResult = ReadProfileLocations();
            if (importDikeProfilesResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            string folderPath = Path.GetDirectoryName(FilePath);
            string[] acceptedIds = importDikeProfilesResult.Items.Select(dp => dp.Id).ToArray();
            ReadResult<DikeProfileData> importDikeProfileDataResult = ReadDikeProfileData(folderPath, acceptedIds);
            if (importDikeProfileDataResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress(MessageProvider.GetAddDataToModelProgressText(), 1, 1);

            try
            {
                CreateProfiles(importDikeProfilesResult, importDikeProfileDataResult);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(MessageProvider.GetUpdateDataFailedLogMessageText(
                                                   typeDescriptor),
                                               e.Message);
                Log.Error(message, e);
                return false;
            }
            catch (CriticalFileReadException e)
            {
                Log.Error(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create profile objects from location and geometry data.
        /// </summary>
        /// <param name="importProfileLocationResult">The read profile locations.</param>
        /// <param name="importDikeProfileDataResult">The read dike profile geometries.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="importDikeProfileDataResult"/>
        /// does not contain data for a location in <paramref name="importProfileLocationResult"/>.</exception>
        /// <exception cref="UpdateDataException">Thrown when applying updates to the data model has failed.</exception>
        protected abstract void CreateProfiles(ReadResult<ProfileLocation> importProfileLocationResult,
                                               ReadResult<DikeProfileData> importDikeProfileDataResult);

        /// <summary>
        /// Construct a <see cref="BreakWater"/> from a dike profile geometry.
        /// </summary>
        /// <param name="dikeProfileData">The dike profile geometry.</param>
        /// <returns>A new <see cref="BreakWater"/>.</returns>
        protected static BreakWater CreateBreakWater(DikeProfileData dikeProfileData)
        {
            switch (dikeProfileData.DamType)
            {
                case DamType.Caisson:
                    return new BreakWater(BreakWaterType.Caisson, dikeProfileData.DamHeight);
                case DamType.HarborDam:
                    return new BreakWater(BreakWaterType.Dam, dikeProfileData.DamHeight);
                case DamType.Vertical:
                    return new BreakWater(BreakWaterType.Wall, dikeProfileData.DamHeight);
            }

            return null;
        }

        /// <summary>
        /// Obtain the dike profile geometry object matching a given <paramref name="id"/>.
        /// </summary>
        /// <param name="dikeProfileDataCollection">The available dike profile geometry objects.</param>
        /// <param name="id">The id on which to match.</param>
        /// <returns>The matching <see cref="DikeProfileData"/>.</returns>
        protected static DikeProfileData GetMatchingDikeProfileData(IEnumerable<DikeProfileData> dikeProfileDataCollection, string id)
        {
            return dikeProfileDataCollection.FirstOrDefault(d => d.Id.Equals(id));
        }

        /// <summary>
        /// Validate the consistency of a <see cref="DikeProfileData"/> object.
        /// </summary>
        /// <param name="data">The <see cref="DikeProfileData"/> to validate.</param>
        /// <param name="prflFilePath">Filepath of the profile data file.</param>
        /// <returns>Value indicating whether the <see cref="DikeProfileData"/> is valid.</returns>
        protected abstract bool DikeProfileDataIsValid(DikeProfileData data, string prflFilePath);

        private ReadResult<ProfileLocation> ReadProfileLocations()
        {
            NotifyProgress(Resources.ProfilesImporter_ReadProfileLocations_reading_profilelocations, 1, 1);
            try
            {
                using (var profileLocationReader = new ProfileLocationReader(FilePath))
                {
                    return GetProfileLocationReadResult(profileLocationReader);
                }
            }
            catch (CriticalFileReadException exception)
            {
                Log.Error(exception.Message);
            }
            catch (ArgumentException exception)
            {
                Log.Error(exception.Message);
            }

            return new ReadResult<ProfileLocation>(true);
        }

        private ReadResult<ProfileLocation> GetProfileLocationReadResult(ProfileLocationReader profileLocationReader)
        {
            var profileLocations = new Collection<ProfileLocation>();

            int totalNumberOfSteps = profileLocationReader.GetLocationCount;
            for (var i = 0; i < totalNumberOfSteps; i++)
            {
                if (Canceled)
                {
                    return new ReadResult<ProfileLocation>(false);
                }

                try
                {
                    NotifyProgress(Resources.ProfilesImporter_GetProfileLocationReadResult_reading_profilelocation, i + 1, totalNumberOfSteps);
                    AddNextProfileLocation(profileLocationReader, profileLocations);
                }
                catch (LineParseException exception)
                {
                    string message = string.Format(
                        Resources.ProfilesImporter_GetProfileLocationReadResult_Error_reading_Profile_LineNumber_0_Error_1,
                        i + 1,
                        exception.Message);
                    Log.Error(message, exception);
                    return new ReadResult<ProfileLocation>(true);
                }
                catch (CriticalFileReadException exception)
                {
                    Log.Error(exception.Message);
                    return new ReadResult<ProfileLocation>(true);
                }
            }

            return new ReadResult<ProfileLocation>(false)
            {
                Items = profileLocations
            };
        }

        /// <summary>
        /// Get the next <see cref="ProfileLocation"/> from <paramref name="profileLocationReader"/>
        /// and add to <paramref name="profileLocations"/> in case it is close enough to the <see cref="ReferenceLine"/>.
        /// </summary>
        /// <param name="profileLocationReader">Reader reading <see cref="ProfileLocation"/> objects from a shapefile.</param>
        /// <param name="profileLocations">Collection of <see cref="ProfileLocation"/> objects
        /// to which the new <see cref="ProfileLocation"/> is to be added.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the <paramref name="profileLocationReader"/> reads 
        /// multiple locations for a profile.</exception>
        /// <exception cref="LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// <item>The read <see cref="ProfileLocation"/> is outside the reference line.</item>
        /// </list></exception>
        private void AddNextProfileLocation(ProfileLocationReader profileLocationReader, Collection<ProfileLocation> profileLocations)
        {
            ProfileLocation profileLocation = profileLocationReader.GetNextProfileLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(profileLocation.Point);
            if (distanceToReferenceLine > 1.0)
            {
                throw new LineParseException(string.Format(Resources.ProfilesImporter_AddNextProfileLocation_Location_with_id_0_outside_referenceline, profileLocation.Id));
            }

            if (profileLocations.Any(dpl => dpl.Id.Equals(profileLocation.Id)))
            {
                string message = string.Format(Resources.ProfilesImporter_AddNextProfileLocation_Location_with_id_0_already_read, profileLocation.Id);
                throw new CriticalFileReadException(message);
            }

            profileLocations.Add(profileLocation);
        }

        private ReadResult<DikeProfileData> ReadDikeProfileData(string folderPath, string[] acceptedIds)
        {
            NotifyProgress(Resources.ProfilesImporter_ReadDikeProfileData_reading_profile_data, 1, 1);

            // No exception handling for GetFiles, as folderPath is derived from an existing, read file.
            string[] prflFilePaths = Directory.GetFiles(folderPath, "*.prfl");

            int totalNumberOfSteps = prflFilePaths.Length;

            var dikeProfileData = new Collection<DikeProfileData>();
            var dikeProfileDataReader = new DikeProfileDataReader(acceptedIds);

            for (var i = 0; i < totalNumberOfSteps; i++)
            {
                if (Canceled)
                {
                    return new ReadResult<DikeProfileData>(false);
                }

                string prflFilePath = prflFilePaths[i];

                try
                {
                    NotifyProgress(Resources.ProfilesImporter_ReadDikeProfileData_reading_profiledata, i + 1, totalNumberOfSteps);

                    DikeProfileData data = dikeProfileDataReader.ReadDikeProfileData(prflFilePath);

                    if (!DikeProfileDataIsValid(data, prflFilePath))
                    {
                        continue;
                    }

                    if (data.SheetPileType != SheetPileType.Coordinates)
                    {
                        Log.Error(string.Format(Resources.ProfilesImporter_ReadDikeProfileData_sheet_piling_not_zero_skipping_0_, prflFilePath));
                        continue;
                    }

                    if (dikeProfileData.Any(d => d.Id.Equals(data.Id)))
                    {
                        string errorMessage = string.Format(
                            Resources.ProfilesImporter_LogDuplicateDikeProfileData_Multiple_DikeProfileData_found_for_DikeProfile_0_File_1_skipped,
                            data.Id,
                            prflFilePath);
                        throw new CriticalFileReadException(errorMessage);
                    }

                    dikeProfileData.Add(data);
                }
                catch (CriticalFileReadException exception)
                {
                    Log.Error(exception.Message);
                    return new ReadResult<DikeProfileData>(true);
                }
                catch (CriticalFileValidationException)
                {
                    // Ignore file
                }
            }

            return new ReadResult<DikeProfileData>(false)
            {
                Items = dikeProfileData
            };
        }

        private double GetDistanceToReferenceLine(Point2D point)
        {
            return GetLineSegments(referenceLine.Points).Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }

        private static IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertPointsToLineSegments(linePoints);
        }
    }
}