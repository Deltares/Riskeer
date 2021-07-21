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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Probability;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// This class contains the results of a semi-probabilistic assessment of the macro stability inwards
    /// failure mechanism.
    /// </summary>
    public class DerivedMacroStabilityInwardsOutput
    {
        private double macroStabilityInwardsProbability;

        /// <summary>
        /// Creates a new instance of <see cref="DerivedMacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="factorOfStability">The calculated factor of stability of the macro stability inwards failure mechanism.</param>
        /// <param name="macroStabilityInwardsProbability">The calculated probability of failing due to macro stability inwards.</param>
        /// <param name="macroStabilityInwardsReliability">The calculated reliability of the macro stability inwards failure mechanism.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a probability that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public DerivedMacroStabilityInwardsOutput(double factorOfStability, double macroStabilityInwardsProbability, double macroStabilityInwardsReliability)
        {
            FactorOfStability = new RoundedDouble(3, factorOfStability);
            MacroStabilityInwardsProbability = macroStabilityInwardsProbability;
            MacroStabilityInwardsReliability = new RoundedDouble(5, macroStabilityInwardsReliability);
        }

        /// <summary>
        /// Gets the factor of stability of the macro stability inwards failure mechanism.
        /// </summary>
        public RoundedDouble FactorOfStability { get; }

        /// <summary>
        /// Gets the probability of failing due to the macro stability inwards failure mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double MacroStabilityInwardsProbability
        {
            get
            {
                return macroStabilityInwardsProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                macroStabilityInwardsProbability = value;
            }
        }
        
        /// <summary>
        /// Gets the reliability of the macro stability inwards failure mechanism.
        /// </summary>
        public RoundedDouble MacroStabilityInwardsReliability { get; }
    }
}