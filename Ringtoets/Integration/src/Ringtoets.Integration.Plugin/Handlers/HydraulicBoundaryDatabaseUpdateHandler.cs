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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Plugin.Handlers;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly update a <see cref="HydraulicBoundaryDatabase"/>.
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

        public bool IsConfirmationRequired(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (readHydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicBoundaryDatabase));
            }

            return hydraulicBoundaryDatabase.IsLinked() && hydraulicBoundaryDatabase.Version != readHydraulicBoundaryDatabase.Version;
        }

        public bool InquireConfirmation()
        {
            DialogResult result = MessageBox.Show(Resources.HydraulicBoundaryDatabaseUpdateHandler_Confirm_clear_hydraulicBoundaryDatabase_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> Update(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                               ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                               IEnumerable<long> excludedLocationIds, string hydraulicBoundaryDatabaseFilePath, string hlcdFilePath)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
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

            var changedObjects = new List<IObservable>();

            updateLocations = !hydraulicBoundaryDatabase.IsLinked() || hydraulicBoundaryDatabase.Version != readHydraulicBoundaryDatabase.Version;

            if (updateLocations)
            {
                hydraulicBoundaryDatabase.FilePath = hydraulicBoundaryDatabaseFilePath;
                hydraulicBoundaryDatabase.Version = readHydraulicBoundaryDatabase.Version;
                hydraulicBoundaryDatabase.TrackId = readHydraulicBoundaryDatabase.TrackId;

                SetLocations(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase.Locations,
                             readHydraulicLocationConfigurationDatabase.LocationIdMappings,
                             excludedLocationIds.ToArray());

                assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
                assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

                duneLocationsReplacementHandler.Replace(hydraulicBoundaryDatabase.Locations);

                changedObjects.AddRange(GetLocationsAndCalculationsObservables(hydraulicBoundaryDatabase));
                changedObjects.AddRange(RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection));
            }
            else
            {
                if (hydraulicBoundaryDatabase.FilePath != hydraulicBoundaryDatabaseFilePath)
                {
                    hydraulicBoundaryDatabase.FilePath = hydraulicBoundaryDatabaseFilePath;
                }
            }

            SetHydraulicLocationConfigurationSettings(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings,
                                                      readHydraulicLocationConfigurationDatabase, 
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

        private static void SetHydraulicLocationConfigurationSettings(HydraulicLocationConfigurationSettings hydraulicLocationConfigurationSettings,
                                                                      ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                                      string hlcdFilePath)
        {
            if (readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings != null)
            {
                ReadHydraulicLocationConfigurationDatabaseSettings readSettings =
                    readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings
                                                              .Single();
                hydraulicLocationConfigurationSettings.SetValues(hlcdFilePath,
                                                                 readSettings.ScenarioName,
                                                                 readSettings.Year,
                                                                 readSettings.Scope,
                                                                 readSettings.SeaLevel,
                                                                 readSettings.RiverDischarge,
                                                                 readSettings.LakeLevel,
                                                                 readSettings.WindDirection,
                                                                 readSettings.WindSpeed,
                                                                 readSettings.Comment);
            }
            else
            {
                hydraulicLocationConfigurationSettings.SetValues(hlcdFilePath,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_mandatory_properties,
                                                                 2023,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_mandatory_properties,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_optional_properties,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_optional_properties,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_optional_properties,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_optional_properties,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_optional_properties,
                                                                 Resources.HydraulicBoundaryDatabaseUpdateHandler_SetHydraulicLocationConfigurationSettings_default_value_for_optional_properties);
            }
        }

        private IEnumerable<IObservable> GetLocationsAndCalculationsObservables(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            return new IObservable[]
            {
                hydraulicBoundaryDatabase.Locations,
                assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm,
                assessmentSection.WaterLevelCalculationsForSignalingNorm,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                assessmentSection.WaveHeightCalculationsForSignalingNorm,
                assessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                assessmentSection.DuneErosion.DuneLocations,
                assessmentSection.DuneErosion.CalculationsForMechanismSpecificFactorizedSignalingNorm,
                assessmentSection.DuneErosion.CalculationsForMechanismSpecificSignalingNorm,
                assessmentSection.DuneErosion.CalculationsForMechanismSpecificLowerLimitNorm,
                assessmentSection.DuneErosion.CalculationsForLowerLimitNorm,
                assessmentSection.DuneErosion.CalculationsForFactorizedLowerLimitNorm
            };
        }

        private static void SetLocations(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, IEnumerable<ReadHydraulicBoundaryLocation> readLocations,
                                         IEnumerable<ReadHydraulicLocationMapping> locationIdMappings, long[] excludedLocationIds)
        {
            hydraulicBoundaryDatabase.Locations.Clear();

            Array.Sort(excludedLocationIds);

            foreach (ReadHydraulicBoundaryLocation readLocation in readLocations)
            {
                long locationConfigurationId = locationIdMappings.Where(m => m.HrdLocationId == readLocation.Id)
                                                                 .Select(m => m.HlcdLocationId)
                                                                 .SingleOrDefault();

                if (locationConfigurationId != 0 && ShouldInclude(excludedLocationIds, locationConfigurationId))
                {
                    hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(locationConfigurationId, readLocation.Name,
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