// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Data;

namespace Riskeer.StabilityStoneCover.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for a collection of <see cref="NonAdoptableFailureMechanismSectionResult"/>
    /// for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverFailureMechanismSectionResultContext : FailureMechanismSectionResultContext<NonAdoptableFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverFailureMechanismSectionResultContext"/>.
        /// </summary>
        /// <param name="wrappedSectionResults">The <see cref="IObservableEnumerable{T}"/>
        /// of <see cref="NonAdoptableFailureMechanismSectionResult"/> to wrap.</param>
        /// <param name="failureMechanism">The <see cref="StabilityStoneCoverFailureMechanism"/>
        /// the <paramref name="wrappedSectionResults"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section results belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityStoneCoverFailureMechanismSectionResultContext(IObservableEnumerable<NonAdoptableFailureMechanismSectionResult> wrappedSectionResults,
                                                                       IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(wrappedSectionResults, failureMechanism, assessmentSection) {}
    }
}