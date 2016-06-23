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
using Core.Common.Base;
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
                return "Dijkprofiel locaties";
            }
        }

        public override string Category
        {
            get
            {
                return "Algemeen";
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
                return string.Format("{0} {1} (*.shp)|*.shp",
                                     "Dijkprofiel locaties", "Shape bestand");
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool CanImportOn(object targetItem)
        {
            return base.CanImportOn(targetItem) && IsReferenceLineAvailable(targetItem);
        }

        public override bool Import(object targetItem, string filePath)
        {
            if (!IsReferenceLineAvailable(targetItem))
            {
                log.Error("Er is geen referentielijn beschikbaar. Geen data ingelezen.");
                return false;
            }

            var dikeProfilesContext = (DikeProfilesContext) targetItem;
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

            string folder = Path.GetDirectoryName(filePath);
            ReadResult<DikeProfileData> importDikeProfileDataResult = ReadDikeProfileData(folder);
            if (importDikeProfileDataResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            ObservableList<DikeProfile> dikeProfiles = CreateDikeProfiles(importDikeProfilesResult.ImportedItems, importDikeProfileDataResult.ImportedItems);

            foreach (DikeProfile dikeProfile in dikeProfiles)
            {
                dikeProfilesContext.WrappedData.Add(dikeProfile);
            }

            return true;
        }

        private ReadResult<DikeProfileLocation> ReadDikeProfileLocations(string filePath, ReferenceLine referenceLine)
        {
            NotifyProgress("Inlezen van dijkprofiel locaties uit een shape bestand.", 1, 1);
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
            var totalNumberOfSteps = dikeProfileLocationReader.GetLocationCount;
            var currentStep = 1;

            var dikeProfileLocations = new Collection<DikeProfileLocation>();
            for (int i = 0; i < totalNumberOfSteps; i++)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<DikeProfileLocation>(false);
                }

                try
                {
                    NotifyProgress("Inlezen van dijkprofiel locatie.", currentStep++, totalNumberOfSteps);
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

        private void AddNextDikeProfileLocation(DikeProfileLocationReader dikeProfileLocationReader, ReferenceLine referenceLine, Collection<DikeProfileLocation> dikeProfileLocations)
        {
            DikeProfileLocation dikeProfileLocation = dikeProfileLocationReader.GetNextDikeProfileLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(dikeProfileLocation.Point, referenceLine);
            if (distanceToReferenceLine > 1.0)
            {
                log.Error("Een dijkprofiel locatie ligt niet op de referentielijn. Locatie wordt overgeslagen.");
                return;
            }
            dikeProfileLocations.Add(dikeProfileLocation);
        }

        private ReadResult<DikeProfileData> ReadDikeProfileData(string folder)
        {
            NotifyProgress("Inlezen van voorland en dijkprofiel data uit een prfl bestand.", 1, 1);
            var dikeProfileDataReader = new DikeProfileDataReader();
            return GetDikeProfileDataReadResult(dikeProfileDataReader, folder);
        }

        private ReadResult<DikeProfileData> GetDikeProfileDataReadResult(DikeProfileDataReader dikeProfileDataReader, string folder)
        {
            // No exception handling for GetFiles, as folder is derived from an existing, read file.
            string[] prflFilePaths = Directory.GetFiles(folder, "*.prfl");

            var totalNumberOfSteps = prflFilePaths.Length;
            var currentStep = 1;

            var dikeProfileData = new Collection<DikeProfileData>();
            Dictionary<string, List<string>> duplicates = new Dictionary<string, List<string>>();

            for (int i = 0; i < totalNumberOfSteps; i++)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<DikeProfileData>(false);
                }

                try
                {
                    NotifyProgress("Inlezen van voorland en dijkprofiel data.", currentStep++, totalNumberOfSteps);

                    DikeProfileData data = dikeProfileDataReader.ReadDikeProfileData(prflFilePaths[i]);
                    if (data.ProfileType != ProfileType.Coordinates)
                    {
                        log.Error(string.Format("Voorland en dijkprofiel data specificeert een damwand waarde ongelijk aan 0. Bestand wordt overgeslagen: {0}", prflFilePaths[i]));
                        continue;
                    }

                    if (dikeProfileData.Any(d => d.Id.Equals(data.Id)))
                    {
                        if (!duplicates.ContainsKey(data.Id))
                        {
                            duplicates.Add(data.Id, new List<string>());
                        }
                        duplicates[data.Id].Add(prflFilePaths[i]);
                    }
                    else
                    {
                        dikeProfileData.Add(data);
                    }
                }
                // No ArgumentException is caught, as prflFilePaths are valid by construction.
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

        private void LogDuplicateDikeProfileData(Dictionary<string, List<string>> duplicates)
        {
            foreach (KeyValuePair<string, List<string>> keyValuePair in duplicates)
            {
                StringBuilder builder = new StringBuilder(string.Format("Meerdere dijkprofiel data bestanden gevonden met Id {0}. Alleen de eerste wordt gebruikt. De {1} overgeslagen bestanden zijn:", keyValuePair.Key, keyValuePair.Value.Count));
                foreach (string filePath in keyValuePair.Value)
                {
                    if (builder.Length + filePath.Length + Environment.NewLine.Length < builder.MaxCapacity)
                    {
                        builder.AppendLine(filePath);
                    }
                }
                string message = builder.ToString();
                log.Error(message);
            }
        }

        private ObservableList<DikeProfile> CreateDikeProfiles(ICollection<DikeProfileLocation> dikeProfileLocationCollection, ICollection<DikeProfileData> dikeProfileDataCollection)
        {
            ObservableList<DikeProfile> dikeProfiles = new ObservableList<DikeProfile>();
            foreach (DikeProfileLocation dikeProfileLocation in dikeProfileLocationCollection)
            {
                string id = dikeProfileLocation.Id;

                var dikeProfileData = GetMatchingDikeProfileData(dikeProfileDataCollection, id);
                if (dikeProfileData != null)
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
            if (matchingDikeProfileData == null)
            {
                log.Error(string.Format("Kan geen voorland en dijkprofiel data vinden voor dijkprofiel locatie met Id: {0}", id));
                return null;
            }
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
                ForeshoreGeometry = dikeProfileData.ForeshoreGeometry.Select(r => r.Point).ToList(),
                DikeGeometry = dikeProfileData.DikeGeometry.ToList(),
                DikeHeight = (RoundedDouble) dikeProfileData.DikeHeight
            };

            switch (dikeProfileData.DamType)
            {
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
            log.Info("Dijkprofielen importeren is afgebroken. Geen data ingelezen.");
            ImportIsCancelled = false;
        }

        private static bool IsReferenceLineAvailable(object targetItem)
        {
            return ((DikeProfilesContext) targetItem).ParentAssessmentSection.ReferenceLine != null;
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