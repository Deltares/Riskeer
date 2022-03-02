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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for a collection of <see cref="IFailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FailureMechanismSectionResult"/>.</typeparam>
    public class FailureMechanismSectionResultContext<T> : WrappedObjectContextBase<IObservableEnumerable<T>>
        where T : FailureMechanismSectionResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResultContext{T}"/>.
        /// </summary>
        /// <param name="wrappedSectionResults">The <see cref="IObservableEnumerable{T}"/> of <typeparamref name="T"/> to wrap.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> the <paramref name="wrappedSectionResults"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section results belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public FailureMechanismSectionResultContext(IObservableEnumerable<T> wrappedSectionResults, IFailurePath failureMechanism,
                                                    IAssessmentSection assessmentSection)
            : base(wrappedSectionResults)
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
        /// Gets the <see cref="IFailureMechanism"/>.
        /// </summary>
        public IFailurePath FailureMechanism { get; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}