// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.PresentationObjects.Probabilistic
{
    /// <summary>
    /// Presentation object for the profile specific output of <see cref="ProbabilisticPipingCalculationScenario"/>.
    /// </summary>
    public class ProbabilisticPipingProfileSpecificOutputContext : ObservableWrappedObjectContextBase<ProbabilisticPipingCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingProfileSpecificOutputContext"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculationScenario"/> object to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticPipingProfileSpecificOutputContext(ProbabilisticPipingCalculationScenario calculation,
                                                               PipingFailureMechanism failureMechanism,
                                                               IAssessmentSection assessmentSection)
            : base(calculation)
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
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public PipingFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}