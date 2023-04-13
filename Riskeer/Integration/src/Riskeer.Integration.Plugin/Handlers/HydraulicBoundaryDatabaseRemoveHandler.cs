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
using Core.Common.Base;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.Integration.Data;
using Riskeer.Integration.Service;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for removing a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseRemoveHandler
    {
        private readonly AssessmentSection assessmentSection;
        private readonly IDuneLocationsUpdateHandler duneLocationsUpdateHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseRemoveHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to update.</param>
        /// <param name="duneLocationsUpdateHandler">The handler to update dune locations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseRemoveHandler(AssessmentSection assessmentSection,
                                                      IDuneLocationsUpdateHandler duneLocationsUpdateHandler)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (duneLocationsUpdateHandler == null)
            {
                throw new ArgumentNullException(nameof(duneLocationsUpdateHandler));
            }

            this.assessmentSection = assessmentSection;
            this.duneLocationsUpdateHandler = duneLocationsUpdateHandler;
        }

        /// <summary>
        /// Removes the hydraulic boundary database.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to remove.</param>
        /// <returns>All objects that have been affected by the remove.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/>
        /// is <c>null</c>.</exception>
        public IEnumerable<IObservable> RemoveHydraulicBoundaryDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            assessmentSection.RemoveHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
            duneLocationsUpdateHandler.RemoveLocations(hydraulicBoundaryDatabase.Locations);
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Remove(hydraulicBoundaryDatabase);

            var changedObjects = new List<IObservable>();
            changedObjects.AddRange(GetLocationsAndCalculationsObservables(assessmentSection.HydraulicBoundaryData));
            changedObjects.AddRange(RiskeerDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(assessmentSection));
            return changedObjects;
        }

        /// <summary>
        /// Performs post-remove actions.
        /// </summary>
        public void DoPostRemoveActions()
        {
            duneLocationsUpdateHandler.DoPostUpdateActions();
        }

        private IEnumerable<IObservable> GetLocationsAndCalculationsObservables(HydraulicBoundaryData hydraulicBoundaryData)
        {
            var locationsAndCalculationsObservables = new List<IObservable>
            {
                hydraulicBoundaryData,
                hydraulicBoundaryData.HydraulicBoundaryDatabases,
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
    }
}