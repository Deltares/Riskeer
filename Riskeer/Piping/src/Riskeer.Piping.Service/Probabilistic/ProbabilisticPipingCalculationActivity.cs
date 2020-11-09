// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.Piping.Service.Probabilistic
{
    /// <summary>
    /// <see cref="CalculatableActivity"/> for running a <see cref="ProbabilisticPipingCalculation"/>.
    /// </summary>
    public class ProbabilisticPipingCalculationActivity : CalculatableActivity
    {
        private readonly ProbabilisticPipingCalculation calculation;
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private readonly ProbabilisticPipingCalculationService service;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculation"/> to perform.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticPipingCalculationActivity(ProbabilisticPipingCalculation calculation,
                                                      PipingFailureMechanism failureMechanism,
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

            Description = string.Format(RiskeerCommonServiceResources.Perform_calculation_with_name_0_, calculation.Name);

            service = new ProbabilisticPipingCalculationService();
            service.OnProgressChanged += UpdateProgressText;
        }

        protected override void OnCancel()
        {
            service.Cancel();
        }

        protected override void OnFinish()
        {
            calculation.NotifyObservers();
        }

        protected override void PerformCalculation()
        {
            calculation.ClearOutput();

            FailureMechanismSection section = failureMechanism.Sections.Single(
                s => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(
                    Math2D.ConvertPointsToLineSegments(s.Points)));

            service.Calculate(calculation, failureMechanism.GeneralInput,
                              HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                              section.Length);
        }

        protected override bool Validate()
        {
            return ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);
        }
    }
}