﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions for macro stability.
    /// </summary>
    public static class MacroStabilityInwardsSemiProbabilisticDesignVariableFactory
    {
        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.AbovePhreaticLevel"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetAbovePhreaticLevel(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.AbovePhreaticLevel)
            {
                Percentile = 0.5
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.BelowPhreaticLevel"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetBelowPhreaticLevel(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.BelowPhreaticLevel)
            {
                Percentile = 0.5
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.Cohesion"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetCohesion(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.Cohesion)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.FrictionAngle"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetFrictionAngle(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.FrictionAngle)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.ShearStrengthRatio"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetShearStrengthRatio(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.ShearStrengthRatio)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.StrengthIncreaseExponent"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetStrengthIncreaseExponent(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.StrengthIncreaseExponent)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="MacroStabilityInwardsSoilLayerData.Pop"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetPop(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.Pop)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="IMacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetPreconsolidationStress(IMacroStabilityInwardsPreconsolidationStress preconsolidationStressUnderSurfaceLine)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(preconsolidationStressUnderSurfaceLine.Stress)
            {
                Percentile = 0.05
            };
        }
    }
}