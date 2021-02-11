// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// Factory for creating design variables based on distributions for semi-probabilistic piping calculations.
    /// </summary>
    public static class SemiProbabilisticPipingDesignVariableFactory
    {
        #region General parameters

        /// <summary>
        /// Creates the deterministic design variable for the uplift model factor.
        /// </summary>
        /// <param name="generalPipingInput">The general piping input.</param>
        /// <returns>The created <see cref="DeterministicDesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalPipingInput"/> is <c>null</c>.</exception>
        public static DeterministicDesignVariable<LogNormalDistribution> GetUpliftModelFactorDesignVariable(GeneralPipingInput generalPipingInput)
        {
            if (generalPipingInput == null)
            {
                throw new ArgumentNullException(nameof(generalPipingInput));
            }

            return new DeterministicDesignVariable<LogNormalDistribution>(generalPipingInput.UpliftModelFactor, generalPipingInput.UpliftModelFactor.Mean);
        }

        /// <summary>
        /// Creates the deterministic design variable for the sellmeijer model factor.
        /// </summary>
        /// <param name="generalPipingInput">The general piping input.</param>
        /// <returns>The created <see cref="DeterministicDesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalPipingInput"/> is <c>null</c>.</exception>
        public static DeterministicDesignVariable<LogNormalDistribution> GetSellmeijerModelFactorDesignVariable(GeneralPipingInput generalPipingInput)
        {
            if (generalPipingInput == null)
            {
                throw new ArgumentNullException(nameof(generalPipingInput));
            }

            return new DeterministicDesignVariable<LogNormalDistribution>(generalPipingInput.SellmeijerModelFactor, generalPipingInput.SellmeijerModelFactor.Mean);
        }

        /// <summary>
        /// Creates the deterministic design variable for the critical exit gradient for heave.
        /// </summary>
        /// <param name="generalPipingInput">The general piping input.</param>
        /// <returns>The created <see cref="DeterministicDesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalPipingInput"/> is <c>null</c>.</exception>
        public static DeterministicDesignVariable<LogNormalDistribution> GetCriticalHeaveGradientDesignVariable(GeneralPipingInput generalPipingInput)
        {
            if (generalPipingInput == null)
            {
                throw new ArgumentNullException(nameof(generalPipingInput));
            }

            return new DeterministicDesignVariable<LogNormalDistribution>(generalPipingInput.CriticalHeaveGradient, 0.3);
        }

        /// <summary>
        /// Creates the design variable for the volumic weight of the saturated coverage layer.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="DesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static DesignVariable<LogNormalDistribution> GetSaturatedVolumicWeightOfCoverageLayer(PipingInput pipingInput)
        {
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(pipingInput);
            LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput);

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
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="DesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static DesignVariable<LogNormalDistribution> GetThicknessCoverageLayer(PipingInput pipingInput)
        {
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(pipingInput);

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
        /// <param name="pipingInput">The piping input.</param>
        /// <param name="generalInput">The general piping input.</param>
        /// <returns>The created <see cref="DesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static DesignVariable<LogNormalDistribution> GetEffectiveThicknessCoverageLayer(PipingInput pipingInput, GeneralPipingInput generalInput)
        {
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(pipingInput);
            LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(pipingInput, generalInput);

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
        /// Creates the design variable for the damping factor at the exit point.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="DesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static DesignVariable<LogNormalDistribution> GetDampingFactorExit(PipingInput pipingInput)
        {
            if (pipingInput == null)
            {
                throw new ArgumentNullException(nameof(pipingInput));
            }

            return new LogNormalDistributionDesignVariable(pipingInput.DampingFactorExit)
            {
                Percentile = 0.95
            };
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for the horizontal distance between entry and exit point.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="VariationCoefficientDesignVariable{VariationCoefficientLogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetSeepageLength(PipingInput pipingInput)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(DerivedPipingInput.GetSeepageLength(pipingInput))
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the sieve size through which 70% of the grains of the top part of the aquifer pass.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="VariationCoefficientDesignVariable{VariationCoefficientLogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetDiameter70(PipingInput pipingInput)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(DerivedPipingInput.GetDiameterD70(pipingInput))
            {
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for the Darcy-speed with which water flows through the aquifer layer.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="VariationCoefficientDesignVariable{VariationCoefficientLogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> GetDarcyPermeability(PipingInput pipingInput)
        {
            return new VariationCoefficientLogNormalDistributionDesignVariable(DerivedPipingInput.GetDarcyPermeability(pipingInput))
            {
                Percentile = 0.95
            };
        }

        /// <summary>
        /// Creates the design variable for the total thickness of the aquifer layers at the exit point.
        /// </summary>
        /// <param name="pipingInput">The piping input.</param>
        /// <returns>The created <see cref="DesignVariable{LogNormalDistribution}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/> is <c>null</c>.</exception>
        public static DesignVariable<LogNormalDistribution> GetThicknessAquiferLayer(PipingInput pipingInput)
        {
            return new LogNormalDistributionDesignVariable(DerivedPipingInput.GetThicknessAquiferLayer(pipingInput))
            {
                Percentile = 0.95
            };
        }

        #endregion
    }
}