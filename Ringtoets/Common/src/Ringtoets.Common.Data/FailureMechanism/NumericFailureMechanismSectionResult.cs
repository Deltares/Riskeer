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

using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// Class which represents results of different layers (1, 2a, 2b, 3) of results of the 
    /// <see cref="FailureMechanismSection"/>. The result for a layer 2a assessment is an arbitrary 
    /// numeric value.
    /// </summary>
    public class NumericFailureMechanismSectionResult: FailureMechanismSectionResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="NumericFailureMechanismSectionResult"/>
        /// </summary>
        /// <param name="section">The section for which to add the result.</param>
        public NumericFailureMechanismSectionResult(FailureMechanismSection section) : base(section) { }

        /// <summary>
        /// Gets the value of assessment layer two a.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoA { get; set; }

        /// <summary>
        /// Gets or sets the value of assessment layer two b.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoB { get; set; }

        /// <summary>
        /// Gets or sets the value of assessment layer three.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }
    }
}