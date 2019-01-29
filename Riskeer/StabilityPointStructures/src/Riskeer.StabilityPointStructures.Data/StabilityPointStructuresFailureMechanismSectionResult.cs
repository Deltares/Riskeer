// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Data
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// for a stability point structures assessment.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        private double tailorMadeAssessmentProbability;
        private double manualAssemblyProbability;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="StabilityPointStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        public StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.None;
            DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.Probability;
            TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.None;
            TailorMadeAssessmentProbability = double.NaN;
            ManualAssemblyProbability = double.NaN;
        }

        /// <summary>
        /// Gets or sets the <see cref="StructuresCalculation{T}"/>, which is chosen 
        /// to be representative for the whole section.
        /// </summary>
        public StructuresCalculation<StabilityPointStructuresInput> Calculation { get; set; }

        /// <summary>
        /// Gets or sets the simple assessment result.
        /// </summary>
        public SimpleAssessmentValidityOnlyResultType SimpleAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the detailed assessment result.
        /// </summary>
        public DetailedAssessmentProbabilityOnlyResultType DetailedAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the tailor made assessment result.
        /// </summary>
        public TailorMadeAssessmentProbabilityCalculationResultType TailorMadeAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the value of the tailor made assessment of safety
        /// per failure mechanism section as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <paramref name="value"/> is not in range [0,1].</exception>
        public double TailorMadeAssessmentProbability
        {
            get
            {
                return tailorMadeAssessmentProbability;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null,
                                                      RingtoetsCommonDataResources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentProbability_Value_needs_to_be_in_Range_0_,
                                                      true);
                tailorMadeAssessmentProbability = value;
            }
        }

        /// <summary>
        /// Gets or sets the manually set assembly probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <paramref name="value"/> is not in range [0,1].</exception>
        public double ManualAssemblyProbability
        {
            get
            {
                return manualAssemblyProbability;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null,
                                                      RingtoetsCommonDataResources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentProbability_Value_needs_to_be_in_Range_0_,
                                                      true);
                manualAssemblyProbability = value;
            }
        }
    }
}