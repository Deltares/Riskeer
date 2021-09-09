// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Service
{
    /// <summary>
    /// Service for synchronizing wave conditions data.
    /// </summary>
    public static class WaveConditionsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations that corresponds with the <paramref name="normType"/>
        /// in the given <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism which contains the calculations.</param>
        /// <param name="normType">The <see cref="NormType"/> to clear for.</param>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
        /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> is invalid.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is not supported.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutput<TFailureMechanism, TCalculation>(
            TFailureMechanism failureMechanism, NormType normType)
            where TFailureMechanism : IFailureMechanism
            where TCalculation : ICalculation<WaveConditionsInput>
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            WaveConditionsInputWaterLevelType waterLevelType = GetWaterLevelTypeFromNormType(normType);

            var affectedItems = new List<IObservable>();
            foreach (TCalculation calculation in failureMechanism.Calculations
                                                                 .Cast<TCalculation>()
                                                                 .Where(c => c.InputParameters.WaterLevelType == waterLevelType))
            {
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output for all calculations that corresponds with the <paramref name="calculationsForTargetProbability"/>
        /// in the given <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism which contains the calculations.</param>
        /// <param name="calculationsForTargetProbability">The <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>
        /// to clear for.</param>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
        /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutput<TFailureMechanism, TCalculation>(
            TFailureMechanism failureMechanism, HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability)
            where TFailureMechanism : IFailureMechanism
            where TCalculation : ICalculation<WaveConditionsInput>
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            var affectedItems = new List<IObservable>();
            foreach (TCalculation calculation in failureMechanism.Calculations
                                                                 .Cast<TCalculation>()
                                                                 .Where(c => c.InputParameters.WaterLevelType == WaveConditionsInputWaterLevelType.UserDefinedTargetProbability
                                                                             && c.InputParameters.CalculationsTargetProbability == calculationsForTargetProbability))
            {
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            return affectedItems;
        }

        /// <summary>
        /// Gets the <see cref="WaveConditionsInputWaterLevelType"/> based on the given <paramref name="normType"/>.
        /// </summary>
        /// <param name="normType">The <see cref="NormType"/> to get the <see cref="WaveConditionsInputWaterLevelType"/> from.</param>
        /// <returns>A <see cref="WaveConditionsInputWaterLevelType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> is invalid.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is not supported.</exception>
        private static WaveConditionsInputWaterLevelType GetWaterLevelTypeFromNormType(NormType normType)
        {
            if (!Enum.IsDefined(typeof(NormType), normType))
            {
                throw new InvalidEnumArgumentException(nameof(normType),
                                                       (int) normType,
                                                       typeof(NormType));
            }

            switch (normType)
            {
                case NormType.LowerLimit:
                    return WaveConditionsInputWaterLevelType.LowerLimit;
                case NormType.Signaling:
                    return WaveConditionsInputWaterLevelType.Signaling;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}