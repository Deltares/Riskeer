// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableFailureMechanismSectionResult"/> in structures.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
    /// <typeparam name="TStructuresInput">The type of input.</typeparam>
    public class StructuresFailureMechanismResultView<TFailureMechanism, TStructuresInput> : AdoptableFailureMechanismResultView<
        TFailureMechanism, StructuresCalculationScenario<TStructuresInput>, TStructuresInput>
        where TFailureMechanism : IFailureMechanism<AdoptableFailureMechanismSectionResult>, ICalculatableFailureMechanism, IFailureMechanism
        where TStructuresInput : IStructuresCalculationInput<StructureBase>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructuresFailureMechanismResultView{TFailureMechanism,TStructuresInput}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="AdoptableFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <param name="performFailureMechanismAssemblyFunc">The function to perform an assembly on the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StructuresFailureMechanismResultView(IObservableEnumerable<AdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                                                    TFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection,
                                                    Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
            : base(failureMechanismSectionResults, failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc,
                   (sr, fm, ass) => StructuresFailureMechanismAssemblyFactory.AssembleSection<TStructuresInput>(sr, fm, ass)) {}

        protected override IFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                              IEnumerable<StructuresCalculationScenario<TStructuresInput>> calculationScenarios)
        {
            return new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TStructuresInput>(sectionResult, calculationScenarios);
        }

        protected override IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider CreateErrorProvider(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                                       IEnumerable<StructuresCalculationScenario<TStructuresInput>> calculationScenarios)
        {
            return new FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<StructuresCalculationScenario<TStructuresInput>>(
                sectionResult, calculationScenarios,
                (scenario, lineSegments) => scenario.IsStructureIntersectionWithReferenceLineInSection(lineSegments));
        }

        protected override IEnumerable<StructuresCalculationScenario<TStructuresInput>> GetCalculationScenarios(AdoptableFailureMechanismSectionResult sectionResult)
        {
            return FailureMechanism.Calculations
                                   .OfType<StructuresCalculationScenario<TStructuresInput>>()
                                   .ToArray();
        }
    }
}