// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.StabilityStoneCover.Data
{
    /// <summary>
    /// This class holds information about the result of a calculation on section level for the
    /// Stability of Stone Cover failure mechanism.
    /// </summary>
    public class StabilityStoneCoverFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> for which the
        /// <see cref="StabilityStoneCoverFailureMechanismSectionResult"/> will hold the result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public StabilityStoneCoverFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            AssessmentLayerTwoA = AssessmentLayerTwoAResult.NotCalculated;
            AssessmentLayerThree = RoundedDouble.NaN;
        }

        /// <summary>
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section, for which
        /// the result is a value of <see cref="AssessmentLayerTwoAResult"/>.
        /// </summary>
        public AssessmentLayerTwoAResult AssessmentLayerTwoA { get; set; }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }
    }
}