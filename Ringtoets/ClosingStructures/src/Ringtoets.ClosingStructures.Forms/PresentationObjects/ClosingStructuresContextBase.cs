// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.ClosingStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required closing structures input knowledge to configure and create
    /// related objects. It will delegate observable behaviour to the wrapped data object.
    /// </summary>
    public abstract class ClosingStructuresContextBase<T> : ObservableWrappedObjectContextBase<T> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ClosingStructuresContextBase{T}"/>.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by the context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the wrapped data belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the wrapped data belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters are <c>null</c>.</exception>
        protected ClosingStructuresContextBase(T wrappedData,
                                               ClosingStructuresFailureMechanism failureMechanism,
                                               IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the assessment section for this instance.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }

        /// <summary>
        /// Gets the failure mechanism for this instance.
        /// </summary>
        public ClosingStructuresFailureMechanism FailureMechanism { get; private set; }
    }
}