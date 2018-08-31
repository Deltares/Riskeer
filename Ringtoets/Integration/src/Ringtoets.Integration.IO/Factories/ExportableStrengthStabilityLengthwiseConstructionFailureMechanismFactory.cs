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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// with assembly results for strength stability lengthwise construction.
    /// </summary>
    public static class ExportableStrengthStabilityLengthwiseConstructionFailureMechanismFactory
    {
        private const ExportableFailureMechanismType failureMechanismCode = ExportableFailureMechanismType.STKWl;
        private const ExportableFailureMechanismGroup failureMechanismGroup = ExportableFailureMechanismGroup.Group4;
        private const ExportableAssemblyMethod failureMechanismAssemblyMethod = ExportableAssemblyMethod.WBI1A1;

        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/> to create an
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> for.</param>
        /// <returns>An <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateExportableFailureMechanism(
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (!failureMechanism.IsRelevant)
            {
                return ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(failureMechanismCode,
                                                                                                                   failureMechanismGroup,
                                                                                                                   failureMechanismAssemblyMethod);
            }

            FailureMechanismAssemblyCategoryGroup failureMechanismAssembly = StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);
            IDictionary<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanism.SectionResults);
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                new ExportableFailureMechanismAssemblyResult(failureMechanismAssemblyMethod,
                                                             failureMechanismAssembly),
                CreateFailureMechanismSectionResults(failureMechanismSectionsLookup),
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly"/>
        /// with assembly results based on the sections in <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The mapping between the <see cref="StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult"/>
        /// and <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly> CreateFailureMechanismSectionResults(
            IDictionary<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSections)
        {
            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly>();
            foreach (KeyValuePair<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSections)
            {
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult failureMechanismSectionResult = failureMechanismSectionPair.Key;
                FailureMechanismSectionAssemblyCategoryGroup simpleAssembly =
                    StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly =
                    StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyCategoryGroup combinedAssembly =
                    StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult);

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                        failureMechanismSectionPair.Value,
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(simpleAssembly, ExportableAssemblyMethod.WBI0E1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(combinedAssembly, ExportableAssemblyMethod.WBI0A1)));
            }

            return exportableResults;
        }
    }
}