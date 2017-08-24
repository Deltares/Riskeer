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
using Core.Common.Base.Data;
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
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="HeightStructuresFailureMechanismSectionResult"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public HeightStructuresFailureMechanismSectionResultRow(HeightStructuresFailureMechanismSectionResult sectionResult) : base(sectionResult) {}

        /// <summary>
        /// Gets or sets the value representing the result of the layer 3 assessment.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                int nrOfDecimals = SectionResult.AssessmentLayerThree.NumberOfDecimalPlaces;
                SectionResult.AssessmentLayerThree = new RoundedDouble(nrOfDecimals, value);
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
                return SectionResult.AssessmentLayerTwoA;
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