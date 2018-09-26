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
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="FailureMechanismSection"/> with a section specific N.
    /// </summary>
    internal class FailureMechanismSectionProbabilityAssessmentRow : FailureMechanismSectionRow
    {
        private readonly FailureMechanismSection section;
        private readonly ProbabilityAssessmentInput probabilityAssessmentInput;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionProbabilityAssessmentRow"/>.
        /// </summary>
        /// <param name="section">The failure mechanism section to use.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <param name="probabilityAssessmentInput">The probability assessment input to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public FailureMechanismSectionProbabilityAssessmentRow(FailureMechanismSection section,
                                                               double sectionStart, double sectionEnd,
                                                               ProbabilityAssessmentInput probabilityAssessmentInput)
            : base(section, sectionStart, sectionEnd)
        {
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentInput));
            }

            this.section = section;
            this.probabilityAssessmentInput = probabilityAssessmentInput;
        }

        /// <summary>
        /// Gets the N of the section.
        /// [-]
        /// </summary>
        public RoundedDouble N
        {
            get
            {
                return new RoundedDouble(2, probabilityAssessmentInput.GetN(section.Length));
            }
        }
    }
}