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
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsCalculationScenario"/> in the <see cref="MacroStabilityInwardsScenariosView"/>.
    /// </summary>
    public class MacroStabilityInwardsScenarioRow : ScenarioRow<MacroStabilityInwardsCalculationScenario>
    {
        private readonly MacroStabilityInwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private DerivedMacroStabilityInwardsOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="MacroStabilityInwardsCalculationScenario"/> this row contains.</param>
        /// <param name="failureMechanism">The failure mechanism that the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal MacroStabilityInwardsScenarioRow(MacroStabilityInwardsCalculationScenario calculationScenario,
                                                MacroStabilityInwardsFailureMechanism failureMechanism,
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

            CreateDerivedOutput();
        }

        public override double FailureProbability => derivedOutput?.MacroStabilityInwardsProbability ?? double.NaN;

        /// <summary>
        /// Updates the row based on the current output of the calculation scenario.
        /// </summary>
        public override void Update()
        {
            CreateDerivedOutput();
        }

        private void CreateDerivedOutput()
        {
            derivedOutput = CalculationScenario.HasOutput
                                ? DerivedMacroStabilityInwardsOutputFactory.Create(CalculationScenario.Output, failureMechanism, assessmentSection)
                                : null;
        }
    }
}