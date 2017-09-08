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
        /// <param name="preconsolidationStressMean">The mean of the stochastic distribution 
        /// for the preconsolidation stress.</param>
        /// <param name="preconsolidationStressCoefficientOfVariation">The coefficient of 
        /// variation of the stochastic distribution for the preconsolidation stress.</param>
        /// <param name="preconsolidationStressShift">The shift of the stochastic distribution 
        /// for the preconsolidation stress</param>
        public MacroStabilityInwardsPreconsolidationStress(double xCoordinate,
                                                           double zCoordinate,
                                                           double preconsolidationStressMean,
                                                           double preconsolidationStressCoefficientOfVariation,
                                                           double preconsolidationStressShift)
        {
            XCoordinate = xCoordinate;
            ZCoordinate = zCoordinate;
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
        /// Gets the value representing the mean of the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double PreconsolidationStressMean { get; }

        /// <summary>
        /// Gets the value representing the coefficient of variation of the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double PreconsolidationStressCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the value representing the shift of the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double PreconsolidationStressShift { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((MacroStabilityInwardsPreconsolidationStress) obj);
        }

        private bool Equals(MacroStabilityInwardsPreconsolidationStress other)
        {
            return XCoordinate.Equals(other.XCoordinate)
                   && ZCoordinate.Equals(other.ZCoordinate)
                   && PreconsolidationStressMean.Equals(other.PreconsolidationStressMean)
                   && PreconsolidationStressCoefficientOfVariation.Equals(other.PreconsolidationStressCoefficientOfVariation)
                   && PreconsolidationStressShift.Equals(other.PreconsolidationStressShift);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = XCoordinate.GetHashCode();
                hashCode = (hashCode * 397) ^ ZCoordinate.GetHashCode();
                hashCode = (hashCode * 397) ^ PreconsolidationStressMean.GetHashCode();
                hashCode = (hashCode * 397) ^ PreconsolidationStressCoefficientOfVariation.GetHashCode();
                hashCode = (hashCode * 397) ^ PreconsolidationStressShift.GetHashCode();
                return hashCode;
            }
        }
    }
}