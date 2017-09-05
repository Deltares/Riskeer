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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Soil layer properties.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.AbovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) properties.AbovePhreaticLevelCoefficientOfVariation
            };

            IsAquifer = properties.IsAquifer;
            UsePop = properties.UsePop;
            ShearStrengthModel = properties.ShearStrengthModel;
            MaterialName = properties.MaterialName;
        }

        /// <summary>
        /// Gets the distribution for the volumic weight of the layer above the phreatic level.
        /// </summary>
        public VariationCoefficientLogNormalDistribution AbovePhreaticLevel { get; }

        /// <summary>
        /// Gets the design variable of the distribution for the volumic weight of the layer above the phreatic level.
        /// </summary>
        public RoundedDouble AbovePhreaticLevelDesignVariable { get; set; }

        /// <summary>
        /// Gets a value indicating whether the layer is an aquifer.
        /// </summary>
        public bool IsAquifer { get; }

        /// <summary>
        /// Gets a value indicating whether to use POP for the layer.
        /// </summary>
        public bool UsePop { get; }

        /// <summary>
        /// Gets the shear strength model to use for the layer.
        /// </summary>
        public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; }

        /// <summary>
        /// Gets the name of the material that was assigned to the layer.
        /// </summary>
        public string MaterialName { get; }

        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                AbovePhreaticLevelMean = double.NaN;
                AbovePhreaticLevelCoefficientOfVariation = double.NaN;
                ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.None;
                MaterialName = string.Empty;
            }

            /// <summary>
            /// Gets or sets the mean of the distribution for the volumic weight of the layer above the phreatic level.
            /// [kN/m³]
            /// </summary>
            public double AbovePhreaticLevelMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the volumic weight of the layer above the phreatic level.
            /// [kN/m³]
            /// </summary>
            public double AbovePhreaticLevelCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the layer is an aquifer.
            /// </summary>
            public bool IsAquifer { internal get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to use POP for the layer.
            /// </summary>
            public bool UsePop { internal get; set; }

            /// <summary>
            /// Gets or sets the shear strength model to use for the layer.
            /// </summary>
            public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { internal get; set; }

            /// <summary>
            /// Gets or sets the name of the material that was assigned to the layer.
            /// </summary>
            public string MaterialName { internal get; set; }
        }
    }
}