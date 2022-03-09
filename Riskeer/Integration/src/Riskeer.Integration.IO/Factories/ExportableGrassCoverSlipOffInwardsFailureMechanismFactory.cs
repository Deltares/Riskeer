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

using System;
using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism"/>
    /// with assembly results for grass cover slip off inwards.
    /// </summary>
    public static class ExportableGrassCoverSlipOffInwardsFailureMechanismFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanism"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverSlipOffInwardsFailureMechanism"/> to create an
        /// <see cref="ExportableFailureMechanism"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <returns>An <see cref="ExportableFailureMechanism"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism CreateExportableFailureMechanism(
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism,
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

            return new ExportableFailureMechanism(
                new ExportableFailureMechanismAssemblyResult(
                    FailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection),
                    failureMechanism.AssemblyResult.ProbabilityResultType == FailurePathAssemblyProbabilityResultType.Manual),
                CreateFailureMechanismSectionResults(failureMechanism, assessmentSection),
                ExportableFailureMechanismType.GABI);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/>
        /// with assembly results based on <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverSlipOffInwardsFailureMechanism"/> to create a collection of
        /// <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/> for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to use in the assembly.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> CreateFailureMechanismSectionResults(
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            IDictionary<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanism.SectionResults);

            var exportableResults = new List<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>();
            foreach (KeyValuePair<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSectionsLookup)
            {
                FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismAssemblyFactory.AssembleSection(
                    failureMechanismSectionPair.Key, failureMechanism, assessmentSection);

                exportableResults.Add(
                    new ExportableFailureMechanismSectionAssemblyWithProbabilityResult(
                        failureMechanismSectionPair.Value, assemblyResult.FailureMechanismSectionAssemblyGroup, assemblyResult.SectionProbability));
            }

            return exportableResults;
        }
    }
}