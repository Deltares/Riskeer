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
using Ringtoets.Common.Primitives;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// for a grass cover erosion inwards assessment.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        private double assessmentLayerThreeValue;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            SimpleAssessmentInput = SimpleAssessmentResultValidityOnlyType.None;
            assessmentLayerThreeValue = double.NaN;
        }

        /// <summary>
        /// Gets or sets the <see cref="GrassCoverErosionInwardsCalculation"/>, which is chosen 
        /// to be representative for the whole section.
        /// </summary>
        public GrassCoverErosionInwardsCalculation Calculation { get; set; }

        /// <summary>
        /// Gets or sets the state of the simple assessment per failure mechanism section.
        /// </summary>
        public SimpleAssessmentResultValidityOnlyType SimpleAssessmentInput { get; set; }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="value"/> is outside of the valid ranges.</exception>
        public double AssessmentLayerThree
        {
            get
            {
                return assessmentLayerThreeValue;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null, true);
                assessmentLayerThreeValue = value;
            }
        }
    }
}