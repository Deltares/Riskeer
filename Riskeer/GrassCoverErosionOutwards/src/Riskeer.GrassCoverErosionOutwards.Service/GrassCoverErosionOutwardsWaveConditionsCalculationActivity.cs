﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Service;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a grass cover erosion outwards wave conditions calculation.
    /// </summary>
    internal class GrassCoverErosionOutwardsWaveConditionsCalculationActivity : CalculatableActivity
    {
        private readonly GrassCoverErosionOutwardsWaveConditionsCalculation calculation;
        private readonly GrassCoverErosionOutwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private readonly GrassCoverErosionOutwardsWaveConditionsCalculationService calculationService;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The grass cover erosion outwards wave conditions data used for the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveConditionsCalculationActivity(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                          GrassCoverErosionOutwardsFailureMechanism failureMechanism,
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

            Description = string.Format(RiskeerCommonServiceResources.Perform_wave_conditions_calculation_with_name_0_, calculation.Name);

            calculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
        }

        protected override bool Validate()
        {
            return WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                                 WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, assessmentSection),
                                                                 assessmentSection.HydraulicBoundaryDatabase);
        }

        protected override void PerformCalculation()
        {
            calculationService.OnProgressChanged += UpdateProgressText;

            RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation);
            calculationService.Calculate(calculation, failureMechanism, assessmentSection);
        }

        protected override void OnCancel()
        {
            calculationService.Cancel();
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }
    }
}