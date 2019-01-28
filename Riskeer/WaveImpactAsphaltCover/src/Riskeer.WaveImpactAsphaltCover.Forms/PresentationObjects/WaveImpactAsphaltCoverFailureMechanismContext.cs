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
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for an instance of <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverFailureMechanismContext : FailureMechanismContext<WaveImpactAsphaltCoverFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveImpactAsphaltCoverFailureMechanismContext"/> class.
        /// </summary>
        /// <param name="wrappedFailureMechanism">The failure mechanism.</param>
        /// <param name="parent">The parent of <paramref name="wrappedFailureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverFailureMechanismContext(WaveImpactAsphaltCoverFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
            base(wrappedFailureMechanism, parent) {}
    }
}