﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// Service for synchronizing piping data.
    /// </summary>
    public static class PipingDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> which contains the calculations.</param>
        public static void ClearAllCalculationOutput(PipingFailureMechanism failureMechanism)
        {
            failureMechanism.Calculations
                            .Cast<PipingCalculation>()
                            .Where(c => c.HasOutput)
                            .ForEachElementDo(ClearCalculationOutput);
        }

        /// <summary>
        /// Clears the output of the given <see cref="PipingCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static void ClearCalculationOutput(PipingCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            calculation.Output = null;
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> for all the calculations in the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> which contains the calculations.</param>
        public static void ClearHydraulicBoundaryLocations(PipingFailureMechanism failureMechanism)
        {
            failureMechanism.Calculations
                            .Cast<PipingCalculation>()
                            .Where(c => c.InputParameters.HydraulicBoundaryLocation != null)
                            .ForEachElementDo(ClearHydraulicBoundaryLocation);
        }

        private static void ClearHydraulicBoundaryLocation(PipingCalculation calculation)
        {
            calculation.InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}