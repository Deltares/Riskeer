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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.DikeProfiles;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Abstract class for profile importers, providing an implementation of importing point shapefiles 
    /// containing dike profile locations and text files containing dike schematizations.
    /// </summary>
    /// <seealso cref="FileImporterBase{T}"/>
    public abstract class ProfilesImporter<T> : FileImporterBase<T>
    {
        protected readonly ILog Log = LogManager.GetLogger(typeof(ProfilesImporter<T>));
        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Initializes a new instance of <see cref="ProfilesImporter{T}"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line used to check if the imported profiles are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="importTarget">The import target.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
        protected ProfilesImporter(ReferenceLine referenceLine, string filePath, T importTarget) : base(filePath, importTarget)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            this.referenceLine = referenceLine;
        }

        protected override bool OnImport()
        {
            ReadResult<ProfileLocation> importDikeProfilesResult = ReadProfileLocations();
            if (importDikeProfilesResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            string folderPath = Path.GetDirectoryName(FilePath);
            string[] acceptedIds = importDikeProfilesResult.ImportedItems.Select(dp => dp.Id).ToArray();
            ReadResult<DikeProfileData> importDikeProfileDataResult = ReadDikeProfileData(folderPath, acceptedIds);
            if (importDikeProfileDataResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress(Resources.Importer_ProgressText_Adding_imported_data_to_DataModel, 1, 1);
            CreateProfiles(importDikeProfilesResult, importDikeProfileDataResult);

            return true;
        }

        /// <summary>
        /// Create profile objects from location and geometry data.
        /// </summary>
        /// <param name="importProfileLocationResult">The read profile locations.</param>
        /// <param name="importDikeProfileDataResult">The read dike profile geometries.</param>
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
            for (int i = 0; i < totalNumberOfSteps; i++)
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
                    var message = string.Format(
                        Resources.ProfilesImporter_GetProfileLocationReadResult_Error_reading_Profile_LineNumber_0_Error_1_The_Profile_is_skipped,
                        i + 1,
                        exception.Message);
                    Log.Warn(message);
                }
                catch (CriticalFileReadException exception)
                {
                    Log.Error(exception.Message);
                    return new ReadResult<ProfileLocation>(true);
                }
            }
            return new ReadResult<ProfileLocation>(false)
            {
                ImportedItems = profileLocations
            };
        }

        /// <summary>
        /// Get the next <see cref="ProfileLocation"/> from <paramref name="profileLocationReader"/>
        /// and add to <paramref name="profileLocations"/> in case it is close enough to the <see cref="ReferenceLine"/>.
        /// </summary>
        /// <param name="profileLocationReader">Reader reading <see cref="ProfileLocation"/> objects from a shapefile.</param>
        /// <param name="profileLocations">Collection of <see cref="ProfileLocation"/> objects
        /// to which the new <see cref="ProfileLocation"/> is to be added.</param>
        /// <exception cref="LineParseException"><list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// <item>The read <see cref="ProfileLocation"/> is outside the reference line.</item>
        /// </list></exception>
        private void AddNextProfileLocation(ProfileLocationReader profileLocationReader, ICollection<ProfileLocation> profileLocations)
        {
            ProfileLocation profileLocation = profileLocationReader.GetNextProfileLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(profileLocation.Point);
            if (distanceToReferenceLine > 1.0)
            {
                throw new LineParseException(string.Format(Resources.ProfilesImporter_AddNextProfileLocation_Location_with_id_0_outside_referenceline, profileLocation.Id));
            }
            if (profileLocations.Any(dpl => dpl.Id.Equals(profileLocation.Id)))
            {
                Log.WarnFormat(Resources.ProfilesImporter_AddNextProfileLocation_Location_with_id_0_already_read, profileLocation.Id);
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
            var errorOccured = false;

            for (int i = 0; i < totalNumberOfSteps; i++)
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
                        LogDuplicate(data, prflFilePath);
                    }
                    else
                    {
                        dikeProfileData.Add(data);
                    }
                }
                // No need to catch ArgumentException, as prflFilePaths are valid by construction.
                catch (CriticalFileReadException exception)
                {
                    Log.Error(exception.Message);
                    errorOccured = true;
                }
                catch (CriticalFileValidationException)
                {
                    // Ignore file
                }
            }

            return errorOccured && !dikeProfileData.Any()
                       ? new ReadResult<DikeProfileData>(true)
                       : new ReadResult<DikeProfileData>(false)
                       {
                           ImportedItems = dikeProfileData
                       };
        }

        private void LogDuplicate(DikeProfileData data, string prflFilePath)
        {
            var message = String.Format(
                Resources.ProfilesImporter_LogDuplicateDikeProfileData_Multiple_DikeProfileData_found_for_DikeProfile_0_File_1_skipped,
                data.Id,
                prflFilePath);
            Log.Error(message);
        }

        private double GetDistanceToReferenceLine(Point2D point)
        {
            return GetLineSegments(referenceLine.Points).Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }

        private static IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertLinePointsToLineSegments(linePoints);
        }
    }
}