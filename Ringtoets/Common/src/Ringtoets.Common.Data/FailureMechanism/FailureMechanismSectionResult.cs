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
using Core.Common.Base;
using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// Base class for classes that hold information of the result of the <see cref="FailureMechanismSection"/>.
    /// </summary>
    public abstract class FailureMechanismSectionResult : Observable
    {
        private RoundedDouble assessmentLayerThreeValue;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        protected FailureMechanismSectionResult(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            Section = section;
            AssessmentLayerOne = AssessmentLayerOneState.NotAssessed;
            AssessmentLayerThree = RoundedDouble.NaN;
        }

        /// <summary>
        /// Gets or sets the state of the assessment layer one.
        /// </summary>
        public AssessmentLayerOneState AssessmentLayerOne { get; set; }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="value"/> is outside of the valid ranges.</exception>
        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return assessmentLayerThreeValue;
            }
            set
            {
                assessmentLayerThreeValue = ValidateAssessmentLayerThree(value);
            }
        }

        /// <summary>
        /// Gets the encapsulated <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; private set; }

        /// <summary>
        /// Validates the value of the tailored assessment of safety
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>The validated value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="value"/> is outside of the valid ranges.</exception>
        protected virtual RoundedDouble ValidateAssessmentLayerThree(RoundedDouble value)
        {
            return value;
        }
    }
}