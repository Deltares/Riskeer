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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Activities
{
    /// <summary>
    /// Factory for creating Hydra-Ring activities.
    /// </summary>
    /// <remarks>The current implementation of this factory is not thread safe (calculations should be performed one at a time).</remarks>
    public static class HydraRingActivityFactory
    {
        private static readonly HydraRingCalculationService hydraRingCalculationService = new HydraRingCalculationService();

        /// <summary>
        /// Creates a new instance of the <see cref="TargetProbabilityCalculationActivity"/> class.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="targetProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <param name="handleCalculationOutputAction">The action to perform after the calculation is performed.</param>
        public static TargetProbabilityCalculationActivity Create(string name,
                                                                  string hlcdDirectory,
                                                                  string ringId,
                                                                  HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
                                                                  HydraRingUncertaintiesType uncertaintiesType,
                                                                  TargetProbabilityCalculationInput targetProbabilityCalculationInput,
                                                                  Action<TargetProbabilityCalculationOutput> handleCalculationOutputAction)
        {
            return new TargetProbabilityCalculationActivity(name, hlcdDirectory, ringId, timeIntegrationSchemeType, uncertaintiesType, targetProbabilityCalculationInput, handleCalculationOutputAction, hydraRingCalculationService);
        }
    }
}
