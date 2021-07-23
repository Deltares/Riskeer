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
using Core.Common.Util;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Factory class to create <see cref="DerivedMacroStabilityInwardsOutput"/>.
    /// </summary>
    public static class DerivedMacroStabilityInwardsOutputFactory
    {
        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="MacroStabilityInwardsCalculation"/> with <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="output">The output of a calculation.</param>
        /// <param name="modelFactor">The model factor used to calculate a reliability from a stability factor.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        public static DerivedMacroStabilityInwardsOutput Create(MacroStabilityInwardsOutput output,
                                                                double modelFactor)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            double factorOfStability = output.FactorOfStability;

            double macroStabilityInwardsReliability = CalculateEstimatedReliability(factorOfStability, modelFactor);
            double macroStabilityInwardsProbability = StatisticsConverter.ReliabilityToProbability(macroStabilityInwardsReliability);

            return new DerivedMacroStabilityInwardsOutput(factorOfStability,
                                                          macroStabilityInwardsProbability,
                                                          macroStabilityInwardsReliability);
        }

        /// <summary>
        /// Calculates the estimated reliability of the macro stability inwards failure mechanism 
        /// based on the stability factor and model factor.
        /// </summary>
        /// <param name="factorOfStability">The factory of stability to calculate the reliability for.</param>
        /// <param name="modelFactor">The model factor of the calculation result.</param>
        /// <returns>The estimated reliability based on the stability and model factor.</returns>
        private static double CalculateEstimatedReliability(double factorOfStability, double modelFactor)
        {
            return ((factorOfStability / modelFactor) - 0.41) / 0.15;
        }
    }
}