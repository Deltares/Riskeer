// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    /// Factory for creating design variables based on probabilistic distributions.
    /// </summary>
    public static class PipingSemiProbabilisticDesignValueFactory
    {
        private static DesignVariable<NormalDistribution> CreateDesignVariable(NormalDistribution distribution, double percentile)
        {
            return new NormalDistributionDesignVariable(distribution)
            {
                Percentile = percentile
            };
        }

        private static DesignVariable<LogNormalDistribution> CreateDesignVariable(LogNormalDistribution distribution, double percentile)
        {
            return new LogNormalDistributionDesignVariable(distribution)
            {
                Percentile = percentile
            };
        }

        private static DesignVariable<LogNormalDistribution> CreateDeterministicDesignVariable(LogNormalDistribution distribution, double deterministicValue)
        {
            return new DeterministicDesignVariable<LogNormalDistribution>(distribution, deterministicValue);
        }

        #region General parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.SaturatedVolumicWeightOfCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetSaturatedVolumicWeightOfCoverageLayer(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.SaturatedVolumicWeightOfCoverageLayer, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.ThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetThicknessCoverageLayer(PipingInput parameters)
        {
            if (double.IsNaN(parameters.ThicknessCoverageLayer.Mean))
            {
                return CreateDeterministicDesignVariable(parameters.ThicknessCoverageLayer, 0);
            }
            return CreateDesignVariable(parameters.ThicknessCoverageLayer, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.EffectiveThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetEffectiveThicknessCoverageLayer(PipingInput parameters)
        {
            if (double.IsNaN(parameters.EffectiveThicknessCoverageLayer.Mean))
            {
                return CreateDeterministicDesignVariable(parameters.EffectiveThicknessCoverageLayer, 0);
            }
            return CreateDesignVariable(parameters.EffectiveThicknessCoverageLayer, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.PhreaticLevelExit"/>.
        /// </summary>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.PhreaticLevelExit, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.DampingFactorExit"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetDampingFactorExit(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.DampingFactorExit, 0.95);
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.SeepageLength"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetSeepageLength(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.SeepageLength, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.Diameter70"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetDiameter70(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.Diameter70, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.DarcyPermeability"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetDarcyPermeability(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.DarcyPermeability, 0.95);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.ThicknessAquiferLayer"/>.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetThicknessAquiferLayer(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.ThicknessAquiferLayer, 0.95);
        }

        #endregion
    }
}