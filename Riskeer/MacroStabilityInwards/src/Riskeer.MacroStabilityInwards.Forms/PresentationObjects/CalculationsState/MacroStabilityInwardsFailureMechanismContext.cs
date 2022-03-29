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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.PresentationObjects.CalculationsState
{
    /// <summary>
    /// Presentation object for <see cref="MacroStabilityInwardsFailureMechanism"/> in the calculations state.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismContext : FailureMechanismContext<MacroStabilityInwardsFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of  <see cref="MacroStabilityInwardsFailureMechanismContext"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The parent of <paramref name="failureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismContext(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(failureMechanism, assessmentSection) {}
    }
}