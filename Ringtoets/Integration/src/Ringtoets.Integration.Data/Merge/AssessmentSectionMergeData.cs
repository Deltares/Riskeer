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
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Data.Merge
{
    /// <summary>
    /// Class that holds the merge data.
    /// </summary>
    public class AssessmentSectionMergeData
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeData"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to merge.</param>
        /// <param name="failureMechanisms">The failure mechanisms to merge.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionMergeData(AssessmentSection assessmentSection, IEnumerable<IFailureMechanism> failureMechanisms)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            AssessmentSection = assessmentSection;
            FailureMechanisms = failureMechanisms;
        }

        /// <summary>
        /// Gets the assessment section to merge.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the failure mechanisms to merge.
        /// </summary>
        public IEnumerable<IFailureMechanism> FailureMechanisms { get; }
    }
}