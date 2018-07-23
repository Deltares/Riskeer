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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service.Properties;

namespace Ringtoets.StabilityStoneCover.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a stability stone cover wave conditions calculation.
    /// </summary>
    internal class StabilityStoneCoverWaveConditionsCalculationActivity : CalculatableActivity
    {
        private readonly StabilityStoneCoverWaveConditionsCalculation calculation;
        private readonly string hlcdFilePath;
        private readonly StabilityStoneCoverFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private readonly StabilityStoneCoverWaveConditionsCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The stability stone cover wave conditions data used for the calculation.</param>
        /// <param name="hlcdFilePath">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationActivity(StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                    string hlcdFilePath,
                                                                    StabilityStoneCoverFailureMechanism failureMechanism,
                                                                    IAssessmentSection assessmentSection)
            : base(calculation)
        {
            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.calculation = calculation;
            this.hlcdFilePath = hlcdFilePath;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            Description = string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_Perform_calculation_with_name_0_, calculation.Name);

            calculationService = new StabilityStoneCoverWaveConditionsCalculationService();
        }

        protected override bool Validate()
        {
            return StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation,
                                                                                assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                                     calculation.InputParameters.CategoryType),
                                                                                hlcdFilePath,
                                                                                assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                                                assessmentSection.GetNorm(calculation.InputParameters.CategoryType));
        }

        protected override void PerformCalculation()
        {
            calculationService.OnProgress = UpdateProgressText;

            StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);
            calculationService.Calculate(
                calculation, assessmentSection, failureMechanism.GeneralInput, hlcdFilePath);
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