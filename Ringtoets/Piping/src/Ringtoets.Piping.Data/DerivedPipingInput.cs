﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.InputParameterCalculation;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.Exceptions;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class responsible for calculating the derived piping input.
    /// </summary>
    public class DerivedPipingInput
    {
        private const double seepageLengthStandardDeviationFraction = 0.1;

        private readonly PipingInput input;

        /// <summary>
        /// Creates a new instance of <see cref="DerivedPipingInput"/>.
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        public DerivedPipingInput(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Cannot create DerivedPipingInput without PipingInput.");
            }
            this.input = input;
        }

        /// <summary>
        /// Gets the assessment level from the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return input.HydraulicBoundaryLocation == null ?
                           new RoundedDouble(2, double.NaN) :
                           new RoundedDouble(2, input.HydraulicBoundaryLocation.DesignWaterLevel);
            }
        }

        /// <summary>
        /// Gets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        public RoundedDouble PiezometricHeadExit
        {
            get
            {
                var dampingFactorExit = PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue();
                var phreaticLevelExit = PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue();

                return new RoundedDouble(2, InputParameterCalculationService.CalculatePiezometricHeadAtExit(AssessmentLevel, dampingFactorExit, phreaticLevelExit));
            }
        }

        /// <summary>
        /// Gets the horizontal distance between entry and exit point.
        /// [m]
        /// </summary>
        public LogNormalDistribution SeepageLength
        {
            get
            {
                LogNormalDistribution seepageLength = new LogNormalDistribution(2);
                double seepageLengthMean = input.ExitPointL - input.EntryPointL;

                seepageLength.Mean = (RoundedDouble) seepageLengthMean;
                seepageLength.StandardDeviation = (RoundedDouble) seepageLengthMean*seepageLengthStandardDeviationFraction;

                return seepageLength;
            }
        }

        /// <summary>
        /// Gets the total thickness of the coverage layers at the exit point.
        /// [m]
        /// </summary>
        public LogNormalDistribution ThicknessCoverageLayer
        {
            get
            {
                LogNormalDistribution thicknessCoverageLayer = new LogNormalDistribution(2)
                {
                    Mean = (RoundedDouble) double.NaN,
                    StandardDeviation = (RoundedDouble) 0.5
                };
                UpdateThicknessCoverageLayerMean(thicknessCoverageLayer);

                return thicknessCoverageLayer;
            }
        }

        /// <summary>
        /// Gets the total thickness of the aquifer layers at the exit point.
        /// [m]
        /// </summary>
        public LogNormalDistribution ThicknessAquiferLayer
        {
            get
            {
                LogNormalDistribution thicknessAquiferLayer = new LogNormalDistribution(2)
                {
                    Mean = (RoundedDouble) double.NaN,
                    StandardDeviation = (RoundedDouble) 0.5
                };
                UpdateThicknessAquiferLayerMean(thicknessAquiferLayer);

                return thicknessAquiferLayer;
            }
        }

        /// <summary>
        /// Gets the sieve size through which 70% of the grains of the top part of the aquifer pass.
        /// [m]
        /// </summary>
        public LogNormalDistribution DiameterD70
        {
            get
            {
                var distribution = new LogNormalDistribution(6)
                {
                    Mean = (RoundedDouble) double.NaN,
                    StandardDeviation = (RoundedDouble) double.NaN
                };
                UpdateDiameterD70Parameters(distribution);

                return distribution;
            }
        }

        /// <summary>
        /// Gets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public LogNormalDistribution DarcyPermeability
        {
            get
            {
                var distribution = new LogNormalDistribution(6)
                {
                    Mean = (RoundedDouble) double.NaN,
                    StandardDeviation = (RoundedDouble) double.NaN
                };
                UpdateDarcyPermeabilityParameters(distribution);

                return distribution;
            }
        }

        /// <summary>
        /// Gets the volumic weight of the saturated coverage layer.
        /// </summary>
        public LogNormalDistribution SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                var distribution = new LogNormalDistribution(2)
                {
                    Mean = (RoundedDouble) double.NaN,
                    StandardDeviation = (RoundedDouble) double.NaN,
                    Shift = (RoundedDouble) double.NaN
                };
                UpdateSaturatedVolumicWeightOfCoverageLayerParameters(distribution);

                return distribution;
            }
        }

        private void UpdateThicknessAquiferLayerMean(LogNormalDistribution thicknessAquiferLayer)
        {
            StochasticSoilProfile stochasticSoilProfile = input.StochasticSoilProfile;
            RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
            RoundedDouble exitPointL = input.ExitPointL;

            if (stochasticSoilProfile != null && stochasticSoilProfile.SoilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
            {
                var thicknessTopAquiferLayer = new RoundedDouble(thicknessAquiferLayer.Mean.NumberOfDecimalPlaces,
                                                                 GetThicknessTopAquiferLayer(stochasticSoilProfile.SoilProfile, surfaceLine, exitPointL));

                if (thicknessTopAquiferLayer > 0)
                {
                    thicknessAquiferLayer.Mean = thicknessTopAquiferLayer;
                }
            }
        }

        private void UpdateThicknessCoverageLayerMean(LogNormalDistribution thicknessCoverageLayerDistribution)
        {
            if (input.SurfaceLine != null && input.StochasticSoilProfile != null && input.StochasticSoilProfile.SoilProfile != null && !double.IsNaN(input.ExitPointL))
            {
                var weightedMean = new RoundedDouble(thicknessCoverageLayerDistribution.Mean.NumberOfDecimalPlaces,
                                                     InputParameterCalculationService.CalculateThicknessCoverageLayer(
                                                         input.WaterVolumetricWeight,
                                                         PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                                                         input.ExitPointL,
                                                         input.SurfaceLine,
                                                         input.StochasticSoilProfile.SoilProfile));

                if (weightedMean > 0)
                {
                    thicknessCoverageLayerDistribution.Mean = weightedMean;
                }
            }
        }

        private void UpdateDiameterD70Parameters(LogNormalDistribution diameterD70Distribution)
        {
            PipingSoilLayer topMostAquiferLayer = GetConsecutiveAquiferLayers().FirstOrDefault();
            if (topMostAquiferLayer != null)
            {
                var diameterD70Mean = new RoundedDouble(diameterD70Distribution.Mean.NumberOfDecimalPlaces, topMostAquiferLayer.DiameterD70Mean);

                if (diameterD70Mean > 0)
                {
                    diameterD70Distribution.Mean = diameterD70Mean;
                }
                diameterD70Distribution.StandardDeviation = (RoundedDouble) topMostAquiferLayer.DiameterD70Deviation;
            }
        }

        private void UpdateDarcyPermeabilityParameters(LogNormalDistribution darcyPermeabilityDistribution)
        {
            PipingSoilLayer topMostAquiferLayer = GetConsecutiveAquiferLayers().FirstOrDefault();
            if (topMostAquiferLayer != null)
            {
                var darcyPermeabilityMean = new RoundedDouble(darcyPermeabilityDistribution.Mean.NumberOfDecimalPlaces, topMostAquiferLayer.PermeabilityMean);

                if (darcyPermeabilityMean > 0)
                {
                    darcyPermeabilityDistribution.Mean = darcyPermeabilityMean;
                }
                darcyPermeabilityDistribution.StandardDeviation = (RoundedDouble) topMostAquiferLayer.PermeabilityDeviation;
            }
        }

        private void UpdateSaturatedVolumicWeightOfCoverageLayerParameters(LogNormalDistribution volumicWeightDistribution)
        {
            PipingSoilLayer[] coverageLayers = GetConsecutiveCoverageLayers();

            var numberOfDecimals = GetNumberOfDecimals(volumicWeightDistribution);

            if (HasCorrectSaturatedWeightDistributionParameterDefinition(
                coverageLayers,
                numberOfDecimals))
            {
                PipingSoilLayer topMostAquitardLayer = coverageLayers.First();
                volumicWeightDistribution.Shift = (RoundedDouble) topMostAquitardLayer.BelowPhreaticLevelShift;
                volumicWeightDistribution.StandardDeviation = (RoundedDouble) topMostAquitardLayer.BelowPhreaticLevelDeviation;

                var weightedMean = new RoundedDouble(volumicWeightDistribution.Mean.NumberOfDecimalPlaces,
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

        private int GetNumberOfDecimals(LogNormalDistribution volumicWeightDistribution)
        {
            return volumicWeightDistribution.Mean.NumberOfDecimalPlaces;
        }

        private static bool HasCorrectSaturatedWeightDistributionParameterDefinition(IList<PipingSoilLayer> consecutiveAquitardLayers, int numberOfDecimals)
        {
            if (!consecutiveAquitardLayers.Any())
            {
                return false;
            }

            var distributions = GetLayerDistributionDefinitions(consecutiveAquitardLayers, numberOfDecimals);

            if (distributions == null)
            {
                return false;
            }

            if (distributions.Length == 1)
            {
                return true;
            }

            return distributions.All(currentLayerDistribution => AreShiftAndDeviationEqual(
                currentLayerDistribution,
                distributions[0]));
        }

        private static LogNormalDistribution[] GetLayerDistributionDefinitions(IList<PipingSoilLayer> consecutiveAquitardLayers, int numberOfDecimals)
        {
            try
            {
                return consecutiveAquitardLayers.Select(layer => new LogNormalDistribution(numberOfDecimals)
                {
                    Mean = (RoundedDouble) layer.BelowPhreaticLevelMean,
                    StandardDeviation = (RoundedDouble) layer.BelowPhreaticLevelDeviation,
                    Shift = (RoundedDouble) layer.BelowPhreaticLevelShift
                }).ToArray();
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private static bool AreShiftAndDeviationEqual(LogNormalDistribution currentLayerDistribution, LogNormalDistribution baseLayerDistribution)
        {
            return currentLayerDistribution.StandardDeviation == baseLayerDistribution.StandardDeviation &&
                   currentLayerDistribution.Shift == baseLayerDistribution.Shift;
        }

        private static double GetWeightedMeanForVolumicWeightOfCoverageLayer(PipingSoilLayer[] aquitardLayers, PipingSoilProfile profile, double surfaceLevel)
        {
            double totalThickness = 0.0;
            double weighedTotal = 0.0;

            foreach (var layer in aquitardLayers)
            {
                double layerThickness = profile.GetLayerThickness(layer);
                double bottom = layer.Top - layerThickness;
                double thicknessUnderSurface = Math.Min(layer.Top, surfaceLevel) - bottom;

                totalThickness += thicknessUnderSurface;
                weighedTotal += layer.BelowPhreaticLevelMean*thicknessUnderSurface;
            }

            return weighedTotal/totalThickness;
        }

        private PipingSoilLayer[] GetConsecutiveAquiferLayers()
        {
            RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
            PipingSoilProfile soilProfile = input.StochasticSoilProfile != null ? input.StochasticSoilProfile.SoilProfile : null;
            RoundedDouble exitPointL = input.ExitPointL;

            if (surfaceLine != null && soilProfile != null && !double.IsNaN(exitPointL))
            {
                return soilProfile.GetConsecutiveAquiferLayersBelowLevel(surfaceLine.GetZAtL(exitPointL)).ToArray();
            }

            return new PipingSoilLayer[0];
        }

        private PipingSoilLayer[] GetConsecutiveCoverageLayers()
        {
            RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
            PipingSoilProfile soilProfile = input.StochasticSoilProfile != null ? input.StochasticSoilProfile.SoilProfile : null;
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

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, RoundedDouble exitPointL)
        {
            try
            {
                double zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(zAtL);
            }
            catch (Exception e)
            {
                if (e is RingtoetsPipingSurfaceLineException || e is InvalidOperationException || e is ArgumentException)
                {
                    return double.NaN;
                }
                throw;
            }
        }
    }
}