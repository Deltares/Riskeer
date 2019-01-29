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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.InputParameterCalculation;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Class responsible for calculating the derived piping input.
    /// </summary>
    public static class DerivedPipingInput
    {
        /// <summary>
        /// Gets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static RoundedDouble GetPiezometricHeadExit(PipingInput input, RoundedDouble assessmentLevel)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            RoundedDouble dampingFactorExit = PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue();
            RoundedDouble phreaticLevelExit = PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue();

            return new RoundedDouble(2, InputParameterCalculationService.CalculatePiezometricHeadAtExit(assessmentLevel,
                                                                                                        dampingFactorExit,
                                                                                                        phreaticLevelExit));
        }

        /// <summary>
        /// Gets the horizontal distance between entry and exit point.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static VariationCoefficientLogNormalDistribution GetSeepageLength(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            double seepageLengthMean = input.ExitPointL - input.EntryPointL;

            return new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) seepageLengthMean,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };
        }

        /// <summary>
        /// Gets the total thickness of the coverage layers at the exit point.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static LogNormalDistribution GetThicknessCoverageLayer(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var thicknessCoverageLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.5
            };

            UpdateThicknessCoverageLayerMean(input, thicknessCoverageLayer);

            return thicknessCoverageLayer;
        }

        /// <summary>
        /// Gets the effective thickness of the coverage layers at the exit point.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static LogNormalDistribution GetEffectiveThicknessCoverageLayer(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var thicknessCoverageLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.5
            };

            UpdateEffectiveThicknessCoverageLayerMean(input, thicknessCoverageLayer);

            return thicknessCoverageLayer;
        }

        /// <summary>
        /// Gets the total thickness of the aquifer layers at the exit point.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static LogNormalDistribution GetThicknessAquiferLayer(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var thicknessAquiferLayer = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = (RoundedDouble) 0.5
            };

            UpdateThicknessAquiferLayerMean(input, thicknessAquiferLayer);

            return thicknessAquiferLayer;
        }

        /// <summary>
        /// Gets the sieve size through which 70% of the grains of the top part of the aquifer pass.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static VariationCoefficientLogNormalDistribution GetDiameterD70(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            PipingSoilLayer topMostAquiferLayer = GetConsecutiveAquiferLayers(input).FirstOrDefault();

            return topMostAquiferLayer != null
                       ? topMostAquiferLayer.DiameterD70
                       : new VariationCoefficientLogNormalDistribution(6)
                       {
                           Mean = RoundedDouble.NaN,
                           CoefficientOfVariation = RoundedDouble.NaN
                       };
        }

        /// <summary>
        /// Gets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static VariationCoefficientLogNormalDistribution GetDarcyPermeability(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var distribution = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            UpdateDarcyPermeabilityParameters(input, distribution);

            return distribution;
        }

        /// <summary>
        /// Gets the volumic weight of the saturated coverage layer.
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static LogNormalDistribution GetSaturatedVolumicWeightOfCoverageLayer(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var distribution = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            };

            UpdateSaturatedVolumicWeightOfCoverageLayerParameters(input, distribution);

            return distribution;
        }

        private static void UpdateThicknessAquiferLayerMean(PipingInput input, LogNormalDistribution thicknessAquiferLayer)
        {
            PipingStochasticSoilProfile stochasticSoilProfile = input.StochasticSoilProfile;
            PipingSurfaceLine surfaceLine = input.SurfaceLine;
            RoundedDouble exitPointL = input.ExitPointL;

            if (stochasticSoilProfile?.SoilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
            {
                var thicknessTopAquiferLayer = new RoundedDouble(GetNumberOfDecimals(thicknessAquiferLayer),
                                                                 GetThicknessTopAquiferLayer(stochasticSoilProfile.SoilProfile, surfaceLine, exitPointL));

                if (thicknessTopAquiferLayer > 0)
                {
                    thicknessAquiferLayer.Mean = thicknessTopAquiferLayer;
                }
            }
        }

        private static void UpdateThicknessCoverageLayerMean(PipingInput input, LogNormalDistribution thicknessCoverageLayerDistribution)
        {
            PipingStochasticSoilProfile stochasticSoilProfile = input.StochasticSoilProfile;
            PipingSurfaceLine surfaceLine = input.SurfaceLine;
            RoundedDouble exitPointL = input.ExitPointL;

            if (stochasticSoilProfile?.SoilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
            {
                var weightedMean = new RoundedDouble(GetNumberOfDecimals(thicknessCoverageLayerDistribution),
                                                     GetThicknessCoverageLayers(stochasticSoilProfile.SoilProfile, surfaceLine, exitPointL));

                if (weightedMean > 0)
                {
                    thicknessCoverageLayerDistribution.Mean = weightedMean;
                }
            }
        }

        private static void UpdateEffectiveThicknessCoverageLayerMean(PipingInput input, LogNormalDistribution effectiveThicknessCoverageLayerDistribution)
        {
            if (input.SurfaceLine != null && input.StochasticSoilProfile?.SoilProfile != null && !double.IsNaN(input.ExitPointL))
            {
                var weightedMean = new RoundedDouble(GetNumberOfDecimals(effectiveThicknessCoverageLayerDistribution),
                                                     InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                                                         input.WaterVolumetricWeight,
                                                         PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                                                         input.ExitPointL,
                                                         input.SurfaceLine,
                                                         input.StochasticSoilProfile.SoilProfile));

                if (weightedMean > 0)
                {
                    effectiveThicknessCoverageLayerDistribution.Mean = weightedMean;
                }
            }
        }

        private static void UpdateDarcyPermeabilityParameters(PipingInput input, VariationCoefficientLogNormalDistribution darcyPermeabilityDistribution)
        {
            IEnumerable<PipingSoilLayer> aquiferLayers = GetConsecutiveAquiferLayers(input);

            int numberOfDecimals = GetNumberOfDecimals(darcyPermeabilityDistribution);

            if (HasCorrectDarcyPermeabilityWeightDistributionParameterDefinition(aquiferLayers))
            {
                PipingSoilLayer topMostAquiferLayer = aquiferLayers.First();

                var weightedMean = new RoundedDouble(numberOfDecimals,
                                                     GetWeightedMeanForDarcyPermeabilityOfAquiferLayer(aquiferLayers,
                                                                                                       input.StochasticSoilProfile.SoilProfile,
                                                                                                       input.SurfaceLine.GetZAtL(input.ExitPointL)));

                darcyPermeabilityDistribution.Mean = weightedMean;
                darcyPermeabilityDistribution.CoefficientOfVariation = topMostAquiferLayer.Permeability.CoefficientOfVariation;
            }
        }

        private static void UpdateSaturatedVolumicWeightOfCoverageLayerParameters(PipingInput input, LogNormalDistribution volumicWeightDistribution)
        {
            IEnumerable<PipingSoilLayer> coverageLayers = GetConsecutiveCoverageLayers(input);

            int numberOfDecimals = GetNumberOfDecimals(volumicWeightDistribution);

            if (HasCorrectSaturatedWeightDistributionParameterDefinition(coverageLayers))
            {
                PipingSoilLayer topMostAquitardLayer = coverageLayers.First();
                volumicWeightDistribution.Shift = topMostAquitardLayer.BelowPhreaticLevel.Shift;
                volumicWeightDistribution.StandardDeviation = topMostAquitardLayer.BelowPhreaticLevel.StandardDeviation;

                var weightedMean = new RoundedDouble(numberOfDecimals,
                                                     GetWeightedMeanForVolumicWeightOfCoverageLayer(
                                                         coverageLayers,
                                                         input.StochasticSoilProfile.SoilProfile,
                                                         input.SurfaceLine.GetZAtL(input.ExitPointL)));

                if (weightedMean > 0)
                {
                    volumicWeightDistribution.Mean = weightedMean;
                }
            }
        }

        private static int GetNumberOfDecimals(IDistribution distribution)
        {
            return distribution.Mean.NumberOfDecimalPlaces;
        }

        private static int GetNumberOfDecimals(IVariationCoefficientDistribution distribution)
        {
            return distribution.Mean.NumberOfDecimalPlaces;
        }

        private static bool HasCorrectSaturatedWeightDistributionParameterDefinition(IEnumerable<PipingSoilLayer> consecutiveAquitardLayers)
        {
            if (!consecutiveAquitardLayers.Any())
            {
                return false;
            }

            IEnumerable<LogNormalDistribution> distributions = consecutiveAquitardLayers.Select(layer => layer.BelowPhreaticLevel);
            if (distributions.Count() == 1)
            {
                return true;
            }

            return distributions.All(currentLayerDistribution => AreShiftAndDeviationEqual(
                                         currentLayerDistribution,
                                         distributions.First()));
        }

        private static bool HasCorrectDarcyPermeabilityWeightDistributionParameterDefinition(IEnumerable<PipingSoilLayer> consecutiveAquitardLayers)
        {
            if (!consecutiveAquitardLayers.Any())
            {
                return false;
            }

            IEnumerable<VariationCoefficientLogNormalDistribution> distributions = consecutiveAquitardLayers.Select(layer => layer.Permeability);
            if (distributions.Count() == 1)
            {
                return true;
            }

            return distributions.All(currentLayerDistribution => AreCoefficientEqual(
                                         currentLayerDistribution,
                                         distributions.First()));
        }

        private static bool AreShiftAndDeviationEqual(LogNormalDistribution currentLayerDistribution, LogNormalDistribution baseLayerDistribution)
        {
            return currentLayerDistribution.StandardDeviation == baseLayerDistribution.StandardDeviation &&
                   currentLayerDistribution.Shift == baseLayerDistribution.Shift;
        }

        private static bool AreCoefficientEqual(VariationCoefficientLogNormalDistribution currentLayerDistribution,
                                                VariationCoefficientLogNormalDistribution baseLayerDistribution)
        {
            return Math.Abs(baseLayerDistribution.CoefficientOfVariation - currentLayerDistribution.CoefficientOfVariation) < 1e-6;
        }

        private static double GetWeightedMeanForVolumicWeightOfCoverageLayer(IEnumerable<PipingSoilLayer> aquitardLayers,
                                                                             PipingSoilProfile profile,
                                                                             double surfaceLevel)
        {
            var totalThickness = 0.0;
            var weighedTotal = 0.0;

            foreach (PipingSoilLayer layer in aquitardLayers)
            {
                double layerThickness = profile.GetLayerThickness(layer);
                double bottom = layer.Top - layerThickness;
                double thicknessUnderSurface = Math.Min(layer.Top, surfaceLevel) - bottom;

                totalThickness += thicknessUnderSurface;
                weighedTotal += layer.BelowPhreaticLevel.Mean * thicknessUnderSurface;
            }

            return weighedTotal / totalThickness;
        }

        private static double GetWeightedMeanForDarcyPermeabilityOfAquiferLayer(IEnumerable<PipingSoilLayer> aquitardLayers,
                                                                                PipingSoilProfile profile,
                                                                                double surfaceLevel)
        {
            var totalThickness = 0.0;
            var weighedTotal = 0.0;

            foreach (PipingSoilLayer layer in aquitardLayers)
            {
                double layerThickness = profile.GetLayerThickness(layer);
                double bottom = layer.Top - layerThickness;
                double thicknessUnderSurface = Math.Min(layer.Top, surfaceLevel) - bottom;

                totalThickness += thicknessUnderSurface;
                weighedTotal += layer.Permeability.Mean * thicknessUnderSurface;
            }

            return weighedTotal / totalThickness;
        }

        private static IEnumerable<PipingSoilLayer> GetConsecutiveAquiferLayers(PipingInput input)
        {
            PipingSurfaceLine surfaceLine = input.SurfaceLine;
            PipingSoilProfile soilProfile = input.StochasticSoilProfile?.SoilProfile;
            RoundedDouble exitPointL = input.ExitPointL;

            if (surfaceLine != null && soilProfile != null && !double.IsNaN(exitPointL))
            {
                return soilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLine.GetZAtL(exitPointL)).ToArray();
            }

            return new PipingSoilLayer[0];
        }

        private static IEnumerable<PipingSoilLayer> GetConsecutiveCoverageLayers(PipingInput input)
        {
            PipingSurfaceLine surfaceLine = input.SurfaceLine;
            PipingSoilProfile soilProfile = input.StochasticSoilProfile?.SoilProfile;
            RoundedDouble exitPointL = input.ExitPointL;

            if (surfaceLine != null && soilProfile != null && !double.IsNaN(exitPointL))
            {
                PipingSoilLayer[] consecutiveAquitardLayersBelowLevel = soilProfile
                                                                        .GetConsecutiveCoverageLayersBelowLevel(surfaceLine.GetZAtL(exitPointL))
                                                                        .ToArray();

                return consecutiveAquitardLayersBelowLevel;
            }

            return new PipingSoilLayer[0];
        }

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, PipingSurfaceLine surfaceLine, RoundedDouble exitPointL)
        {
            try
            {
                double zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(zAtL);
            }
            catch (Exception e)
            {
                if (e is MechanismSurfaceLineException || e is InvalidOperationException || e is ArgumentException)
                {
                    return double.NaN;
                }

                throw;
            }
        }

        private static double GetThicknessCoverageLayers(PipingSoilProfile soilProfile, PipingSurfaceLine surfaceLine, RoundedDouble exitPointL)
        {
            try
            {
                double zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetConsecutiveCoverageLayerThicknessBelowLevel(zAtL);
            }
            catch (Exception e)
            {
                if (e is MechanismSurfaceLineException || e is InvalidOperationException || e is ArgumentException)
                {
                    return double.NaN;
                }

                throw;
            }
        }
    }
}