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
using log4net;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(DerivedPipingInput));

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
                RoundedDouble piezometricHeadExit;
                try
                {
                    var assessmentLevel = input.AssessmentLevel;
                    var dampingFactorExit = PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue();
                    var phreaticLevelExit = PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue();

                    piezometricHeadExit = (RoundedDouble) InputParameterCalculationService.CalculatePiezometricHeadAtExit(assessmentLevel, dampingFactorExit, phreaticLevelExit);
                }
                catch (ArgumentOutOfRangeException)
                {
                    piezometricHeadExit = (RoundedDouble) double.NaN;
                }

                return new RoundedDouble(2, piezometricHeadExit);
            }
        }

        /// <summary>
        /// Gets the seepage length.
        /// </summary>
        public LognormalDistribution SeepageLength
        {
            get
            {
                LognormalDistribution seepageLenght = new LognormalDistribution(2)
                {
                    Mean = (RoundedDouble) double.NaN,
                    StandardDeviation = (RoundedDouble) double.NaN
                };
                try
                {
                    seepageLenght.Mean = input.ExitPointL - input.EntryPointL;
                }
                catch (ArgumentOutOfRangeException)
                {
                    seepageLenght.Mean = (RoundedDouble) double.NaN;
                }
                seepageLenght.StandardDeviation = seepageLenght.Mean * seepageLengthStandardDeviationFraction;

                return seepageLenght;
            }
        }

        /// <summary>
        /// Gets the thickness coverage layer.
        /// </summary>
        public LognormalDistribution ThicknessCoverageLayer
        {
            get
            {
                LognormalDistribution thicknessCoverageLayer = new LognormalDistribution(2)
                {
                    Mean = (RoundedDouble)double.NaN,
                    StandardDeviation = (RoundedDouble) 0.5
                };
                if (input.SurfaceLine != null && input.SoilProfile != null & !double.IsNaN(input.ExitPointL))
                {
                    TrySetThicknessCoverageLayer(thicknessCoverageLayer);

                    if (double.IsNaN(thicknessCoverageLayer.Mean))
                    {
                        log.Warn(Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
                    }
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
        public LognormalDistribution ThicknessAquiferLayer
        {
            get
            {
                LognormalDistribution thicknesAquiferLayer = new LognormalDistribution(2)
                {
                    Mean = (RoundedDouble)double.NaN,
                    StandardDeviation = (RoundedDouble) 0.5
                };

                PipingSoilProfile soilProfile = input.SoilProfile;
                RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
                double exitPointL = input.ExitPointL;

                if (soilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
                {
                    double thicknessTopAquiferLayer = GetThicknessTopAquiferLayer(soilProfile, surfaceLine, exitPointL);
                    TrySetThicknessAquiferLayerMean(thicknesAquiferLayer, thicknessTopAquiferLayer);

                    if (double.IsNaN(thicknesAquiferLayer.Mean))
                    {
                        log.Warn(Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer);
                    }
                }
                else
                {
                    thicknesAquiferLayer.Mean = (RoundedDouble)double.NaN;
                }

                return thicknesAquiferLayer;
            }
        }

        private static void TrySetThicknessAquiferLayerMean(LognormalDistribution thicknesAquiferLayer, double thicknessTopAquiferLayer)
        {
            try
            {
                thicknesAquiferLayer.Mean = (RoundedDouble)thicknessTopAquiferLayer;
            }
            catch (ArgumentOutOfRangeException)
            {
                thicknesAquiferLayer.Mean = (RoundedDouble)double.NaN;
            }
        }

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, double exitPointL)
        {
            try
            {
                var zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetTopAquiferLayerThicknessBelowLevel(zAtL);
            }
            catch (ArgumentOutOfRangeException)
            {
                return double.NaN;
            }
            catch (ArgumentException)
            {
                return double.NaN;
            }
        }

        private void TrySetThicknessCoverageLayer(LognormalDistribution thicknessCoverageLayer)
        {
            try
            {
                thicknessCoverageLayer.Mean = (RoundedDouble)InputParameterCalculationService.CalculateThicknessCoverageLayer(input.WaterVolumetricWeight, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), input.ExitPointL, input.SurfaceLine, input.SoilProfile);
            }
            catch (ArgumentOutOfRangeException)
            {
                thicknessCoverageLayer.Mean = (RoundedDouble)double.NaN;
            }
        }
    }
}