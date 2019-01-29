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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for <see cref="IFailureMechanism.Sections"/>.
    /// </summary>
    public class FailureMechanismSectionsContext : ObservableWrappedObjectContextBase<IFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSectionsContext"/> class.
        /// </summary>
        /// <param name="wrappedData">The failure mechanism to wrap.</param>
        /// <param name="assessmentSection">The owning assessment section of <paramref name="wrappedData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public FailureMechanismSectionsContext(IFailureMechanism wrappedData, IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}