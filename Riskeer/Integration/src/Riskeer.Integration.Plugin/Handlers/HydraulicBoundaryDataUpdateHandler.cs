﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly update the hydraulic boundary databases within a <see cref="HydraulicBoundaryData"/> instance.
    /// </summary>
    public class HydraulicBoundaryDataUpdateHandler : IHydraulicBoundaryDataUpdateHandler
    {
        private readonly AssessmentSection assessmentSection;
        private readonly IDuneLocationsUpdateHandler duneLocationsUpdateHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDataUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to update for.</param>
        /// <param name="duneLocationsUpdateHandler">The handler for adding dune locations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDataUpdateHandler(AssessmentSection assessmentSection,
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

        public IEnumerable<IObservable> Update(HydraulicBoundaryData hydraulicBoundaryData, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                               ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                               IEnumerable<long> excludedLocationIds, string hrdFilePath)
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

            if (hrdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hrdFilePath));
            }

            IEnumerable<HydraulicBoundaryLocation> newHydraulicBoundaryLocations = CreateHydraulicBoundaryLocations(
                readHydraulicBoundaryDatabase.Locations,
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocations
                                                          .Where(rhl => rhl.TrackId == readHydraulicBoundaryDatabase.TrackId),
                excludedLocationIds.ToArray()).ToArray();

            var newHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hrdFilePath,
                Version = readHydraulicBoundaryDatabase.Version,
                UsePreprocessorClosure = readHydraulicLocationConfigurationDatabase.ReadTracks
                                                                                   .FirstOrDefault(rt => rt.TrackId == readHydraulicBoundaryDatabase.TrackId)?
                                                                                   .UsePreprocessorClosure ?? false
            };

            newHydraulicBoundaryDatabase.Locations.AddRange(newHydraulicBoundaryLocations);

            hydraulicBoundaryData.FilePath = hrdFilePath;
            hydraulicBoundaryData.HydraulicBoundaryDatabases.Add(newHydraulicBoundaryDatabase);

            hydraulicBoundaryData.Locations.AddRange(newHydraulicBoundaryLocations);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(newHydraulicBoundaryDatabase.Locations);
            duneLocationsUpdateHandler.AddLocations(newHydraulicBoundaryDatabase.Locations);

            return GetLocationsAndCalculationsObservables(hydraulicBoundaryData);
        }

        public void DoPostUpdateActions()
        {
            duneLocationsUpdateHandler.DoPostUpdateActions();
        }

        private IEnumerable<IObservable> GetLocationsAndCalculationsObservables(HydraulicBoundaryData hydraulicBoundaryData)
        {
            var locationsAndCalculationsObservables = new List<IObservable>
            {
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

        private static IEnumerable<HydraulicBoundaryLocation> CreateHydraulicBoundaryLocations(IEnumerable<ReadHydraulicBoundaryLocation> readLocations,
                                                                                               IEnumerable<ReadHydraulicLocation> readHydraulicLocations,
                                                                                               long[] excludedLocationIds)
        {
            Array.Sort(excludedLocationIds);

            foreach (ReadHydraulicBoundaryLocation readLocation in readLocations)
            {
                long locationConfigurationId = readHydraulicLocations.Where(m => m.HrdLocationId == readLocation.Id)
                                                                     .Select(m => m.HlcdLocationId)
                                                                     .SingleOrDefault();

                if (locationConfigurationId != 0 && ShouldInclude(excludedLocationIds, locationConfigurationId))
                {
                    yield return new HydraulicBoundaryLocation(locationConfigurationId, readLocation.Name,
                                                               readLocation.CoordinateX, readLocation.CoordinateY);
                }
            }
        }

        private static bool ShouldInclude(long[] excludedLocationIds, long locationId)
        {
            return Array.BinarySearch(excludedLocationIds, locationId) < 0;
        }
    }
}