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
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a piping calculation.
    /// </summary>
    public class PipingCalculationActivity : Activity
    {
        private readonly double norm;
        private readonly double contribution;
        private readonly PipingCalculation calculation;
        private readonly PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationActivity"/> class.
        /// </summary>
        /// <param name="calculation">The piping data used for the calculation.</param>
        /// <param name="pipingProbabilityAssessmentInput">General input that influences the probability estimate for a piping
        /// assessment.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <param name="contribution">The contribution of piping as a percentage (0-100) to the total of the failure probability
        /// of the assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any <paramref name="calculation"/> 
        /// or <paramref name="pipingProbabilityAssessmentInput"/> is <c>null</c>.</exception>
        public PipingCalculationActivity(PipingCalculation calculation, PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput,
                                         double norm, double contribution)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (pipingProbabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(pipingProbabilityAssessmentInput));
            }

            this.calculation = calculation;
            this.pipingProbabilityAssessmentInput = pipingProbabilityAssessmentInput;
            this.norm = norm;
            this.contribution = contribution;

            Name = calculation.Name;
        }

        protected override void OnRun()
        {
            if (!PipingCalculationService.Validate(calculation))
            {
                State = ActivityState.Failed;
                return;
            }

            PipingDataSynchronizationService.ClearCalculationOutput(calculation);

            PipingCalculationService.Calculate(calculation);
            PipingSemiProbabilisticCalculationService.Calculate(calculation, pipingProbabilityAssessmentInput, norm, contribution);
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