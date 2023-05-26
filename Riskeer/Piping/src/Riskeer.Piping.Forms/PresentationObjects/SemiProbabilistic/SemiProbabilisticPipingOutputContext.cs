﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic
{
    /// <summary>
    /// A presentation layer object which wraps a <see cref="SemiProbabilisticPipingOutput"/>.
    /// </summary>
    public class SemiProbabilisticPipingOutputContext : WrappedObjectContextBase<SemiProbabilisticPipingOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingOutputContext"/>.
        /// </summary>
        /// <param name="pipingOutput">The <see cref="SemiProbabilisticPipingOutput"/> object to wrap.</param>
        /// <param name="failureMechanism">The failure mechanism that the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SemiProbabilisticPipingOutputContext(SemiProbabilisticPipingOutput pipingOutput, PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(pipingOutput)
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
        /// Gets the failure mechanism.
        /// </summary>
        public PipingFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}