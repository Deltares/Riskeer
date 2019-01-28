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
using Deltares.WTIStability;
using Deltares.WTIStability.Calculation.Wrapper;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.Data.Standard;
using Deltares.WTIStability.IO;
using WtiStabilityWaternet = Deltares.WTIStability.Data.Geo.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing an Uplift Van calculation.
    /// </summary>
    internal class UpliftVanKernelWrapper : IUpliftVanKernel
    {
        private readonly StabilityModel stabilityModel;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanKernelWrapper"/>.
        /// </summary>
        public UpliftVanKernelWrapper()
        {
            stabilityModel = new StabilityModel
            {
                GridOrientation = GridOrientation.Inwards,
                SlipCircle = new SlipCircle(),
                SearchAlgorithm = SearchAlgorithm.Grid,
                ModelOption = ModelOptions.UpliftVan,
                GeotechnicsData =
                {
                    CurrentWaternetDaily = new WtiStabilityWaternet
                    {
                        Name = "WaternetDaily"
                    }
                },
                MultiplicationFactorsCPhiForUpliftList =
                {
                    new MultiplicationFactorOnCPhiForUplift
                    {
                        UpliftFactor = 1.2,
                        MultiplicationFactor = 0.0
                    }
                }
            };

            FactorOfStability = double.NaN;
            ZValue = double.NaN;
            ForbiddenZonesXEntryMin = double.NaN;
            ForbiddenZonesXEntryMax = double.NaN;
        }

        public SoilModel SoilModel
        {
            set
            {
                stabilityModel.SoilModel = value;
            }
        }

        public SoilProfile2D SoilProfile
        {
            set
            {
                stabilityModel.SoilProfile = value;
            }
        }

        public StabilityLocation LocationExtreme
        {
            set
            {
                stabilityModel.Location = value;
            }
        }

        public StabilityLocation LocationDaily
        {
            set
            {
                stabilityModel.LocationDaily = value;
            }
        }

        public SurfaceLine2 SurfaceLine
        {
            set
            {
                stabilityModel.SurfaceLine2 = value;
            }
        }

        public SlipPlaneConstraints SlipPlaneConstraints
        {
            set
            {
                stabilityModel.SlipPlaneConstraints = value;
            }
        }

        public bool MoveGrid
        {
            set
            {
                stabilityModel.MoveGrid = value;
            }
        }

        public double MaximumSliceWidth
        {
            set
            {
                stabilityModel.MaximumSliceWidth = value;
            }
        }

        public SlipPlaneUpliftVan SlipPlaneUpliftVan
        {
            set
            {
                stabilityModel.SlipPlaneUpliftVan = value;
            }
        }

        public bool GridAutomaticDetermined
        {
            set
            {
                stabilityModel.SlipCircle.Auto = value;
            }
        }

        public double FactorOfStability { get; private set; }

        public double ZValue { get; private set; }

        public double ForbiddenZonesXEntryMin { get; private set; }

        public double ForbiddenZonesXEntryMax { get; private set; }

        public SlidingDualCircle SlidingCurveResult { get; private set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; private set; }

        public IEnumerable<LogMessage> CalculationMessages { get; private set; }

        public void Calculate()
        {
            try
            {
                var wtiStabilityCalculation = new WTIStabilityCalculation();
                wtiStabilityCalculation.InitializeForDeterministic(WTISerializer.Serialize(stabilityModel));

                string result = wtiStabilityCalculation.Run();

                ReadResult(result);
            }
            catch (Exception e) when (!(e is UpliftVanKernelWrapperException))
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            try
            {
                var wtiStabilityCalculation = new WTIStabilityCalculation();
                wtiStabilityCalculation.InitializeForDeterministic(WTISerializer.Serialize(stabilityModel));

                string result = wtiStabilityCalculation.Validate();

                return WTIDeserializer.DeserializeValidation(result);
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        /// <summary>
        /// Reads the calculation result.
        /// </summary>
        /// <param name="result">The result to read.</param>
        /// <exception cref="UpliftVanKernelWrapperException">Thrown when the
        /// calculation result contains error messages.</exception>
        private void ReadResult(string result)
        {
            StabilityAssessmentCalculationResult convertedResult = WTIDeserializer.DeserializeResult(result);

            FactorOfStability = convertedResult.FactorOfSafety;
            ZValue = convertedResult.ZValue;
            ForbiddenZonesXEntryMin = convertedResult.XMinEntry;
            ForbiddenZonesXEntryMax = convertedResult.XMaxEntry;

            SlidingCurveResult = (SlidingDualCircle) convertedResult.Curve;
            SlipPlaneResult = convertedResult.SlipPlaneUpliftVan;

            CalculationMessages = convertedResult.Messages ?? Enumerable.Empty<LogMessage>();
        }
    }
}