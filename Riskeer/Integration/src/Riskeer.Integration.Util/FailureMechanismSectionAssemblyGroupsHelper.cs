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
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;

namespace Riskeer.Integration.Util
{
    /// <summary>
    /// Class containing helper methods for <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>.
    /// </summary>
    public static class FailureMechanismSectionAssemblyGroupsHelper
    {
        /// <summary>
        /// Gets the failure mechanism section assembly group boundaries based on the assessment section.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="assessmentSection"/> is <c>null</c>.</exception>
        /// <returns>A collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>.</returns>
        /// <remarks>An empty collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>
        /// is returned when the boundaries cannot be assembled.</remarks>
        public static IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> GetFailureMechanismSectionAssemblyGroupBoundaries(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            try
            {
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                return FailureMechanismSectionAssemblyGroupBoundariesFactory
                       .CreateFailureMechanismSectionAssemblyGroupBoundaries(failureMechanismContribution.SignalingNorm,
                                                                             failureMechanismContribution.LowerLimitNorm)
                       .Concat(new[]
                       {
                           new FailureMechanismSectionAssemblyGroupBoundaries(double.NaN, double.NaN, FailureMechanismSectionAssemblyGroup.Dominant),
                           new FailureMechanismSectionAssemblyGroupBoundaries(double.NaN, double.NaN, FailureMechanismSectionAssemblyGroup.NotDominant),
                           new FailureMechanismSectionAssemblyGroupBoundaries(double.NaN, double.NaN, FailureMechanismSectionAssemblyGroup.NotRelevant)
                       })
                       .ToArray();
            }
            catch (AssemblyException)
            {
                return Enumerable.Empty<FailureMechanismSectionAssemblyGroupBoundaries>();
            }
        }
    }
}