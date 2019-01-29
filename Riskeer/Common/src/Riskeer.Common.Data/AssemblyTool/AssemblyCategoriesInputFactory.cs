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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for creating instances of <see cref="AssemblyCategoriesInput"/>.
    /// </summary>
    public static class AssemblyCategoriesInputFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="AssemblyCategoriesInput"/>.
        /// </summary>
        /// <param name="n">The 'N' parameter of the failure mechanism
        /// used to factor in the 'length effect'.</param>
        /// <param name="failureMechanism">The failure mechanism to create the input for.</param>
        /// <param name="assessmentSection">The assessment section to create the input for.</param>
        /// <returns>A <see cref="AssemblyCategoriesInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static AssemblyCategoriesInput CreateAssemblyCategoriesInput(double n,
                                                                            IFailureMechanism failureMechanism,
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

            return new AssemblyCategoriesInput(n,
                                               failureMechanism.Contribution / 100,
                                               assessmentSection.FailureMechanismContribution.SignalingNorm,
                                               assessmentSection.FailureMechanismContribution.LowerLimitNorm);
        }
    }
}