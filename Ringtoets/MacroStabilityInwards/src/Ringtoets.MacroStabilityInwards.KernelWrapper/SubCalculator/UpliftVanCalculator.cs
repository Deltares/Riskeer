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
using Deltares.WTIStability;
using Deltares.WTIStability.Calculation.Wrapper;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.IO;
using Deltares.WTIStability.Levenberg;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="WTIStabilityCalculation"/>.
    /// </summary>
    public class UpliftVanCalculator : IUpliftVanCalculator
    {
        private readonly WTIStabilityCalculation wrappedCalculator;
        private readonly StabilityModel calculatorInput;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanCalculator"/>.
        /// </summary>
        public UpliftVanCalculator()
        {
            wrappedCalculator = new WTIStabilityCalculation();
            calculatorInput = new StabilityModel
            {
                ModelOption = ModelOptions.UpliftVan,
                SearchAlgorithm = SearchAlgorithm.Grid,
                GridOrientation = GridOrientation.Inwards,
                SlipPlaneConstraints = new SlipPlaneConstraints
                {
                    AutomaticForbiddenZones = true,
                    CreateZones = true
                },
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
                calculatorInput.SoilModel = value;
            }
        }

        public SoilProfile2D SoilProfile
        {
            set
            {
                calculatorInput.SoilProfile = value;
            }
        }

        public StabilityLocation Location
        {
            set
            {
                calculatorInput.Location = value;
            }
        }

        public SurfaceLine2 SurfaceLine
        {
            set
            {
                calculatorInput.SurfaceLine2 = value;
            }
        }

        public bool MoveGrid
        {
            set
            {
                calculatorInput.MoveGrid = value;
            }
        }

        public double MaximumSliceWidth
        {
            set
            {
                calculatorInput.MaximumSliceWidth = value;
            }
        }

        public SlipPlaneUpliftVan SlipPlaneUpliftVan
        {
            set
            {
                calculatorInput.SlipPlaneUpliftVan = value;
            }
        }

        public bool GridAutomaticDetermined
        {
            set
            {
                calculatorInput.SlipCircle = new SlipCircle
                {
                    Auto = value
                };
            }
        }

        public void Calculate()
        {
            try
            {
                wrappedCalculator.InitializeForDeterministic(WTISerializer.Serialize(calculatorInput));

                string messages = wrappedCalculator.Validate();
                string result = wrappedCalculator.Run();
            }
            catch (Exception e)
            {
                // Do nothing
            }
        }
    }
}