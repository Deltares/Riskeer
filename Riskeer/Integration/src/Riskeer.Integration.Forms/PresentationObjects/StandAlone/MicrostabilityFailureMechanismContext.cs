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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Data.StandAlone;

namespace Riskeer.Integration.Forms.PresentationObjects.StandAlone
{
    /// <summary>
    /// Presentation object for <see cref="MicrostabilityFailureMechanism"/> in the registration state.
    /// </summary>
    public class MicrostabilityFailureMechanismContext : FailureMechanismContext<MicrostabilityFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MicrostabilityFailureMechanismContext"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The parent of <paramref name="failureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public MicrostabilityFailureMechanismContext(MicrostabilityFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(failureMechanism, assessmentSection) {}
    }
}