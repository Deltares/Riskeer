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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.Revetment.Service;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service.Properties;

namespace Riskeer.StabilityStoneCover.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a stability stone cover wave conditions calculation.
    /// </summary>
    internal class StabilityStoneCoverWaveConditionsCalculationActivity : CalculatableActivity
    {
        private readonly StabilityStoneCoverWaveConditionsCalculation calculation;
        private readonly StabilityStoneCoverFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private readonly StabilityStoneCoverWaveConditionsCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The stability stone cover wave conditions data used for the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationActivity(StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                    StabilityStoneCoverFailureMechanism failureMechanism,
                                                                    IAssessmentSection assessmentSection)
            : base(calculation)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.calculation = calculation;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            Description = string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_Perform_calculation_with_name_0_, calculation.Name);

            calculationService = new StabilityStoneCoverWaveConditionsCalculationService();
        }

        protected override bool Validate()
        {
            return WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                                 assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                      calculation.InputParameters.CategoryType),
                                                                 assessmentSection.HydraulicBoundaryDatabase,
                                                                 assessmentSection.GetNorm(calculation.InputParameters.CategoryType));
        }

        protected override void PerformCalculation()
        {
            calculationService.OnProgressChanged += UpdateProgressText;

            StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);
            calculationService.Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }
    }
}