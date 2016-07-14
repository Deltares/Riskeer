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
                return string.Format(RingtoetsCommonIOResources.DataTypeDisplayName_0_shape_file_filter, Name);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool CanImportOn(object targetItem)
        {
            return base.CanImportOn(targetItem) && IsReferenceLineAvailable(targetItem);
        }

        public override bool Import(object targetItem, string filePath)
        {
            var dikeProfilesContext = (DikeProfilesContext)targetItem;
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
                catch (LineParseException exception)
                {
                    var message = string.Format(
                        Resources.DikeProfilesImporter_GetDikeProfileLocationReadResult_Error_reading_DikeProfile_LineNumber_0_Error_1_The_DikeProfile_is_skipped,
                        i + 1,
                        exception.Message);
                    log.Warn(message);
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
        /// <param name="dikeProfileLocationReader">Reader reading <see cref="DikeProfileLocation"/>s for a shapefile.</param>
        /// <param name="referenceLine">The reference line.</param>
        /// <param name="dikeProfileLocations">Collection of <see cref="DikeProfileLocation"/>s to which a new <see cref="DikeProfileLocation"/> is to be added.</param>
        /// <exception cref="LineParseException"><list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// </list></exception>
        private void AddNextDikeProfileLocation(DikeProfileLocationReader dikeProfileLocationReader, ReferenceLine referenceLine, Collection<DikeProfileLocation> dikeProfileLocations)
        {
            DikeProfileLocation dikeProfileLocation = dikeProfileLocationReader.GetNextDikeProfileLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(dikeProfileLocation.Point, referenceLine);
            if (distanceToReferenceLine > 1.0)
            {
                log.ErrorFormat(Resources.DikeProfilesImporter_AddNextDikeProfileLocation_0_skipping_location_outside_referenceline, dikeProfileLocation.Id);
                return;
            }
            if (dikeProfileLocations.Any(dpl => dpl.Id.Equals(dikeProfileLocation.Id)))
            {
                log.WarnFormat(Resources.DikeProfilesImporter_AddNextDikeProfileLocation_DikeLocation_with_id_0_already_read, dikeProfileLocation.Id);
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
                    if (data.SheetPileType != SheetPileType.Coordinates)
                    {
                        log.Error(string.Format(Resources.DikeProfilesImporter_ReadDikeProfileData_sheet_piling_not_zero_skipping_0_, prflFilePath));
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
                    log.Error(exception.Message);
                }
            }

            return new ReadResult<DikeProfileData>(false)
            {
                ImportedItems = dikeProfileData
            };
        }

        private void LogDuplicate(DikeProfileData data, string prflFilePath)
        {
            var message = string.Format(
                Resources.DikeProfilesImporter_LogDuplicateDikeProfileData_Multiple_DikeProfileData_found_for_DikeProfile_0_File_1_skipped,
                data.Id,
                prflFilePath);
            log.Error(message);
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

        private static DikeProfileData GetMatchingDikeProfileData(ICollection<DikeProfileData> dikeProfileDataCollection, string id)
        {
            return dikeProfileDataCollection.FirstOrDefault(d => d.Id.Equals(id));
        }

        private static DikeProfile CreateDikeProfile(DikeProfileLocation dikeProfileLocation, DikeProfileData dikeProfileData)
        {
            var dikeProfile = new DikeProfile(dikeProfileLocation.Point, dikeProfileData.DikeGeometry,
                                              dikeProfileData.ForeshoreGeometry.Select(fg => fg.Point).ToArray(),
                                              CreateBreakWater(dikeProfileData),
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Name = dikeProfileData.Id,
                                                  X0 = dikeProfileLocation.Offset,
                                                  Orientation = dikeProfileData.Orientation,
                                                  DikeHeight = dikeProfileData.DikeHeight
                                              });

            return dikeProfile;
        }

        private static BreakWater CreateBreakWater(DikeProfileData dikeProfileData)
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

        private void HandleUserCancellingImport()
        {
            log.Info(Resources.DikeProfilesImporter_HandleUserCancellingImport_dikeprofile_import_aborted);
            ImportIsCancelled = false;
        }

        private static bool IsReferenceLineAvailable(object targetItem)
        {
            return ((DikeProfilesContext)targetItem).ParentAssessmentSection.ReferenceLine != null;
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