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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Data;

namespace Riskeer.DuneErosion.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for an instance of <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneErosionFailureMechanismContext : FailureMechanismContext<DuneErosionFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionFailureMechanismContext"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> instance wrapped by this context object.</param>
        /// <param name="assessmentSection">The assessment section which the failure mechanism belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public DuneErosionFailureMechanismContext(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
            : base(failureMechanism, assessmentSection) {}
    }
}