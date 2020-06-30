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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// Representation of a <see cref="StructuresCalculationScenario{T}"/>
    /// which takes care of the representation of properties in a grid.
    /// </summary>
    public class StabilityPointStructuresScenarioRow : ScenarioRow<StructuresCalculationScenario<StabilityPointStructuresInput>>
    {
        private readonly StabilityPointStructuresFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private ProbabilityAssessmentOutput probabilityAssessmentOutput;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresScenarioRow"/>.
        /// <param name="calculationScenario">The <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/> this row contains.</param>
        /// <param name="failureMechanism">The failure mechanism that the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// </summary>
        public StabilityPointStructuresScenarioRow(StructuresCalculationScenario<StabilityPointStructuresInput> calculationScenario,
                                                   StabilityPointStructuresFailureMechanism failureMechanism,
                                                   IAssessmentSection assessmentSection)
            : base(calculationScenario)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            CreateProbabilityAssessmentOutput();
        }

        public override double FailureProbability => probabilityAssessmentOutput?.Probability ?? double.NaN;

        public override void Update()
        {
            CreateProbabilityAssessmentOutput();
        }

        private void CreateProbabilityAssessmentOutput()
        {
            probabilityAssessmentOutput = CalculationScenario.HasOutput
                                              ? StabilityPointStructuresProbabilityAssessmentOutputFactory.Create(
                                                  CalculationScenario.Output, failureMechanism, assessmentSection)
                                              : null;
        }
    }
}