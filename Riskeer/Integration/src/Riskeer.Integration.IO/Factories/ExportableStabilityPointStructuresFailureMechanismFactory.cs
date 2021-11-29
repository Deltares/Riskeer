﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Structures;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Helpers;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// with assembly results for stability point structures.
    /// </summary>
    public static class ExportableStabilityPointStructuresFailureMechanismFactory
    {
        private const ExportableFailureMechanismType failureMechanismCode = ExportableFailureMechanismType.STKWp;
        private const ExportableFailureMechanismGroup failureMechanismGroup = ExportableFailureMechanismGroup.Group1;
        private const ExportableAssemblyMethod failureMechanismAssemblyMethod = ExportableAssemblyMethod.WBI1B1;

        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StabilityPointStructuresFailureMechanism"/> to create an
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>An <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateExportableFailureMechanism(
            StabilityPointStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!failureMechanism.InAssembly)
            {
                return ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(assessmentSection,
                                                                                                                failureMechanismCode,
                                                                                                                failureMechanismGroup,
                                                                                                                failureMechanismAssemblyMethod);
            }

            FailureMechanismAssembly failureMechanismAssembly = StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection, false);

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                new ExportableFailureMechanismAssemblyResultWithProbability(failureMechanismAssemblyMethod,
                                                                            failureMechanismAssembly.Group,
                                                                            failureMechanismAssembly.Probability),
                CreateExportableFailureMechanismSectionResults(failureMechanism, assessmentSection),
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>
        /// with assembly results based on <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StabilityPointStructuresFailureMechanism"/>
        /// to create a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> CreateExportableFailureMechanismSectionResults(
            StabilityPointStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            IDictionary<StabilityPointStructuresFailureMechanismSectionResultOld, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanism.SectionResultsOld);

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios = failureMechanism.Calculations.Cast<StructuresCalculationScenario<StabilityPointStructuresInput>>().ToArray();

            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>();
            foreach (KeyValuePair<StabilityPointStructuresFailureMechanismSectionResultOld, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSectionsLookup)
            {
                StabilityPointStructuresFailureMechanismSectionResultOld failureMechanismSectionResult = failureMechanismSectionPair.Key;

                FailureMechanismSectionAssemblyOld simpleAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyOld detailedAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleDetailedAssessment(failureMechanismSectionResult,
                                                                                                       calculationScenarios,
                                                                                                       failureMechanism,
                                                                                                       assessmentSection);
                FailureMechanismSectionAssemblyOld tailorMadeAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult,
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);
                FailureMechanismSectionAssemblyOld combinedAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult,
                                                                                                       calculationScenarios,
                                                                                                       failureMechanism,
                                                                                                       assessmentSection);

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                        failureMechanismSectionPair.Value,
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(simpleAssembly, ExportableAssemblyMethod.WBI0E3),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(detailedAssembly, ExportableAssemblyMethod.WBI0G3),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T3),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(combinedAssembly, ExportableAssemblyMethod.WBI0A1)));
            }

            return exportableResults;
        }
    }
}