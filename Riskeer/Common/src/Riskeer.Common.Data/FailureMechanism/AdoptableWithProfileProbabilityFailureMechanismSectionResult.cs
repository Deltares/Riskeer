// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Class that holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// with an <see cref="AdoptableInitialFailureMechanismResultType"/> and profile probabilities.
    /// </summary>
    public class AdoptableWithProfileProbabilityFailureMechanismSectionResult : AdoptableFailureMechanismSectionResult
    {
        private double manualInitialFailureMechanismResultProfileProbability;
        private double refinedProfileProbability;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.
        /// </summary>
        public AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSection section)
            : base(section)
        {
            ManualInitialFailureMechanismResultProfileProbability = double.NaN;
            ProbabilityRefinementType = ProbabilityRefinementType.Section;
            RefinedProfileProbability = double.NaN;
        }

        /// <summary>
        /// Gets or sets the value of the manual initial failure mechanism result per profile as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="value"/> is not in range [0,1].</item>
        /// <item><paramref name="value"/> is greater than the manual initial section probability.</item>
        /// </list></exception>
        public double ManualInitialFailureMechanismResultProfileProbability
        {
            get => manualInitialFailureMechanismResultProfileProbability;
            set
            {
                ValidateInitialProfileProbability(value);
                manualInitialFailureMechanismResultProfileProbability = value;
            }
        }

        /// <summary>
        /// Gets or sets the probability refinement type.
        /// </summary>
        public ProbabilityRefinementType ProbabilityRefinementType { get; set; }

        /// <summary>
        /// Gets or sets the value of the refined probability per profile.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="value"/> is not in range [0,1].</item>
        /// <item><paramref name="value"/> is greater than the refined section probability.</item>
        /// </list></exception>
        public double RefinedProfileProbability
        {
            get => refinedProfileProbability;
            set
            {
                ValidateRefinedProfileProbability(value);
                refinedProfileProbability = value;
            }
        }

        private void ValidateInitialProfileProbability(double value)
        {
            ValidateFailureProbability(value);
            ValidateProbabilities(value, ManualInitialFailureMechanismResultSectionProbability,
                                  RiskeerCommonDataResources.WithProfileProbabilityFailureMechanismSectionResult_ManualInitialFailureMechanismResultSectionProbability_must_be_greater_than_ManualInitialFailureMechanismResultProfileProbability);
        }

        protected override void ValidateInitialSectionProbability(double value)
        {
            base.ValidateInitialSectionProbability(value);
            ValidateProbabilities(ManualInitialFailureMechanismResultProfileProbability, value,
                                  RiskeerCommonDataResources.WithProfileProbabilityFailureMechanismSectionResult_ManualInitialFailureMechanismResultSectionProbability_must_be_greater_than_ManualInitialFailureMechanismResultProfileProbability);
        }

        private void ValidateRefinedProfileProbability(double value)
        {
            ValidateFailureProbability(value);
            ValidateProbabilities(value, RefinedSectionProbability,
                                  RiskeerCommonDataResources.WithProfileProbabilityFailureMechanismSectionResult_RefinedSectionProbability_must_be_greater_than_RefinedProfileProbability);
        }

        protected override void ValidateRefinedSectionProbability(double value)
        {
            base.ValidateRefinedSectionProbability(value);
            ValidateProbabilities(RefinedProfileProbability, value,
                                  RiskeerCommonDataResources.WithProfileProbabilityFailureMechanismSectionResult_RefinedSectionProbability_must_be_greater_than_RefinedProfileProbability);
        }

        private static void ValidateProbabilities(double profileProbability, double sectionProbability, string errorMessage)
        {
            if (profileProbability > sectionProbability)
            {
                throw new ArgumentOutOfRangeException(null, errorMessage);
            }
        }
    }
}