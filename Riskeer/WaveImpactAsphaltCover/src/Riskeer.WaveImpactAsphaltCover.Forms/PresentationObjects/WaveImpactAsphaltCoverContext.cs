﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required wave impact asphalt cover input knowledge to configure and create
    /// related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class WaveImpactAsphaltCoverContext<T> : ObservableWrappedObjectContextBase<T> where T : class, IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveImpactAsphaltCoverContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected WaveImpactAsphaltCoverContext(
            T wrappedData,
            WaveImpactAsphaltCoverFailureMechanism failureMechanism,
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
        public WaveImpactAsphaltCoverFailureMechanism FailureMechanism { get; }

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