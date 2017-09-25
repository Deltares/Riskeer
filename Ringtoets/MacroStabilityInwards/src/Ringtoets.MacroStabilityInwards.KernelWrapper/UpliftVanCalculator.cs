﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Deltares.WTIStability.Data.Geo;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernel;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// Class representing an Uplift Van calculator.
    /// </summary>
    public class UpliftVanCalculator : IUpliftVanCalculator
    {
        private readonly UpliftVanCalculatorInput input;
        private readonly IMacroStabilityInwardsKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> containing all the values required
        /// for performing the Uplift Van calculation.</param>
        /// <param name="factory">The factory responsible for creating the Uplift Van kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> or <paramref name="factory"/> is <c>null</c>.</exception>
        public UpliftVanCalculator(UpliftVanCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), @"MacroStabilityInwardsCalculatorInput required for creating a MacroStabilityInwardsCalculator.");
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), @"IMacroStabilityInwardsSubCalculatorFactory required for creating a MacroStabilityInwardsCalculator.");
            }
            this.input = input;
            this.factory = factory;
        }

        public UpliftVanCalculatorResult Calculate()
        {
            IUpliftVanKernel upliftVanKernel = CalculateUpliftVan();

            return new UpliftVanCalculatorResult(
                UpliftVanSlidingCurveResultCreator.Create(upliftVanKernel.SlidingCurveResult),
                UpliftVanCalculationGridResultCreator.Create(upliftVanKernel.SlipPlaneResult),
                new UpliftVanCalculatorResult.ConstructionProperties
                {
                    FactorOfStability = upliftVanKernel.FactorOfStability,
                    ZValue = upliftVanKernel.ZValue,
                    ForbiddenZonesXEntryMin = upliftVanKernel.ForbiddenZonesXEntryMin,
                    ForbiddenZonesXEntryMax = upliftVanKernel.ForbiddenZonesXEntryMax,
                    ForbiddenZonesAutomaticallyCalculated = upliftVanKernel.ForbiddenZonesAutomaticallyCalculated,
                    GridAutomaticallyCalculated = upliftVanKernel.GridAutomaticallyCalculated
                });
        }

        public List<string> Validate()
        {
            return new List<string>();
        }

        private IUpliftVanKernel CalculateUpliftVan()
        {
            IUpliftVanKernel upliftVanKernel = CreateUpliftVanKernel();

            try
            {
                upliftVanKernel.Calculate();
            }
            catch (Exception e)
            {
                // Temporary do nothing
            }

            return upliftVanKernel;
        }

        private IUpliftVanKernel CreateUpliftVanKernel()
        {
            IUpliftVanKernel upliftVanKernel = factory.CreateUpliftVanKernel();

            upliftVanKernel.MoveGrid = input.MoveGrid;
            upliftVanKernel.MaximumSliceWidth = input.MaximumSliceWidth;

            Soil[] soils = SoilCreator.Create(input.SoilProfile);
            upliftVanKernel.SoilModel = SoilModelCreator.Create(soils);

            Dictionary<MacroStabilityInwardsSoilLayerUnderSurfaceLine, Soil> layersWithSoils =
                input.SoilProfile.Layers
                     .Zip(soils, (layer, soil) => new
                     {
                         layer,
                         soil
                     })
                     .ToDictionary(x => x.layer, x => x.soil);

            upliftVanKernel.SoilProfile = SoilProfileCreator.Create(input.SoilProfile, layersWithSoils);
            upliftVanKernel.Location = StabilityLocationCreator.Create(input);
            upliftVanKernel.SurfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
            upliftVanKernel.SlipPlaneUpliftVan = SlipPlaneUpliftVanCreator.Create(input);
            upliftVanKernel.GridAutomaticDetermined = input.GridAutomaticDetermined;
            upliftVanKernel.CreateZones = input.CreateZones;
            upliftVanKernel.AutomaticForbiddenZones = input.AutomaticForbiddenZones;
            upliftVanKernel.SlipPlaneMinimumDepth = input.SlipPlaneMinimumDepth;
            upliftVanKernel.SlipPlaneMinimumLength = input.SlipPlaneMinimumLength;

            return upliftVanKernel;
        }
    }
}