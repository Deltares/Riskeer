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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// View for configuring macrostability inwards calculation scenarios.
    /// </summary>
    public class MacroStabilityInwardsScenariosView : ScenariosView<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput, MacroStabilityInwardsScenarioRow, MacroStabilityInwardsFailureMechanism>
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// to get the sections from.</param>
        /// <param name="assessmentSection">The assessment section the scenarios belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsScenariosView(CalculationGroup calculationGroup, MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(calculationGroup, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        protected override MacroStabilityInwardsInput GetCalculationInput(MacroStabilityInwardsCalculationScenario calculationScenario)
        {
            return calculationScenario.InputParameters;
        }

        protected override IEnumerable<MacroStabilityInwardsScenarioRow> GetScenarioRows(FailureMechanismSection failureMechanismSection)
        {
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = CalculationGroup
                                                                                 .GetCalculations()
                                                                                 .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                 .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            return calculations.Select(pc => new MacroStabilityInwardsScenarioRow(pc, FailureMechanism, assessmentSection)).ToList();
        }
    }
}