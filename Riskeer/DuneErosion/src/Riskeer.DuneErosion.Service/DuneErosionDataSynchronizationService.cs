// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.IO;

namespace Riskeer.DuneErosion.Service
{
    /// <summary>
    /// Service for synchronizing dune erosion data.
    /// </summary>
    public static class DuneErosionDataSynchronizationService
    {
        /// <summary>
        /// Sets <see cref="DuneErosionFailureMechanism.DuneLocations"/> based upon 
        /// the <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to use.</param>
        /// <param name="readDuneLocations">The read dune locations to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void SetDuneLocations(DuneErosionFailureMechanism failureMechanism,
                                            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                            IEnumerable<ReadDuneLocation> readDuneLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            if (readDuneLocations == null)
            {
                throw new ArgumentNullException(nameof(readDuneLocations));
            }

            if (!hydraulicBoundaryLocations.Any() || !readDuneLocations.Any())
            {
                return;
            }

            failureMechanism.SetDuneLocations(GetDuneLocationsToSet(hydraulicBoundaryLocations, readDuneLocations));
        }

        /// <summary>
        /// Clears the output of the dune location calculations within the dune erosion failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism for which the output of the calculations needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearDuneLocationCalculationsOutput(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedCalculations = new List<IObservable>();

            foreach (DuneLocationCalculationsForTargetProbability calculationsForTargetProbability in failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities)
            {
                affectedCalculations.AddRange(ClearDuneLocationCalculationsOutput(calculationsForTargetProbability.DuneLocationCalculations));
            }

            return affectedCalculations;
        }

        /// <summary>
        /// Clears the output of the provided dune location calculations.
        /// </summary>
        /// <param name="calculations">The calculations for which the output needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearDuneLocationCalculationsOutput(IEnumerable<DuneLocationCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            IEnumerable<DuneLocationCalculation> affectedCalculations = calculations.Where(c => c.Output != null).ToArray();

            affectedCalculations.ForEachElementDo(c => c.Output = null);

            return affectedCalculations;
        }

        private static IEnumerable<DuneLocation> GetDuneLocationsToSet(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, IEnumerable<ReadDuneLocation> readDuneLocations)
        {
            Dictionary<string, ReadDuneLocation> readDuneLocationsLookup = readDuneLocations.ToDictionary(rdl => rdl.Name, rdl => rdl);

            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in hydraulicBoundaryLocations)
            {
                if (readDuneLocationsLookup.TryGetValue(hydraulicBoundaryLocation.Name, out ReadDuneLocation correspondingReadDuneLocation))
                {
                    yield return new DuneLocation(hydraulicBoundaryLocation.Name,
                                                  hydraulicBoundaryLocation,
                                                  new DuneLocation.ConstructionProperties
                                                  {
                                                      CoastalAreaId = correspondingReadDuneLocation.CoastalAreaId,
                                                      Offset = correspondingReadDuneLocation.Offset / 10.0
                                                  });
                }
            }
        }
    }
}