﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// This class holds information about the result of a calculation on section level for the
    /// Closing Structures failure mechanism.
    /// </summary>
    public class ClosingStructuresFailureMechanismSectionResult : StructuresFailureMechanismSectionResult<ClosingStructuresInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> for which the
        /// <see cref="ClosingStructuresFailureMechanismSectionResult"/> will hold the result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public ClosingStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section) {}

        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        public double AssessmentLayerTwoA
        {
            get
            {
                if (Calculation == null || !Calculation.HasOutput)
                {
                    return double.NaN;
                }
                return Calculation.Output.ProbabilityAssessmentOutput.Probability;
            }
        }
    }
}