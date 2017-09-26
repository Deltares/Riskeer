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

using System;
using System.Linq;
using Deltares.WTIStability;
using Deltares.WTIStability.Calculation.Wrapper;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.IO;
using Deltares.WTIStability.Levenberg;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing an Uplift Van calculation.
    /// </summary>
    public class UpliftVanKernelWrapper : IUpliftVanKernel
    {
        private readonly WTIStabilityCalculation wtiStabilityCalculation;
        private readonly StabilityModel stabilityModel;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanKernelWrapper"/>.
        /// </summary>
        public UpliftVanKernelWrapper()
        {
            wtiStabilityCalculation = new WTIStabilityCalculation();
            stabilityModel = new StabilityModel
            {
                ModelOption = ModelOptions.UpliftVan,
                SearchAlgorithm = SearchAlgorithm.Grid,
                GridOrientation = GridOrientation.Inwards,
                SlipPlaneConstraints = new SlipPlaneConstraints(),
                GeneticAlgorithmOptions = new GeneticAlgorithmOptions
                {
                    EliteCount = 2,
                    PopulationCount = 60,
                    GenerationCount = 50,
                    MutationRate = 0.3,
                    CrossOverScatterFraction = 0,
                    CrossOverSinglePointFraction = 0.7,
                    CrossOverDoublePointFraction = 0.3,
                    MutationJumpFraction = 0,
                    MutationCreepFraction = 0.9,
                    MutationInverseFraction = 0.1,
                    MutationCreepReduction = 0.05,
                    Seed = 1
                }
            };
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

        public StabilityLocation Location
        {
            set
            {
                stabilityModel.Location = value;
            }
        }

        public SurfaceLine2 SurfaceLine
        {
            set
            {
                stabilityModel.SurfaceLine2 = value;
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
                stabilityModel.SlipCircle = new SlipCircle
                {
                    Auto = value
                };
            }
        }

        public bool CreateZones
        {
            set
            {
                stabilityModel.SlipPlaneConstraints.CreateZones = value;
            }
        }

        public bool AutomaticForbiddenZones
        {
            set
            {
                stabilityModel.SlipPlaneConstraints.AutomaticForbiddenZones = value;
            }
        }

        public double SlipPlaneMinimumDepth
        {
            set
            {
                stabilityModel.SlipPlaneConstraints.SlipPlaneMinDepth = value;
            }
        }

        public double SlipPlaneMinimumLength
        {
            set
            {
                stabilityModel.SlipPlaneConstraints.SlipPlaneMinLength = value;
            }
        }

        public double FactorOfStability { get; private set; }

        public double ZValue { get; private set; }

        public double ForbiddenZonesXEntryMin { get; private set; }

        public double ForbiddenZonesXEntryMax { get; private set; }

        public bool ForbiddenZonesAutomaticallyCalculated { get; private set; }

        public bool GridAutomaticallyCalculated { get; private set; }

        public SlidingDualCircle SlidingCurveResult { get; private set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; private set; }

        public void Calculate()
        {
            wtiStabilityCalculation.InitializeForDeterministic(WTISerializer.Serialize(stabilityModel));

            string messages = wtiStabilityCalculation.Validate();
            string result = wtiStabilityCalculation.Run();

            ReadResult(result);
        }

        private void ReadResult(string result)
        {
            StabilityAssessmentCalculationResult convertedResult = WTIDeserializer.DeserializeResult(result);

            if (convertedResult.Messages.Any())
            {
                throw new Exception();
            }

            FactorOfStability = convertedResult.FactorOfSafety;
            ZValue = convertedResult.ZValue;

            ForbiddenZonesXEntryMin = convertedResult.XMinEntry;
            ForbiddenZonesXEntryMax = convertedResult.XMaxEntry;
            ForbiddenZonesAutomaticallyCalculated = convertedResult.AreForbiddenZonesAuto;
            GridAutomaticallyCalculated = convertedResult.IsGridAuto;

            SlidingCurveResult = (SlidingDualCircle) convertedResult.Curve;
            SlipPlaneResult = convertedResult.SlipPlaneUpliftVan;
        }
    }
}