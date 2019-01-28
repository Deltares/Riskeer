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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required stability stone cover input knowledge
    /// to configure and create related calculation objects.
    /// </summary>
    /// <typeparam name="T">The observable object type of the wrapped instance.</typeparam>
    public abstract class StabilityStoneCoverContext<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityStoneCoverContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected StabilityStoneCoverContext(T wrappedData,
                                             StabilityStoneCoverFailureMechanism failureMechanism,
                                             IAssessmentSection assessmentSection) : base(wrappedData)
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
        public StabilityStoneCoverFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the foreshore profiles currently known in the <see cref="FailureMechanism"/>.
        /// </summary>
        public IEnumerable<ForeshoreProfile> ForeshoreProfiles
        {
            get
            {
                return FailureMechanism.ForeshoreProfiles;
            }
        }
    }
}