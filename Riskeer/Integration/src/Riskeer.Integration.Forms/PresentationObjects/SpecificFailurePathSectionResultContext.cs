﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for a collection of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/>
    /// for the <see cref="SpecificFailurePath"/>.
    /// </summary>
    public class SpecificFailurePathSectionResultContext : FailureMechanismSectionResultContext<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SpecificFailurePathSectionResultContext"/>.
        /// </summary>
        /// <param name="wrappedSectionResults">The <see cref="IObservableEnumerable{T}"/>
        /// of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/> to wrap.</param>
        /// <param name="failurePath">The <see cref="SpecificFailurePath"/>
        /// the <paramref name="wrappedSectionResults"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section results belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SpecificFailurePathSectionResultContext(IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> wrappedSectionResults,
                                                       SpecificFailurePath failurePath, IAssessmentSection assessmentSection)
            : base(wrappedSectionResults, failurePath, assessmentSection) {}
    }
}