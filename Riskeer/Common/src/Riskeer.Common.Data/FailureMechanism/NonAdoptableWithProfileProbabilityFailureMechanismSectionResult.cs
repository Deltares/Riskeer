﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Probability;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Class that holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// with an <see cref="NonAdoptableInitialFailureMechanismResultType"/> and profile probabilities.
    /// </summary>
    public class NonAdoptableWithProfileProbabilityFailureMechanismSectionResult : NonAdoptableFailureMechanismSectionResult
    {
        private double manualInitialFailureMechanismResultProfileProbability;
        private double refinedProfileProbability;

        public NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            ManualInitialFailureMechanismResultProfileProbability = double.NaN;
            RefinedProfileProbability = double.NaN;
        }

        /// <summary>
        /// Gets or sets the value of the manual initial failure mechanism result per profile as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        public double ManualInitialFailureMechanismResultProfileProbability
        {
            get => manualInitialFailureMechanismResultProfileProbability;
            set
            {
                ProbabilityHelper.ValidateProbability(
                    value, null,
                    RiskeerCommonDataResources.FailureProbability_Value_needs_to_be_in_Range_0_,
                    true);
                manualInitialFailureMechanismResultProfileProbability = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the refined probability per profile.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        public double RefinedProfileProbability
        {
            get => refinedProfileProbability;
            set
            {
                ProbabilityHelper.ValidateProbability(
                    value, null,
                    RiskeerCommonDataResources.FailureProbability_Value_needs_to_be_in_Range_0_,
                    true);
                refinedProfileProbability = value;
            }
        }
    }
}