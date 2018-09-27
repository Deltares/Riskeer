// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    public static class PipingSemiProbabilisticDesignVariableFactory
    {
        #region General parameters

        /// <summary>
        /// Creates the design variable for the volumic weight of the saturated coverage layer.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetSaturatedVolumicWeightOfCoverageLayer(PipingInput parameters)
        {
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(parameters);
            LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(parameters);

            if (double.IsNaN(thicknessCoverageLayer.Mean))
            {
                return new DeterministicDesignVariable<LogNormalDistribution>(saturatedVolumicWeightOfCoverageLayer);
            }

            return new LogNormalDistributionDesignVariable(saturatedVolumicWeightOfCoverageLayer)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the total thickness of the coverage layers at the exit point.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetThicknessCoverageLayer(PipingInput parameters)
        {
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(parameters);

            if (double.IsNaN(thicknessCoverageLayer.Mean))
            {
                return new DeterministicDesignVariable<LogNormalDistribution>(thicknessCoverageLayer);
            }

            return new LogNormalDistributionDesignVariable(thicknessCoverageLayer)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the effective thickness of the coverage layers at the exit point.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetEffectiveThicknessCoverageLayer(PipingInput parameters)
        {
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(parameters);
            LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(parameters);

            if (double.IsNaN(thicknessCoverageLayer.Mean))
            {
                return new DeterministicDesignVariable<LogNormalDistribution>(effectiveThicknessCoverageLayer);
            }

            return new LogNormalDistributionDesignVariable(effectiveThicknessCoverageLayer)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the phreatic level at the exit point.
        /// </summary>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingInput parameters)
        {
            return new NormalDistributionDesignVariable(parameters.PhreaticLevelExit)
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the damping factor at the exit point.
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
        /// Creates the design variable for the horizontal distance between entry and exit point.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetSeepageLength(PipingInput parameters)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(DerivedPipingInput.GetSeepageLength(parameters))
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the sieve size through which 70% of the grains of the top part of the aquifer pass.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetDiameter70(PipingInput parameters)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(DerivedPipingInput.GetDiameterD70(parameters))
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the Darcy-speed with which water flows through the aquifer layer.
        /// </summary>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetDarcyPermeability(PipingInput parameters)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(DerivedPipingInput.GetDarcyPermeability(parameters))
            {
                Percentile = 0.95
            };
        }

        /// <summary>
        /// Creates the design variable for the total thickness of the aquifer layers at the exit point.
        /// </summary>
        public static DesignVariable<LogNormalDistribution> GetThicknessAquiferLayer(PipingInput parameters)
        {
            return new LogNormalDistributionDesignVariable(DerivedPipingInput.GetThicknessAquiferLayer(parameters))
            {
                Percentile = 0.95
            };
        }

        #endregion
    }
}