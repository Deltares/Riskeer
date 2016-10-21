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

using System;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// This class holds information about the result of a calculation on section level for the
    /// Closing Structures failure mechanism.
    /// </summary>
    public class ClosingStructuresFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        private double assessmentLayerTwoA;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> for which the
        /// <see cref="ClosingStructuresFailureMechanismSectionResult"/> will hold the result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public ClosingStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            AssessmentLayerTwoA = double.NaN;
        }

        /// <summary>
        /// Gets or sets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        public double AssessmentLayerTwoA
        {
            get
            {
                return assessmentLayerTwoA;
            }
            set
            {
                if (!double.IsNaN(value) && (value < 0 || value > 1))
                {
                    throw new ArgumentException(Resources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentLayerTwoA_Value_needs_to_be_between_0_and_1);
                }
                assessmentLayerTwoA = value;
            }
        }
    }
}