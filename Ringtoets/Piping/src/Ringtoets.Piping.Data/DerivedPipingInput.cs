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

using System;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.InputParameterCalculation;
using Ringtoets.Piping.Primitives;

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
        /// Gets the piezometric head exit.
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
        /// Gets the seepage length.
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
        /// Gets the thickness coverage layer.
        /// </summary>
        public LogNormalDistribution ThicknessCoverageLayer
        {
            get
            {
                LogNormalDistribution thicknessCoverageLayer = new LogNormalDistribution(2)
                {
                    StandardDeviation = (RoundedDouble) 0.5
                };
                if (input.SurfaceLine != null && input.StochasticSoilProfile != null && input.StochasticSoilProfile.SoilProfile != null & !double.IsNaN(input.ExitPointL))
                {
                    TrySetThicknessCoverageLayer(thicknessCoverageLayer);
                }
                else
                {
                    thicknessCoverageLayer.Mean = (RoundedDouble) double.NaN;
                }

                return thicknessCoverageLayer;
            }
        }

        /// <summary>
        /// gets the thickness aquifer layer.
        /// </summary>
        public LogNormalDistribution ThicknessAquiferLayer
        {
            get
            {
                LogNormalDistribution thicknessAquiferLayer = new LogNormalDistribution(2)
                {
                    StandardDeviation = (RoundedDouble) 0.5
                };

                StochasticSoilProfile stochasticSoilProfile = input.StochasticSoilProfile;

                RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
                RoundedDouble exitPointL = input.ExitPointL;

                if (stochasticSoilProfile != null && stochasticSoilProfile.SoilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
                {
                    double thicknessTopAquiferLayer = GetThicknessTopAquiferLayer(stochasticSoilProfile.SoilProfile, surfaceLine, exitPointL);
                    TrySetThicknessAquiferLayerMean(thicknessAquiferLayer, thicknessTopAquiferLayer);
                }
                else
                {
                    thicknessAquiferLayer.Mean = (RoundedDouble) double.NaN;
                }

                return thicknessAquiferLayer;
            }
        }

        private static void TrySetThicknessAquiferLayerMean(LogNormalDistribution thicknessAquiferLayer, double thicknessTopAquiferLayer)
        {
            if (thicknessTopAquiferLayer > 0)
            {
                thicknessAquiferLayer.Mean = (RoundedDouble) thicknessTopAquiferLayer;
            }
            else
            {
                thicknessAquiferLayer.Mean = (RoundedDouble) double.NaN;
            }
        }

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, RoundedDouble exitPointL)
        {
            try
            {
                var zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(zAtL);
            }
            catch (ArgumentException)
            {
                return double.NaN;
            }
        }

        private void TrySetThicknessCoverageLayer(LogNormalDistribution thicknessCoverageLayer)
        {
            try
            {
                thicknessCoverageLayer.Mean = (RoundedDouble) InputParameterCalculationService.CalculateThicknessCoverageLayer(
                    input.WaterVolumetricWeight,
                    PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                    input.ExitPointL,
                    input.SurfaceLine,
                    input.StochasticSoilProfile.SoilProfile);
            }
            catch (ArgumentOutOfRangeException)
            {
                thicknessCoverageLayer.Mean = (RoundedDouble) double.NaN;
            }
        }
    }
}