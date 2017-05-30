// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Service;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsCalculationActivity : Activity
    {
        private readonly double norm;
        private readonly double contribution;
        private readonly MacroStabilityInwardsCalculation calculation;
        private readonly MacroStabilityInwardsProbabilityAssessmentInput macroStabilityInwardsProbabilityAssessmentInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsCalculationActivity"/> class.
        /// </summary>
        /// <param name="calculation">The macro stability inwards data used for the calculation.</param>
        /// <param name="macroStabilityInwardsProbabilityAssessmentInput">General input that influences the probability estimate for a
        /// macro stability inwards assessment.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <param name="contribution">The contribution of macro stability inwards as a percentage (0-100) to the total of the failure probability
        /// of the assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any <paramref name="calculation"/> 
        /// or <paramref name="macroStabilityInwardsProbabilityAssessmentInput"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculationActivity(MacroStabilityInwardsCalculation calculation, MacroStabilityInwardsProbabilityAssessmentInput macroStabilityInwardsProbabilityAssessmentInput,
                                                        double norm, double contribution)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (macroStabilityInwardsProbabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(macroStabilityInwardsProbabilityAssessmentInput));
            }

            this.calculation = calculation;
            this.macroStabilityInwardsProbabilityAssessmentInput = macroStabilityInwardsProbabilityAssessmentInput;
            this.norm = norm;
            this.contribution = contribution;

            Name = calculation.Name;
        }

        protected override void OnRun()
        {
            if (!MacroStabilityInwardsCalculationService.Validate(calculation))
            {
                State = ActivityState.Failed;
                return;
            }

            LogMessages.Clear();
            MacroStabilityInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

            MacroStabilityInwardsCalculationService.Calculate(calculation);
            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation, macroStabilityInwardsProbabilityAssessmentInput, norm, contribution);
        }

        protected override void OnCancel()
        {
            // Unable to cancel a running kernel, so nothing can be done.
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }
    }
}