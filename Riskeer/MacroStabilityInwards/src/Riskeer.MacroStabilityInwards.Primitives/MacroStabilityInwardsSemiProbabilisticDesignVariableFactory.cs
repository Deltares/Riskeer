// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions for macro stability.
    /// </summary>
    public static class MacroStabilityInwardsSemiProbabilisticDesignVariableFactory
    {
        /// <summary>
        /// Creates the design variable for the volumic weight of the layer above the phreatic level.
        /// </summary>
        public static VariationCoefficientDeterministicDesignVariable<VariationCoefficientLogNormalDistribution> GetAbovePhreaticLevel(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientDeterministicDesignVariable<VariationCoefficientLogNormalDistribution>(data.AbovePhreaticLevel, data.AbovePhreaticLevel.Mean);
        }

        /// <summary>
        /// Creates the design variable for the volumic weight of the layer below the phreatic level.
        /// </summary>
        public static VariationCoefficientDeterministicDesignVariable<VariationCoefficientLogNormalDistribution> GetBelowPhreaticLevel(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientDeterministicDesignVariable<VariationCoefficientLogNormalDistribution>(data.BelowPhreaticLevel, data.BelowPhreaticLevel.Mean);
        }

        /// <summary>
        /// Creates the design variable for the cohesion.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetCohesion(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.Cohesion)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the friction angle.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetFrictionAngle(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.FrictionAngle)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the shear strength ratio.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetShearStrengthRatio(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.ShearStrengthRatio)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the strength increase component.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetStrengthIncreaseExponent(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.StrengthIncreaseExponent)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the POP.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetPop(MacroStabilityInwardsSoilLayerData data)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(data.Pop)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for a preconsolidation stress definition.
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