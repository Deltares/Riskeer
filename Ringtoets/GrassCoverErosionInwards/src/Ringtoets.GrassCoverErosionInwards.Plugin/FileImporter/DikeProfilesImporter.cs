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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.FileImporter
{
    /// <summary>
    /// Imports point shapefiles containing dike profile locations and text file containing the foreland/dike schematizations.
    /// </summary>
    public class DikeProfilesImporter : FileImporterBase<DikeProfilesContext>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(DikeProfilesImporter));

        public override string Name
        {
            get
            {
                return Resources.DikeProfilesImporter_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return Resources.DikeProfile;
            }
        }

        public override string FileFilter
        {
            get
            {
                return string.Format(RingtoetsCommonIOResources.DikeProfilesImport_FileFilter_0_1_shapefile_extension,
                                     Resources.DikeProfilesImporter_DisplayName, RingtoetsCommonDataResources.DikeProfilesImporter_FileFilter_Shapefile);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool CanImportOn(object targetItem)
        {
            var dikeProfilesContext = (DikeProfilesContext) targetItem;
            return base.CanImportOn(targetItem) && IsReferenceLineAvailable(dikeProfilesContext);
        }

        public override bool Import(object targetItem, string filePath)
        {
            var dikeProfilesContext = (DikeProfilesContext) targetItem;
            if (!IsReferenceLineAvailable(dikeProfilesContext))
            {
                log.Error(Resources.DikeProfilesImporter_Import_no_referenceline_import_aborted);
                return false;
            }

            ReferenceLine referenceLine = dikeProfilesContext.ParentAssessmentSection.ReferenceLine;

            ReadResult<DikeProfileLocation> importDikeProfilesResult = ReadDikeProfileLocations(filePath, referenceLine);
            if (importDikeProfilesResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            string folderPath = Path.GetDirectoryName(filePath);
            ReadResult<DikeProfileData> importDikeProfileDataResult = ReadDikeProfileData(folderPath);
            if (importDikeProfileDataResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            IEnumerable<DikeProfile> dikeProfiles = CreateDikeProfiles(importDikeProfilesResult.ImportedItems, importDikeProfileDataResult.ImportedItems);

            foreach (DikeProfile dikeProfile in dikeProfiles)
            {
                dikeProfilesContext.WrappedData.Add(dikeProfile);
            }

            return true;
        }

        private ReadResult<DikeProfileLocation> ReadDikeProfileLocations(string filePath, ReferenceLine referenceLine)
        {
            NotifyProgress(Resources.DikeProfilesImporter_ReadDikeProfileLocations_reading_dikeprofilelocations, 1, 1);
            try
            {
                using (var dikeProfileLocationReader = new DikeProfileLocationReader(filePath))
                {
                    return GetDikeProfileLocationReadResult(dikeProfileLocationReader, referenceLine);
                }
            }
            catch (CriticalFileReadException exception)
            {
                log.Error(exception.Message);
            }
            catch (ArgumentException exception)
            {
                log.Error(exception.Message);
            }
            return new ReadResult<DikeProfileLocation>(true);
        }

        private ReadResult<DikeProfileLocation> GetDikeProfileLocationReadResult(DikeProfileLocationReader dikeProfileLocationReader, ReferenceLine referenceLine)
        {
            var dikeProfileLocations = new Collection<DikeProfileLocation>();

            int totalNumberOfSteps = dikeProfileLocationReader.GetLocationCount;
            for (int i = 0; i < totalNumberOfSteps; i++)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<DikeProfileLocation>(false);
                }

                try
                {
                    NotifyProgress(Resources.DikeProfilesImporter_GetDikeProfileLocationReadResult_reading_dikeprofilelocation, i + 1, totalNumberOfSteps);
                    AddNextDikeProfileLocation(dikeProfileLocationReader, referenceLine, dikeProfileLocations);
                }
                catch (CriticalFileReadException exception)
                {
                    log.Error(exception.Message);
                    return new ReadResult<DikeProfileLocation>(true);
                }
            }
            return new ReadResult<DikeProfileLocation>(false)
            {
                ImportedItems = dikeProfileLocations
            };
        }

        /// <summary>
        /// Get the next DikeProfileLocation from <paramref name="dikeProfileLocationReader"/> and add to <paramref name="dikeProfileLocations"/> in case it is close enough to <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="dikeProfileLocationReader">Reader reading DikeProfileLocations for a shapefile.</param>
        /// <param name="referenceLine">The reference line.</param>
        /// <param name="dikeProfileLocations">Collection of DikeProfileLocations to which a new DikeProfileLocation is to be added.</param>
        /// <exception cref="CriticalFileReadException"><list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// </list></exception>
        private void AddNextDikeProfileLocation(DikeProfileLocationReader dikeProfileLocationReader, ReferenceLine referenceLine, Collection<DikeProfileLocation> dikeProfileLocations)
        {
            DikeProfileLocation dikeProfileLocation = dikeProfileLocationReader.GetNextDikeProfileLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(dikeProfileLocation.Point, referenceLine);
            if (distanceToReferenceLine > 1.0)
            {
                log.Error(string.Format(Resources.DikeProfilesImporter_AddNextDikeProfileLocation_0_skipping_location_outside_referenceline, dikeProfileLocation.Id));
                return;
            }
            dikeProfileLocations.Add(dikeProfileLocation);
        }

        private ReadResult<DikeProfileData> ReadDikeProfileData(string folderPath)
        {
            NotifyProgress(Resources.DikeProfilesImporter_ReadDikeProfileData_reading_dikeprofile_data, 1, 1);

            // No exception handling for GetFiles, as folderPath is derived from an existing, read file.
            string[] prflFilePaths = Directory.GetFiles(folderPath, "*.prfl");

            int totalNumberOfSteps = prflFilePaths.Length;

            var dikeProfileData = new Collection<DikeProfileData>();
            Dictionary<string, List<string>> duplicates = new Dictionary<string, List<string>>();
            var dikeProfileDataReader = new DikeProfileDataReader();

            for (int i = 0; i < totalNumberOfSteps; i++)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<DikeProfileData>(false);
                }

                string prflFilePath = prflFilePaths[i];

                try
                {
                    NotifyProgress(Resources.DikeProfilesImporter_ReadDikeProfileData_reading_dikeprofiledata, i + 1, totalNumberOfSteps);

                    DikeProfileData data = dikeProfileDataReader.ReadDikeProfileData(prflFilePath);
                    if (data.ProfileType != ProfileType.Coordinates)
                    {
                        log.Error(string.Format(Resources.DikeProfilesImporter_ReadDikeProfileData_sheet_piling_not_zero_skipping_0_, prflFilePath));
                        continue;
                    }

                    if (dikeProfileData.Any(d => d.Id.Equals(data.Id)))
                    {
                        UpdateDuplicates(duplicates, data, prflFilePath);
                    }
                    else
                    {
                        dikeProfileData.Add(data);
                    }
                }
                    // No need to catch ArgumentException, as prflFilePaths are valid by construction.
                catch (CriticalFileReadException exception)
                {
                    log.Error(exception.Message);
                    return new ReadResult<DikeProfileData>(true);
                }
            }

            LogDuplicateDikeProfileData(duplicates);

            return new ReadResult<DikeProfileData>(false)
            {
                ImportedItems = dikeProfileData
            };
        }

        private static void UpdateDuplicates(Dictionary<string, List<string>> duplicates, DikeProfileData data, string prflFilePath)
        {
            if (!duplicates.ContainsKey(data.Id))
            {
                duplicates.Add(data.Id, new List<string>());
            }
            duplicates[data.Id].Add(prflFilePath);
        }

        private void LogDuplicateDikeProfileData(Dictionary<string, List<string>> duplicates)
        {
            foreach (KeyValuePair<string, List<string>> keyValuePair in duplicates)
            {
                StringBuilder builder = new StringBuilder(
                    string.Format(Resources.DikeProfilesImporter_LogDuplicateDikeProfileData_dikeprofiledata_file_with_id_0_used_files_1_are_skipped,
                                  keyValuePair.Key, keyValuePair.Value.Count));

                foreach (string filePath in keyValuePair.Value)
                {
                    if (BuilderNotAtMaxCapacity(builder, filePath))
                    {
                        builder.AppendLine(filePath);
                    }
                }
                string message = builder.ToString();
                log.Error(message);
            }
        }

        private static bool BuilderNotAtMaxCapacity(StringBuilder builder, string filePath)
        {
            return builder.Length + filePath.Length + Environment.NewLine.Length < builder.MaxCapacity;
        }

        private IEnumerable<DikeProfile> CreateDikeProfiles(ICollection<DikeProfileLocation> dikeProfileLocationCollection, ICollection<DikeProfileData> dikeProfileDataCollection)
        {
            List<DikeProfile> dikeProfiles = new List<DikeProfile>();
            foreach (DikeProfileLocation dikeProfileLocation in dikeProfileLocationCollection)
            {
                string id = dikeProfileLocation.Id;

                var dikeProfileData = GetMatchingDikeProfileData(dikeProfileDataCollection, id);
                if (dikeProfileData == null)
                {
                    log.Error(string.Format(Resources.DikeProfilesImporter_GetMatchingDikeProfileData_no_dikeprofiledata_for_location_0_, id));
                }
                else
                {
                    DikeProfile dikeProfile = CreateDikeProfile(dikeProfileLocation, dikeProfileData);
                    dikeProfiles.Add(dikeProfile);
                }
            }
            return dikeProfiles;
        }

        private DikeProfileData GetMatchingDikeProfileData(ICollection<DikeProfileData> dikeProfileDataCollection, string id)
        {
            DikeProfileData matchingDikeProfileData = dikeProfileDataCollection.FirstOrDefault(d => d.Id.Equals(id));
            return matchingDikeProfileData;
        }

        private static DikeProfile CreateDikeProfile(DikeProfileLocation dikeProfileLocation, DikeProfileData dikeProfileData)
        {
            var dikeProfile = new DikeProfile(dikeProfileLocation.Point)
            {
                Name = dikeProfileData.Id,
                Memo = dikeProfileData.Memo,
                X0 = dikeProfileLocation.Offset,
                Orientation = (RoundedDouble) dikeProfileData.Orientation,
                DikeHeight = (RoundedDouble) dikeProfileData.DikeHeight
            };

            List<Point2D> foreshoreGeometry = dikeProfile.ForeshoreGeometry;
            foreshoreGeometry.Clear();
            foreshoreGeometry.AddRange(dikeProfileData.ForeshoreGeometry.Select(r => r.Point));

            List<RoughnessPoint> dikeGeometry = dikeProfile.DikeGeometry;
            dikeGeometry.Clear();
            dikeGeometry.AddRange(dikeProfileData.DikeGeometry);

            switch (dikeProfileData.DamType)
            {
                case DamType.None:
                    dikeProfile.BreakWater = null;
                    break;
                case DamType.Caisson:
                    dikeProfile.BreakWater = new BreakWater(BreakWaterType.Caisson, dikeProfileData.DamHeight);
                    break;
                case DamType.HarborDam:
                    dikeProfile.BreakWater = new BreakWater(BreakWaterType.Dam, dikeProfileData.DamHeight);
                    break;
                case DamType.Vertical:
                    dikeProfile.BreakWater = new BreakWater(BreakWaterType.Wall, dikeProfileData.DamHeight);
                    break;
                default:
                    // Invalid values are caught as exceptions from DikeProfileDataReader.
                    break;
            }
            return dikeProfile;
        }

        private void HandleUserCancellingImport()
        {
            log.Info(Resources.DikeProfilesImporter_HandleUserCancellingImport_dikeprofile_import_aborted);
            ImportIsCancelled = false;
        }

        private static bool IsReferenceLineAvailable(DikeProfilesContext targetItem)
        {
            return targetItem.ParentAssessmentSection.ReferenceLine != null;
        }

        private double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points).Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }

        private IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertLinePointsToLineSegments(linePoints);
        }
    }
}