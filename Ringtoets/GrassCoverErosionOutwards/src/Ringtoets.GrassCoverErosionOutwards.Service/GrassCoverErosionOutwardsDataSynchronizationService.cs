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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Utils.Extensions;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion outwards data.
    /// </summary>
    public static class GrassCoverErosionOutwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output of the given <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
        /// to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        public static void ClearWaveConditionsCalculationOutput(GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            calculation.Output = null;
        }

        /// <summary>
        /// Clears the output of the grass cover erosion outwards hydraulic boundary locations within the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="locations">The locations for which the output needs to be cleared.</param>
        /// <returns><c>true</c> when one or multiple locations are affected by clearing the output. <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> is <c>null</c>.</exception>
        public static bool ClearHydraulicBoundaryLocationOutput(ObservableList<HydraulicBoundaryLocation> locations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }

            var locationsAffected = false;

            foreach (var location in locations.Where(location =>
                                                     !double.IsNaN(location.DesignWaterLevel) ||
                                                     !double.IsNaN(location.WaveHeight)))
            {
                location.DesignWaterLevel = RoundedDouble.NaN;
                location.WaveHeight = RoundedDouble.NaN;
                location.DesignWaterLevelCalculationConvergence = CalculationConvergence.NotCalculated;
                location.WaveHeightCalculationConvergence = CalculationConvergence.NotCalculated;
                locationsAffected = true;
            }

            return locationsAffected;
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the wave conditions calculations
        /// in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// removal of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            Collection<GrassCoverErosionOutwardsWaveConditionsCalculation> affectedItems = new Collection<GrassCoverErosionOutwardsWaveConditionsCalculation>();
            foreach (var calculation in failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>())
            {
                var calculationChanged = false;

                if (calculation.HasOutput)
                {
                    ClearWaveConditionsCalculationOutput(calculation);
                    calculationChanged = true;
                }

                if (calculation.InputParameters.HydraulicBoundaryLocation != null)
                {
                    ClearHydraulicBoundaryLocation(calculation);
                    calculationChanged = true;
                }

                if (calculationChanged)
                {
                    affectedItems.Add(calculation);
                }
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output for all calculations in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> ClearAllWaveConditionsCalculationOutput(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = failureMechanism.Calculations
                                                .Where(c => c.HasOutput)
                                                .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                .ToArray();

            affectedItems.ForEachElementDo(ClearWaveConditionsCalculationOutput);

            return affectedItems;
        }

        private static void ClearHydraulicBoundaryLocation(GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            calculation.InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}