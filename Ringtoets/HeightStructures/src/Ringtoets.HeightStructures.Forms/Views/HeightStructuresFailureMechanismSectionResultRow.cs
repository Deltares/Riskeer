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
using System.ComponentModel;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HeightStructuresFailureMechanismSectionResult"/>.
    /// </summary>
    internal class HeightStructuresFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<HeightStructuresFailureMechanismSectionResult>
    {
        private readonly HeightStructuresFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="HeightStructuresFailureMechanismSectionResult"/> this row contains.</param>
        /// <param name="failureMechanism">The failure mechanism the result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the result belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresFailureMechanismSectionResultRow(HeightStructuresFailureMechanismSectionResult sectionResult,
                                                                HeightStructuresFailureMechanism failureMechanism,
                                                                IAssessmentSection assessmentSection)
            : base(sectionResult)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="value"/> is outside of the valid ranges.</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                SectionResult.AssessmentLayerThree = value;
            }
        }

        /// <summary>
        /// Gets the assessment layer two a of the <see cref="HeightStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerTwoA
        {
            get
            {
                return SectionResult.GetAssessmentLayerTwoA(failureMechanism, assessmentSection);
            }
        }

        /// <summary>
        /// Gets the <see cref="StructuresCalculation{T}"/> of the wrapped
        /// <see cref="HeightStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <returns><c>null</c> if the wrapped section result does not have a calculation
        /// set. Otherwise the calculation of the wrapped section result is returned.</returns>
        public StructuresCalculation<HeightStructuresInput> GetSectionResultCalculation()
        {
            return SectionResult.Calculation;
        }
    }
}