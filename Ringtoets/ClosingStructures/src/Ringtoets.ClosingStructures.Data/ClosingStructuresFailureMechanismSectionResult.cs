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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// for a closing structures assessment.
    /// </summary>
    public class ClosingStructuresFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        private double assessmentLayerThree;

        /// <summary>
        /// Initializes a new instance of <see cref="StructuresFailureMechanismSectionResult{T}"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public ClosingStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            SimpleAssessmentInput = SimpleAssessmentResultType.None;
            assessmentLayerThree = double.NaN;
        }

        /// <summary>
        /// Gets or sets the <see cref="StructuresCalculation{T}"/>, which is chosen 
        /// to be representative for the whole section.
        /// </summary>
        public StructuresCalculation<ClosingStructuresInput> Calculation { get; set; }

        /// <summary>
        /// Gets or sets the state of the simple assessment per failure mechanism section.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentInput { get; set; }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="value"/> is outside of the valid ranges.</exception>
        public double AssessmentLayerThree
        {
            get
            {
                return assessmentLayerThree;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null, true);
                assessmentLayerThree = value;
            }
        }
    }
}