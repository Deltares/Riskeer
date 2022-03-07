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
using Riskeer.AssemblyTool.Data.Old;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism"/>
    /// with assembly results for grass cover erosion outwards.
    /// </summary>
    public static class ExportableGrassCoverErosionOutwardsFailureMechanismFactory
    {
        private const ExportableFailureMechanismType failureMechanismCode = ExportableFailureMechanismType.GEBU;
        private const ExportableAssemblyMethod failureMechanismAssemblyMethod = ExportableAssemblyMethod.WBI1A1;

        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanism"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to create an
        /// <see cref="ExportableFailureMechanism"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>An <see cref="ExportableFailureMechanism"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism CreateExportableFailureMechanism(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
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
                return ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(assessmentSection,
                                                                                                                   failureMechanismCode,
                                                                                                                   failureMechanismAssemblyMethod);
            }

            return new ExportableFailureMechanism(
                new ExportableFailureMechanismAssemblyResult(
                    GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection),
                    failureMechanism.AssemblyResult.ProbabilityResultType == FailurePathAssemblyProbabilityResultType.Manual),
                CreateFailureMechanismSectionResults(failureMechanism.SectionResults),
                failureMechanismCode);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>
        /// with assembly results based on <paramref name="failureMechanismSectionResults"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/>
        /// to create a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResult> CreateFailureMechanismSectionResults(
            IEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> failureMechanismSectionResults)
        {
            IDictionary<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanismSectionResults);

            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResult>();
            foreach (KeyValuePair<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSectionsLookup)
            {
                const FailureMechanismSectionAssemblyCategoryGroup simpleAssembly = FailureMechanismSectionAssemblyCategoryGroup.None;
                const FailureMechanismSectionAssemblyCategoryGroup detailedAssembly = FailureMechanismSectionAssemblyCategoryGroup.None;
                const FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly = FailureMechanismSectionAssemblyCategoryGroup.None;
                const FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = FailureMechanismSectionAssemblyCategoryGroup.None;

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                        failureMechanismSectionPair.Value,
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(simpleAssembly, ExportableAssemblyMethod.WBI0E1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(detailedAssembly, ExportableAssemblyMethod.WBI0G6),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T4),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(combinedAssembly, ExportableAssemblyMethod.WBI0A1)));
            }

            return exportableResults;
        }
    }
}