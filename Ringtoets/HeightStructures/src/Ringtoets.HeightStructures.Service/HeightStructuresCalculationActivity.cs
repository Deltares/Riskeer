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

using System.Linq;
using Core.Common.Base.Service;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data.Output;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// <see cref="Activity"/> for running a height structures calculation.
    /// </summary>
    public class HeightStructuresCalculationActivity : HydraRingActivity<ExceedanceProbabilityCalculationOutput>
    {
        private readonly HeightStructuresCalculation calculation;
        private readonly string hlcdDirectory;
        private readonly HeightStructuresFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        public override string Name
        {
            get
            {
                return calculation.Name;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationActivity"/>.
        /// </summary>
        /// <param name="calculation">The height structures data used for the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        public HeightStructuresCalculationActivity(HeightStructuresCalculation calculation, string hlcdDirectory, 
                                                   HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            this.calculation = calculation;
            this.hlcdDirectory = hlcdDirectory;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        protected override void OnRun()
        {
            var failureMechanismSection = failureMechanism.Sections.First(); // TODO: Obtain dike section based on cross section of structure with reference line

            PerformRun(() => HeightStructuresCalculationService.Validate(calculation, assessmentSection),
                       () => calculation.ClearOutput(),
                       () => HeightStructuresCalculationService.Calculate(calculation,
                                                                          hlcdDirectory,
                                                                          failureMechanismSection,
                                                                          failureMechanismSection.Name, // TODO: Provide name of reference line instead
                                                                          failureMechanism.GeneralInput));
        }

        protected override void OnFinish()
        {
            PerformFinish(() =>
            {
                calculation.Output = ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm,
                                                                            failureMechanism.Contribution,
                                                                            failureMechanism.GeneralInput.N,
                                                                            Output.Beta);
            }, calculation);
        }
    }
}