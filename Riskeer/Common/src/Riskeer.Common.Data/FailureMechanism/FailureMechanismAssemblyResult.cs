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
using Core.Common.Base;
using Riskeer.Common.Data.Probability;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Class containing the assembly result of an entire failure mechanism.
    /// </summary>
    public class FailureMechanismAssemblyResult : Observable
    {
        private double manualFailurePathAssemblyProbability;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResult"/>.
        /// </summary>
        public FailureMechanismAssemblyResult()
        {
            ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic;
            ManualFailurePathAssemblyProbability = double.NaN;
        }

        /// <summary>
        /// Gets or sets the <see cref="FailurePathAssemblyProbabilityResultType"/>.
        /// </summary>
        public FailurePathAssemblyProbabilityResultType ProbabilityResultType { get; set; }

        /// <summary>
        /// Gets or sets the probability of a failure mechanism assembly.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        public double ManualFailurePathAssemblyProbability
        {
            get => manualFailurePathAssemblyProbability;
            set
            {
                ProbabilityHelper.ValidateProbability(value, null,
                                                      RiskeerCommonDataResources.FailureProbability_Value_needs_to_be_in_Range_0_,
                                                      true);
                manualFailurePathAssemblyProbability = value;
            }
        }
    }
}