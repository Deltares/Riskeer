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
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine
{
    /// <summary>
    /// Preconsolidation stress properties.
    /// </summary>
    public class MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="properties">The object containing the value for the properties of 
        /// the new <see cref="MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            XCoordinate = properties.XCoordinate;
            ZCoordinate = properties.ZCoordinate;
            PreconsolidationStress = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.PreconsolidationStressMean,
                CoefficientOfVariation = (RoundedDouble) properties.PreconsolidationStressCoefficientOfVariation
            };
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
        /// Gets the preconsolidation stress distribution.
        /// [kN/m³]
        /// </summary>
        public VariationCoefficientLogNormalDistribution PreconsolidationStress { get; }

        /// <summary>
        /// Gets or sets the preconsolidation stress distribution design variable.
        /// [kN/m³]
        /// </summary>
        public RoundedDouble PreconsolidationStressDesignVariable { get; set; } = RoundedDouble.NaN;

        /// <summary>
        /// Class holding the properties for constructing a 
        /// <see cref="MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value representing the X coordinate of the preconsolidation stress location.
            /// [m]
            /// </summary>
            public double XCoordinate { get; set; } = double.NaN;

            /// <summary>
            /// Gets or sets the value representing the Z coordinate of the preconsolidation stress location.
            /// [m]
            /// </summary>
            public double ZCoordinate { get; set; } = double.NaN;

            /// <summary>
            /// Gets or sets the mean of the preconsolidation stress distribution.
            /// [kN/m³]
            /// </summary>
            public double PreconsolidationStressMean { get; set; } = double.NaN;

            /// <summary>
            /// Gets or sets the coefficient of variation of the preconsolidation stress distribution.
            /// [kN/m³]
            /// </summary>
            public double PreconsolidationStressCoefficientOfVariation { get; set; } = double.NaN;
        }
    }
}