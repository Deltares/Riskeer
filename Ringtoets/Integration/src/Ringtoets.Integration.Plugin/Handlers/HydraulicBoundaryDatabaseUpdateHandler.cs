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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui.Commands;
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
        private DuneLocationsReplacementHandler duneLocationsReplacementHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to update for.</param>
        /// <param name="viewCommands">The view commands used to close views for removed data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseUpdateHandler(AssessmentSection assessmentSection, IViewCommands viewCommands)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.assessmentSection = assessmentSection;
            duneLocationsReplacementHandler = new DuneLocationsReplacementHandler(viewCommands, assessmentSection.DuneErosion);
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
                                               ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase, string filePath)
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

            var changedObjects = new List<IObservable>();

            if (hydraulicBoundaryDatabase.IsLinked() && hydraulicBoundaryDatabase.Version == readHydraulicBoundaryDatabase.Version)
            {
                if (hydraulicBoundaryDatabase.FilePath != filePath)
                {
                    hydraulicBoundaryDatabase.FilePath = filePath;
                }
            }
            else
            {
                hydraulicBoundaryDatabase.FilePath = filePath;
                hydraulicBoundaryDatabase.Version = readHydraulicBoundaryDatabase.Version;
                SetLocations(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase.Locations);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
                assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
                
                duneLocationsReplacementHandler.Replace(hydraulicBoundaryDatabase.Locations);

                changedObjects.AddRange(GetLocationsAndCalculationsObservables(hydraulicBoundaryDatabase));
                changedObjects.AddRange(RingtoetsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection));
            }

            return changedObjects;
        }

        public void DoPostUpdateActions()
        {
            throw new NotImplementedException();
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

        private static void SetLocations(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, IEnumerable<ReadHydraulicBoundaryLocation> readLocations)
        {
            hydraulicBoundaryDatabase.Locations.Clear();

            foreach (ReadHydraulicBoundaryLocation readLocation in readLocations)
            {
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(readLocation.Id, readLocation.Name,
                                                                                      readLocation.CoordinateX, readLocation.CoordinateY));
            }
        }
    }
}