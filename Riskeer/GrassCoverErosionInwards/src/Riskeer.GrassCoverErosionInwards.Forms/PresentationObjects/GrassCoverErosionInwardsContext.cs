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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required grass cover erosion inwards input knowledge to configure and create
    /// related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class GrassCoverErosionInwardsContext<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected GrassCoverErosionInwardsContext(
            T wrappedData,
            GrassCoverErosionInwardsFailureMechanism failureMechanism,
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
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the available dike profiles which can be used to assign to a <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        public IEnumerable<DikeProfile> AvailableDikeProfiles
        {
            get
            {
                return FailureMechanism.DikeProfiles;
            }
        }
    }
}