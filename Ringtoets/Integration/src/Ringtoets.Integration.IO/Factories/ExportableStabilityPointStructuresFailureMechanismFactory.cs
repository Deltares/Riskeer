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
using System.Collections.Generic;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.Integration.IO.Factories
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
        /// <param name="assessmentSection">The assessment section this failure mechanism belongs to.</param>
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

            if (!failureMechanism.IsRelevant)
            {
                return ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(
                    failureMechanismCode, failureMechanismGroup, failureMechanismAssemblyMethod);
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
            IDictionary<StabilityPointStructuresFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanism.SectionResults);

            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>();
            foreach (KeyValuePair<StabilityPointStructuresFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSectionsLookup)
            {
                StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult = failureMechanismSectionPair.Key;

                FailureMechanismSectionAssembly simpleAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssembly detailedAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleDetailedAssessment(failureMechanismSectionResult,
                                                                                                       failureMechanism,
                                                                                                       assessmentSection);
                FailureMechanismSectionAssembly tailorMadeAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult,
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);
                FailureMechanismSectionAssembly combinedAssembly =
                    StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult,
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