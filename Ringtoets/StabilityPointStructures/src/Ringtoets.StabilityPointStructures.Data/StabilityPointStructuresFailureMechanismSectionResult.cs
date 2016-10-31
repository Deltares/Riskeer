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
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// This class holds information about the result of a calculation on section level for the
    /// Strength and Stability of Point Constructions failure mechanism.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismSectionResult : StructuresFailureMechanismSectionResult<StabilityPointStructuresInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> for which the
        /// <see cref="StabilityPointStructuresFailureMechanismSectionResult"/> will hold the result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section) {}

        /// <summary>
        /// Gets or sets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        public double AssessmentLayerTwoA
        {
            get
            {
                if (Calculation == null || !Calculation.HasOutput)
                {
                    return double.NaN;
                }
                return Calculation.Output.Probability;
            }
        }
    }
}