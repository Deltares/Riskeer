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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents a preconsolidation stress definition that was imported
    /// from D-Soil model.
    /// </summary>
    public class MacroStabilityInwardsPreconsolidationStress
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the preconsolidation stress location.</param>
        /// <param name="zCoordinate">The z coordinate of the preconsolidation stress location.</param>
        /// <param name="preconsolidationStressCalculationValue">The stress calculation value 
        /// of the preconsolidation stress.</param>
        /// <param name="preconsolidationStressMean">The mean of the stochastic distribution 
        /// for the preconsolidation stress.</param>
        /// <param name="preconsolidationStressCoefficientOfVariation">The coefficient of 
        /// variation of the stochastic distribution for the preconsolidation stress.</param>
        /// <param name="preconsolidationStressShift">The shift of the stochastic distribution 
        /// for the preconsolidation stress</param>
        public MacroStabilityInwardsPreconsolidationStress(double xCoordinate,
                                                           double zCoordinate,
                                                           double preconsolidationStressCalculationValue,
                                                           double preconsolidationStressMean,
                                                           double preconsolidationStressCoefficientOfVariation,
                                                           double preconsolidationStressShift)
        {
            XCoordinate = xCoordinate;
            ZCoordinate = zCoordinate;
            PreconsolidationStressCalculationValue = preconsolidationStressCalculationValue;
            PreconsolidationStressMean = preconsolidationStressMean;
            PreconsolidationStressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation;
            PreconsolidationStressShift = preconsolidationStressShift;
        }

        /// <summary>
        /// Gets the value representing the X coordinate of the preconsolidation stress location.
        /// [m]
        /// </summary>
        public double XCoordinate { get; }

        /// <summary>
        /// Gets the value representing the Z coordinate of the preconsolidation stress location.
        /// [m]
        /// </summary>
        public double ZCoordinate { get; }

        /// <summary>
        /// Gets the value representing the calculation value of the preconsolidation stress.
        /// [kN/m設
        /// </summary>
        public double PreconsolidationStressCalculationValue { get; }

        /// <summary>
        /// Gets the value representing the mean of the distribution for the preconsolidation stress.
        /// [kN/m設
        /// </summary>
        public double PreconsolidationStressMean { get; }

        /// <summary>
        /// Gets the value representing the coefficient of variation of the distribution for the preconsolidation stress.
        /// [kN/m設
        /// </summary>
        public double PreconsolidationStressCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the value representing the shift of the distribution for the preconsolidation stress.
        /// [kN/m設
        /// </summary>
        public double PreconsolidationStressShift { get; }
    }
}