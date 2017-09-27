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

using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions for macro stability.
    /// </summary>
    public static class MacroStabilityInwardsSemiProbabilisticDesignVariableFactory
    {
        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.AbovePhreaticLevel"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetAbovePhreaticLevel(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.AbovePhreaticLevel)
            {
                Percentile = 0.5
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.BelowPhreaticLevel"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetBelowPhreaticLevel(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.BelowPhreaticLevel)
            {
                Percentile = 0.5
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.Cohesion"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetCohesion(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.Cohesion)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.FrictionAngle"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetFrictionAngle(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.FrictionAngle)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.ShearStrengthRatio"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetShearStrengthRatio(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.ShearStrengthRatio)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.StrengthIncreaseExponent"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetStrengthIncreaseExponent(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.StrengthIncreaseExponent)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerProperties.Pop"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetPop(MacroStabilityInwardsSoilLayerProperties properties)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(properties.Pop)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetPreconsolidationStress(MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine preconsolidationStressUnderSurfaceLine)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(preconsolidationStressUnderSurfaceLine.PreconsolidationStress)
            {
                Percentile = 0.05
            };
        }
    }
}