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
        /// <param name="beforeRunAction">The action to perform before the calculation is performed.</param>
        /// <param name="handleOutputAction">The action to perform after the calculation is performed.</param>
        /// <exception cref="ArgumentException">Thrown when one of the <c>string</c> arguments is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when one of the other arguments is <c>null</c>.</exception>
        public static TargetProbabilityCalculationActivity Create(string name,
                                                                  string hlcdDirectory,
                                                                  string ringId,
                                                                  HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
                                                                  HydraRingUncertaintiesType uncertaintiesType,
                                                                  TargetProbabilityCalculationInput targetProbabilityCalculationInput,
                                                                  Action beforeRunAction,
                                                                  Action<TargetProbabilityCalculationOutput> handleOutputAction)
        {
            VerifyCalculationActivityInput(hlcdDirectory, ringId, timeIntegrationSchemeType,
                                           uncertaintiesType, targetProbabilityCalculationInput,
                                           beforeRunAction, handleOutputAction);

            return new TargetProbabilityCalculationActivity(name, hlcdDirectory, ringId, timeIntegrationSchemeType,
                                                            uncertaintiesType, targetProbabilityCalculationInput,
                                                            beforeRunAction, handleOutputAction,
                                                            hydraRingCalculationService);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ExceedanceProbabilityCalculationActivity"/> class.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while executing the calculation.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="exceedanceProbabilityCalculationInput">The input of the calculation to perform.</param>
        /// <param name="beforeRunAction">The action to perform before the calculation is performed.</param>
        /// <param name="handleOutputAction">The action to perform after the calculation is performed.</param>
        /// <exception cref="ArgumentException">Thrown when one of the <c>string</c> arguments is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when one of the other arguments is <c>null</c>.</exception>
        public static ExceedanceProbabilityCalculationActivity Create(string name,
                                                                      string hlcdDirectory,
                                                                      string ringId,
                                                                      HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
                                                                      HydraRingUncertaintiesType uncertaintiesType,
                                                                      ExceedanceProbabilityCalculationInput exceedanceProbabilityCalculationInput,
                                                                      Action beforeRunAction,
                                                                      Action<ExceedanceProbabilityCalculationOutput> handleOutputAction)
        {
            VerifyCalculationActivityInput(hlcdDirectory, ringId, timeIntegrationSchemeType,
                                           uncertaintiesType, exceedanceProbabilityCalculationInput,
                                           beforeRunAction, handleOutputAction);

            return new ExceedanceProbabilityCalculationActivity(name, hlcdDirectory, ringId, timeIntegrationSchemeType,
                                                                uncertaintiesType, exceedanceProbabilityCalculationInput,
                                                                beforeRunAction, handleOutputAction,
                                                                hydraRingCalculationService);
        }

        private static void VerifyCalculationActivityInput(string hlcdDirectory, string ringId, HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType,
                                                           HydraRingUncertaintiesType uncertaintiesType, HydraRingCalculationInput hydraRingCalculationInput,
                                                           object beforeRunAction, object handleOutputAction)
        {
            if (string.IsNullOrEmpty(hlcdDirectory))
            {
                throw new ArgumentException(@"HLCD directory should be set.", "hlcdDirectory");
            }

            if (string.IsNullOrEmpty(ringId))
            {
                throw new ArgumentException(@"Ring id should be set.", "ringId");
            }

            if (hydraRingCalculationInput == null)
            {
                throw new ArgumentNullException("hydraRingCalculationInput", @"Calculation input should be set.");
            }

            if (beforeRunAction == null)
            {
                throw new ArgumentNullException("beforeRunAction", @"Before calculation run action should be set.");
            }

            if (handleOutputAction == null)
            {
                throw new ArgumentNullException("handleOutputAction", @"Handle calculation output action should be set.");
            }
        }
    }
}