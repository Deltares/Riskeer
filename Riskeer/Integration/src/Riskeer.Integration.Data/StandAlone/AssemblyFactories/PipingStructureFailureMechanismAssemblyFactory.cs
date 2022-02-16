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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Integration.Data.StandAlone.AssemblyFactories
{
    /// <summary>
    /// Factory for assembling assembly results for a piping structure failure mechanism.
    /// </summary>
    public static class PipingStructureFailureMechanismAssemblyFactory
    {
        /// <summary>
        /// Assembles the failure mechanism based on its input arguments.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assemble.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/>
        /// belongs to.</param>
        /// <returns>A <see cref="double"/> representing the assembly result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the failure mechanism cannot be assembled.</exception>
        public static double AssembleFailureMechanism(PipingStructureFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Func<double> performAssemblyFunc = () =>
            {
                IEnumerable<FailureMechanismSectionAssemblyResult> sectionAssemblyResults =
                    failureMechanism.SectionResults.Select(sr => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, assessmentSection))
                                    .ToArray();
                return FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(failureMechanism.GeneralInput.N, sectionAssemblyResults);
            };

            return FailurePathAssemblyHelper.AssembleFailurePath(failureMechanism, performAssemblyFunc);
        }
    }
}