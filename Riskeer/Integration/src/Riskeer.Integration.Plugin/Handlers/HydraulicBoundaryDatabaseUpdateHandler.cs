// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Helpers;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly update <see cref="HydraulicBoundaryData"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseUpdateHandler : IHydraulicBoundaryDatabaseUpdateHandler
    {
        private readonly AssessmentSection assessmentSection;
        private readonly IDuneLocationsReplacementHandler duneLocationsReplacementHandler;
        private bool updateLocations;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to update for.</param>
        /// <param name="duneLocationsReplacementHandler">The handler to replace dune locations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseUpdateHandler(AssessmentSection assessmentSection,
                                                      IDuneLocationsReplacementHandler duneLocationsReplacementHandler)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (duneLocationsReplacementHandler == null)
            {
                throw new ArgumentNullException(nameof(duneLocationsReplacementHandler));
            }

            this.assessmentSection = assessmentSection;
            this.duneLocationsReplacementHandler = duneLocationsReplacementHandler;
        }

        public bool IsConfirmationRequired(HydraulicBoundaryData hydraulicBoundaryData, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (readHydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicBoundaryDatabase));
            }

            return hydraulicBoundaryData.IsLinked() && hydraulicBoundaryData.Version != readHydraulicBoundaryDatabase.Version;
        }

        public bool InquireConfirmation()
        {
            DialogResult result = MessageBox.Show(Resources.HydraulicBoundaryDatabaseUpdateHandler_Confirm_clear_hydraulicBoundaryDatabase_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> Update(HydraulicBoundaryData hydraulicBoundaryData, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                               ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                               IEnumerable<long> excludedLocationIds, string hydraulicBoundaryDatabaseFilePath, string hlcdFilePath)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (readHydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicBoundaryDatabase));
            }

            if (readHydraulicLocationConfigurationDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicLocationConfigurationDatabase));
            }

            if (excludedLocationIds == null)
            {
                throw new ArgumentNullException(nameof(excludedLocationIds));
            }

            if (hydraulicBoundaryDatabaseFilePath == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabaseFilePath));
            }

            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            if (!IsValidReadHydraulicLocationConfigurationDatabase(readHydraulicLocationConfigurationDatabase))
            {
                string errorMessage = $"{nameof(readHydraulicLocationConfigurationDatabase)} must be null or contain exactly one item for " +
                                      "the collection of hydraulic location configuration database settings.";
                throw new ArgumentException(errorMessage);
            }

            var changedObjects = new List<IObservable>();

            updateLocations = !hydraulicBoundaryData.IsLinked() || hydraulicBoundaryData.Version != readHydraulicBoundaryDatabase.Version;

            if (updateLocations)
            {
                hydraulicBoundaryData.FilePath = hydraulicBoundaryDatabaseFilePath;
                hydraulicBoundaryData.Version = readHydraulicBoundaryDatabase.Version;

                SetLocations(hydraulicBoundaryData, readHydraulicBoundaryDatabase.Locations,
                             readHydraulicLocationConfigurationDatabase.LocationIdMappings,
                             excludedLocationIds.ToArray());

                assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryData.Locations);

                duneLocationsReplacementHandler.Replace(hydraulicBoundaryData.Locations);

                changedObjects.AddRange(GetLocationsAndCalculationsObservables(hydraulicBoundaryData));
                changedObjects.AddRange(RiskeerDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection));
            }
            else
            {
                if (hydraulicBoundaryData.FilePath != hydraulicBoundaryDatabaseFilePath)
                {
                    hydraulicBoundaryData.FilePath = hydraulicBoundaryDatabaseFilePath;
                }
            }

            HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(
                hydraulicBoundaryData.HydraulicLocationConfigurationSettings,
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings?.Single(),
                readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure,
                hlcdFilePath);

            return changedObjects;
        }

        public void DoPostUpdateActions()
        {
            if (updateLocations)
            {
                duneLocationsReplacementHandler.DoPostReplacementUpdates();
            }
        }

        private static bool IsValidReadHydraulicLocationConfigurationDatabase(ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase)
        {
            return readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings == null
                   || readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings.Count() == 1;
        }

        private IEnumerable<IObservable> GetLocationsAndCalculationsObservables(HydraulicBoundaryData hydraulicBoundaryData)
        {
            var locationsAndCalculationsObservables = new List<IObservable>
            {
                hydraulicBoundaryData.Locations,
                assessmentSection.WaterLevelCalculationsForSignalFloodingProbability,
                assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability,
                assessmentSection.DuneErosion.DuneLocations,
                assessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities
            };

            locationsAndCalculationsObservables.AddRange(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                          .Select(element => element.HydraulicBoundaryLocationCalculations));
            locationsAndCalculationsObservables.AddRange(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                          .Select(element => element.HydraulicBoundaryLocationCalculations));
            locationsAndCalculationsObservables.AddRange(assessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                                                          .Select(element => element.DuneLocationCalculations));

            return locationsAndCalculationsObservables;
        }

        private static void SetLocations(HydraulicBoundaryData hydraulicBoundaryData, IEnumerable<ReadHydraulicBoundaryLocation> readLocations,
                                         IEnumerable<ReadHydraulicLocationMapping> locationIdMappings, long[] excludedLocationIds)
        {
            hydraulicBoundaryData.Locations.Clear();

            Array.Sort(excludedLocationIds);

            foreach (ReadHydraulicBoundaryLocation readLocation in readLocations)
            {
                long locationConfigurationId = locationIdMappings.Where(m => m.HrdLocationId == readLocation.Id)
                                                                 .Select(m => m.HlcdLocationId)
                                                                 .SingleOrDefault();

                if (locationConfigurationId != 0 && ShouldInclude(excludedLocationIds, locationConfigurationId))
                {
                    hydraulicBoundaryData.Locations.Add(new HydraulicBoundaryLocation(locationConfigurationId, readLocation.Name,
                                                                                      readLocation.CoordinateX, readLocation.CoordinateY));
                }
            }
        }

        private static bool ShouldInclude(long[] excludedLocationIds, long locationId)
        {
            int matchingIndex = Array.BinarySearch(excludedLocationIds, locationId);
            return matchingIndex < 0;
        }
    }
}