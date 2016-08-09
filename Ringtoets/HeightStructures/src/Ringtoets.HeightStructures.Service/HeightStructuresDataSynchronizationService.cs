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
using System.Linq;
using Core.Common.Utils.Extensions;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// Service for synchronizing height structures data.
    /// </summary>
    public static class HeightStructuresDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="HeightStructuresFailureMechanism"/> which contains the calculations.</param>
        public static void ClearAllCalculationOutput(HeightStructuresFailureMechanism failureMechanism)
        {
            failureMechanism.Calculations
                            .Cast<HeightStructuresCalculation>()
                            .Where(c => c.HasOutput)
                            .ForEachElementDo(ClearCalculationOutput);
        }

        /// <summary>
        /// Clears the output of the given <see cref="HeightStructuresCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static void ClearCalculationOutput(HeightStructuresCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            calculation.Output = null;
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> for all the calculations in the <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="HeightStructuresFailureMechanism"/> which contains the calculations.</param>
        public static void ClearHydraulicBoundaryLocations(HeightStructuresFailureMechanism failureMechanism)
        {
            failureMechanism.Calculations
                            .Cast<HeightStructuresCalculation>()
                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null)
                            .ForEachElementDo(ClearHydraulicBoundaryLocation);
        }

        private static void ClearHydraulicBoundaryLocation(HeightStructuresCalculation calculation)
        {
            calculation.InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}