// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Riskeer.Piping.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a piping calculation.
    /// </summary>
    internal class PipingCalculationActivity : CalculatableActivity
    {
        private readonly PipingCalculation calculation;
        private readonly RoundedDouble normativeAssessmentLevel;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The piping calculation to perform.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public PipingCalculationActivity(PipingCalculation calculation,
                                         RoundedDouble normativeAssessmentLevel)
            : base(calculation)
        {
            this.calculation = calculation;
            this.normativeAssessmentLevel = normativeAssessmentLevel;

            Description = string.Format(RingtoetsCommonServiceResources.Perform_calculation_with_name_0_, calculation.Name);
        }

        protected override void PerformCalculation()
        {
            PipingCalculationService.Calculate(calculation, normativeAssessmentLevel);
        }

        protected override bool Validate()
        {
            return PipingCalculationService.Validate(calculation, normativeAssessmentLevel);
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