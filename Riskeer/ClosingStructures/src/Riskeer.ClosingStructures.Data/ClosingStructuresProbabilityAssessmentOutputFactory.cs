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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;

namespace Riskeer.ClosingStructures.Data
{
    /// <summary>
    /// Factory for creating <see cref="ProbabilityAssessmentOutput"/> for
    /// closing structures.
    /// </summary>
    public static class ClosingStructuresProbabilityAssessmentOutputFactory
    {
        /// <summary>
        /// Creates <see cref="ProbabilityAssessmentOutput"/> based on the provided parameters.
        /// </summary>
        /// <param name="output">The output to get the reliability from.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <returns>The calculated <see cref="ProbabilityAssessmentOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static ProbabilityAssessmentOutput Create(StructuresOutput output,
                                                         ClosingStructuresFailureMechanism failureMechanism,
                                                         IAssessmentSection assessmentSection)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return ProbabilityAssessmentOutputFactory.Create(assessmentSection.FailureMechanismContribution.Norm,
                                                             failureMechanism.Contribution,
                                                             failureMechanism.GeneralInput.N,
                                                             output.Reliability);
        }
    }
}