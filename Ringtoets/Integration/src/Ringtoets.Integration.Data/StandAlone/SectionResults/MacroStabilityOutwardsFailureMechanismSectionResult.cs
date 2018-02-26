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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Data.StandAlone.SectionResults
{
    /// <summary>
    /// This class holds information about the result of a calculation on section level for the
    /// macro stability outwards failure mechanism.
    /// </summary>
    public class MacroStabilityOutwardsFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        private static readonly Range<double> validityRangeAssessmentProbability = new Range<double>(0, 1);
        private double detailedAssessmentProbability;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> for which the
        /// <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/> will hold the result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            SimpleAssessmentResult = SimpleAssessmentResultType.None;
            DetailedAssessmentResult = DetailedAssessmentResultType.Probability;
            DetailedAssessmentProbability = double.NaN;
            TailorMadeAssessmentResult = TailorMadeAssessmentResultType.None;
            TailorMadeAssessmentProbability = RoundedDouble.NaN;
        }

        /// <summary>
        /// Gets or sets the simple assessment result.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the detailed assessment result.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the tailor made assessment result.
        /// </summary>
        public TailorMadeAssessmentResultType TailorMadeAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        public double DetailedAssessmentProbability
        {
            get
            {
                return detailedAssessmentProbability;
            }
            set
            {
                ProbabilityHelper.ValidateProbability(value, null,
                                                      RingtoetsCommonDataResources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentProbability_Value_needs_to_be_in_Range_0_,
                                                      true);
                detailedAssessmentProbability = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the tailor made assessment of safety per failure mechanism section as a probability.
        /// </summary>
        public RoundedDouble TailorMadeAssessmentProbability { get; set; }
    }
}