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

using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions for macrostability.
    /// </summary>
    public static class MacroStabilityInwardsSemiProbabilisticDesignValueFactory
    {
        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.AbovePhreaticLevel"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetAbovePhreaticLevel(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.AbovePhreaticLevel, 0.5);
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.BelowPhreaticLevel"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetBelowPhreaticLevel(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.BelowPhreaticLevel, 0.5);
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.Cohesion"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetCohesion(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.Cohesion, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.FrictionAngle"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetFrictionAngle(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.FrictionAngle, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ShearStrengthRatio"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetShearStrengthRatio(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.ShearStrengthRatio, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.StrengthIncreaseExponent"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetStrengthIncreaseExponent(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.StrengthIncreaseExponent, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.Pop"/>.
        /// </summary>
        public static VariationCoefficientLogNormalDistributionDesignVariable GetPop(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties)
        {
            return SemiProbabilisticDesignValueFactory.CreateDesignVariable(properties.Pop, 0.05);
        }
    }
}
