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

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions for piping.
    /// </summary>
    public static class PipingSemiProbabilisticDesignValueFactory
    {
        #region General parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.SaturatedVolumicWeightOfCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetSaturatedVolumicWeightOfCoverageLayer(PipingInput parameters)
        {
            if (double.IsNaN(parameters.ThicknessCoverageLayer.Mean))
            {
                return new DeterministicDesignVariable<LogNormalDistribution>(parameters.SaturatedVolumicWeightOfCoverageLayer);
            }

            return new LogNormalDistributionDesignVariable(parameters.SaturatedVolumicWeightOfCoverageLayer)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.ThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetThicknessCoverageLayer(PipingInput parameters)
        {
            if (double.IsNaN(parameters.ThicknessCoverageLayer.Mean))
            {
                return new DeterministicDesignVariable<LogNormalDistribution>(parameters.ThicknessCoverageLayer);
            }

            return new LogNormalDistributionDesignVariable(parameters.ThicknessCoverageLayer)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.EffectiveThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetEffectiveThicknessCoverageLayer(PipingInput parameters)
        {
            if (double.IsNaN(parameters.ThicknessCoverageLayer.Mean))
            {
                return new DeterministicDesignVariable<LogNormalDistribution>(parameters.EffectiveThicknessCoverageLayer);
            }

            return new LogNormalDistributionDesignVariable(parameters.EffectiveThicknessCoverageLayer)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.PhreaticLevelExit"/>.
        /// </summary>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingInput parameters)
        {
            return new NormalDistributionDesignVariable(parameters.PhreaticLevelExit)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.DampingFactorExit"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetDampingFactorExit(PipingInput parameters)
        {
            return new LogNormalDistributionDesignVariable(parameters.DampingFactorExit)
            {
                Percentile = 0.95
            };
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.SeepageLength"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetSeepageLength(PipingInput parameters)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(parameters.SeepageLength)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.Diameter70"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetDiameter70(PipingInput parameters)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(parameters.Diameter70)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.DarcyPermeability"/>.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetDarcyPermeability(PipingInput parameters)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(parameters.DarcyPermeability)
            {
                Percentile = 0.95
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.ThicknessAquiferLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetThicknessAquiferLayer(PipingInput parameters)
        {
            return new LogNormalDistributionDesignVariable(parameters.ThicknessAquiferLayer)
            {
                Percentile = 0.95
            };
        }

        #endregion
    }
}