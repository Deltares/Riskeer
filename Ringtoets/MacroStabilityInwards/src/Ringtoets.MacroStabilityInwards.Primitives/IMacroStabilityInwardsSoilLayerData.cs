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
using System.Drawing;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Interface for defining the data of a soil layer.
    /// </summary>
    public interface IMacroStabilityInwardsSoilLayerData
    {
        /// <summary>
        /// Gets or sets a value indicating whether the layer is an aquifer.
        /// </summary>
        bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the layer.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        string MaterialName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> that was used to represent the layer.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use POP for the layer.
        /// </summary>
        bool UsePop { get; set; }

        /// <summary>
        /// Gets or sets the shear strength model to use for the layer.
        /// </summary>
        MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        VariationCoefficientLogNormalDistribution AbovePhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        VariationCoefficientLogNormalDistribution BelowPhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the cohesion.
        /// [kN/m³]
        /// </summary>
        VariationCoefficientLogNormalDistribution Cohesion { get; set; }

        /// <summary>
        /// Gets or sets the friction angle.
        /// [°]
        /// </summary>
        VariationCoefficientLogNormalDistribution FrictionAngle { get; set; }

        /// <summary>
        /// Gets or sets the strength increase component.
        /// [-]
        /// </summary>
        VariationCoefficientLogNormalDistribution StrengthIncreaseExponent { get; set; }

        /// <summary>
        /// Gets or sets the shear strength ratio.
        /// [-]
        /// </summary>
        VariationCoefficientLogNormalDistribution ShearStrengthRatio { get; set; }

        /// <summary>
        /// Gets or sets the POP.
        /// [kN/m²]
        /// </summary>
        VariationCoefficientLogNormalDistribution Pop { get; set; }
    }
}