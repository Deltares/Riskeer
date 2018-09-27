// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>.
    /// </summary>
    public static class ExportableFailureMechanismFactory
    {
        /// <summary>
        /// Creates a default instance of an <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with a probability based on its input parameters.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <param name="failureMechanismCode">The <see cref="ExportableFailureMechanismType"/> of the failure mechanism.</param>
        /// <param name="failureMechanismGroup">The <see cref="ExportableFailureMechanismGroup"/> of the failure mechanism.</param>
        /// <param name="assemblyMethod">The assembly method which is used to obtain the general assembly result of the failure mechanism.</param>
        /// <returns>An <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with default values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateDefaultExportableFailureMechanismWithProbability(
            IAssessmentSection assessmentSection,
            ExportableFailureMechanismType failureMechanismCode,
            ExportableFailureMechanismGroup failureMechanismGroup,
            ExportableAssemblyMethod assemblyMethod)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                new ExportableFailureMechanismAssemblyResultWithProbability(assemblyMethod,
                                                                            FailureMechanismAssemblyCategoryGroup.NotApplicable,
                                                                            0),
                new[]
                {
                    new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult(CreateExportableFailureMechanismSection(assessmentSection.ReferenceLine),
                                                                                                         new ExportableSectionAssemblyResultWithProbability(ExportableAssemblyMethod.WBI0A1,
                                                                                                                                                            FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                                                                                                                                                            0))
                },
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a default instance of an <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// without a probability based on its input parameters.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <param name="failureMechanismCode">The <see cref="ExportableFailureMechanismType"/> of the failure mechanism.</param>
        /// <param name="failureMechanismGroup">The <see cref="ExportableFailureMechanismGroup"/> of the failure mechanism.</param>
        /// <param name="assemblyMethod">The assembly method which is used to obtain the general assembly result of the failure mechanism.</param>
        /// <returns>An <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with default values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateDefaultExportableFailureMechanismWithoutProbability(
            IAssessmentSection assessmentSection,
            ExportableFailureMechanismType failureMechanismCode,
            ExportableFailureMechanismGroup failureMechanismGroup,
            ExportableAssemblyMethod assemblyMethod)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                new ExportableFailureMechanismAssemblyResult(assemblyMethod,
                                                             FailureMechanismAssemblyCategoryGroup.NotApplicable),
                new[]
                {
                    new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(CreateExportableFailureMechanismSection(assessmentSection.ReferenceLine),
                                                                                              new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI0A1,
                                                                                                                                  FailureMechanismSectionAssemblyCategoryGroup.NotApplicable))
                },
                failureMechanismCode,
                failureMechanismGroup);
        }

        private static ExportableFailureMechanismSection CreateExportableFailureMechanismSection(ReferenceLine referenceLine)
        {
            return new ExportableFailureMechanismSection(referenceLine.Points, 0, referenceLine.Length);
        }
    }
}