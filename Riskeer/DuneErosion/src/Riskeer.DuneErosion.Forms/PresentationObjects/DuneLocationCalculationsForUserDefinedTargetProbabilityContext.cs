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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all dune location calculations based on a user defined target probability.
    /// </summary>
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityContext : ObservableWrappedObjectContextBase<DuneLocationCalculationsForTargetProbability>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsForUserDefinedTargetProbabilityContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculations wrapped by the <see cref="DuneLocationCalculationsForUserDefinedTargetProbabilityContext"/>.</param>
        /// <param name="failureMechanism">The failure mechanism that the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationCalculationsForUserDefinedTargetProbabilityContext(DuneLocationCalculationsForTargetProbability wrappedData,
                                                                              DuneErosionFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the failure mechanism that the context belongs to.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}