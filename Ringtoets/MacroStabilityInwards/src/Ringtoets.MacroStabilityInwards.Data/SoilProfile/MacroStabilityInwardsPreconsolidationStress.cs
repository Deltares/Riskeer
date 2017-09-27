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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Primitives.Properties;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
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
        /// <exception cref="ArgumentException">Thrown when any of the parameters are <see cref="double.NaN"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="preconsolidationStressMean"/> is less than or equal to 0.</item>
        /// <item><paramref name="preconsolidationStressCoefficientOfVariation"/> is less than 0.</item>
        /// </list></exception>
        public MacroStabilityInwardsPreconsolidationStress(double xCoordinate,
                                                           double zCoordinate,
                                                           double preconsolidationStressMean,
                                                           double preconsolidationStressCoefficientOfVariation)
        {
            ValidateParameterNaN(xCoordinate, Resources.MacroStabilityInwardsPreconsolidationStress_XCoordinate_ParameterName);
            ValidateParameterNaN(zCoordinate, Resources.MacroStabilityInwardsPreconsolidationStress_ZCoordinate_ParameterName);
            ValidateParameterNaN(preconsolidationStressMean, Resources.MacroStabilityInwardsPreconsolidationStress_PreconsolidationStressMean_ParameterName);
            ValidateParameterNaN(preconsolidationStressCoefficientOfVariation, Resources.MacroStabilityInwardsPreconsolidationStress_PreconsolidationStressCoefficientOfVariation_ParameterName);

            new RoundedDouble(2, xCoordinate);
            new RoundedDouble(2, zCoordinate);

            Location = new Point2D(xCoordinate, zCoordinate);

            Stress = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) preconsolidationStressMean,
                CoefficientOfVariation = (RoundedDouble) preconsolidationStressCoefficientOfVariation
            };
        }

        /// <summary>
        /// Gets the distribution for the preconsolidation stress.
        /// [kN/m²]
        /// </summary>
        public VariationCoefficientLogNormalDistribution Stress { get; }

        /// <summary>
        /// Gets the location of the preconsolidation stress
        /// [m]
        /// </summary>
        /// <remarks>The <see cref="Point2D.Y"/> has the unit [m+NAP]</remarks>
        public Point2D Location { get; }

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

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Location.GetHashCode();
                hashCode = (hashCode * 397) ^ Stress.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Validates that the <paramref name="value"/> is not a <see cref="double.NaN"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="parameterName">The name of the parameter to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the parameter value is <see cref="double.NaN"/>.</exception>
        private static void ValidateParameterNaN(double value, string parameterName)
        {
            if (double.IsNaN(value))
            {
                string message = string.Format(Resources.MacroStabilityInwardsPreconsolidationStress_ValidateParameter_The_value_of_ParameterName_0_must_be_a_concrete_value, parameterName);
                throw new ArgumentException(message);
            }
        }

        private bool Equals(MacroStabilityInwardsPreconsolidationStress other)
        {
            return Location.Equals(other.Location)
                   && Stress.Equals(other.Stress);
        }
    }
}