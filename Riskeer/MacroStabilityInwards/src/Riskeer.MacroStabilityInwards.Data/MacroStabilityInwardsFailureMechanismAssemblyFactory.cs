// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;

namespace Riskeer.MacroStabilityInwards.Data
{
    public static class MacroStabilityInwardsFailureMechanismAssemblyFactory
    {
        public static FailureMechanismSectionAssemblyResult AssembleSection(AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                            MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                            IAssessmentSection assessmentSection)
        {
            MacroStabilityInwardsCalculationScenario[] calculationScenarios = failureMechanism.Calculations
                                                                                              .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                              .ToArray();

            return FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                sectionResult, assessmentSection,
                new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(sectionResult, calculationScenarios, failureMechanism),
                failureMechanism.GeneralInput.ApplyLengthEffectInSection,
                failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(sectionResult.Section.Length));
        }
    }
}