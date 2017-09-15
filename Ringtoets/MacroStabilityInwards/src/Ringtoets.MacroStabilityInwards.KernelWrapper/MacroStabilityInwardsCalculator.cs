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
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// This class represents a combination of macro stability inwards sub calculations, which together can be used
    /// to assess based on macro stability inwards.
    /// </summary>
    public class MacroStabilityInwardsCalculator
    {
        private readonly MacroStabilityInwardsCalculatorInput input;
        private readonly IMacroStabilityInwardsSubCalculatorFactory factory;

        /// <summary>
        /// Constructs a new <see cref="MacroStabilityInwardsCalculator"/>. The <paramref name="input"/> is used to
        /// obtain the parameters used in the different sub calculations.
        /// </summary>
        /// <param name="input">The <see cref="MacroStabilityInwardsCalculatorInput"/> containing all the values required
        /// for performing a macro stability inwards calculation.</param>
        /// <param name="factory">The factory responsible for creating the sub calculators.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> or <paramref name="factory"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculator(MacroStabilityInwardsCalculatorInput input, IMacroStabilityInwardsSubCalculatorFactory factory)
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

        /// <summary>
        /// Performs the actual sub calculations and returns a <see cref="MacroStabilityInwardsCalculatorResult"/>, which
        /// contains the results of all sub calculations.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculatorResult"/> containing the results of the sub calculations.</returns>
        public MacroStabilityInwardsCalculatorResult Calculate()
        {
            IUpliftVanCalculator upliftVanCalculator = CalculateUpliftVan();

            return new MacroStabilityInwardsCalculatorResult(new MacroStabilityInwardsCalculatorResult.ConstructionProperties
            {
                FactorOfStability = upliftVanCalculator.FactoryOfStability,
                ZValue = upliftVanCalculator.ZValue,
                ForbiddenZonesXEntryMin = upliftVanCalculator.ForbiddenZonesXEntryMin,
                ForbiddenZonesXEntryMax = upliftVanCalculator.ForbiddenZonesXEntryMax,
                ForbiddenZonesAutomaticallyCalculated = upliftVanCalculator.ForbiddenZonesAutomaticallyCalculated
            });
        }

        /// <summary>
        /// Returns a list of validation messages. The validation messages are based on the values of the <see cref="MacroStabilityInwardsCalculatorInput"/>
        /// which was provided to this <see cref="MacroStabilityInwardsCalculator"/> and are determined by the kernel.
        /// </summary>
        public List<string> Validate()
        {
            return new List<string>();
        }

        private IUpliftVanCalculator CalculateUpliftVan()
        {
            IUpliftVanCalculator upliftVanCalculator = CreateUpliftVanCalculator();

            try
            {
                upliftVanCalculator.Calculate();
            }
            catch (Exception e)
            {
                // Temporary do nothing
            }

            return upliftVanCalculator;
        }

        private IUpliftVanCalculator CreateUpliftVanCalculator()
        {
            IUpliftVanCalculator calculator = factory.CreateUpliftVanCalculator();
            calculator.MoveGrid = input.MoveGrid;
            calculator.MaximumSliceWidth = input.MaximumSliceWidth;

            Soil[] soils = SoilCreator.Create(input.SoilProfile);
            calculator.SoilModel = SoilModelCreator.Create(soils);

            Dictionary<MacroStabilityInwardsSoilLayerUnderSurfaceLine, Soil> layersWithSoils =
                input.SoilProfile.LayersUnderSurfaceLine
                     .Zip(soils, (layer, soil) => new
                     {
                         layer, soil
                     })
                     .ToDictionary(x => x.layer, x => x.soil);

            calculator.SoilProfile = SoilProfileCreator.Create(input.SoilProfile, layersWithSoils);
            calculator.Location = StabilityLocationCreator.Create(input);
            calculator.SurfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
            calculator.SlipPlaneUpliftVan = SlipPlaneUpliftVanCreator.Create(input);
            calculator.GridAutomaticDetermined = input.GridAutomaticDetermined;
            calculator.CreateZones = input.CreateZones;
            calculator.AutomaticForbidenZones = input.AutomaticForbiddenZones;
            calculator.SlipPlaneMinimumDepth = input.SlipPlaneMinimumDepth;
            calculator.SlipPlaneMinimumLength = input.SlipPlaneMinimumLength;
            return calculator;
        }
    }
}