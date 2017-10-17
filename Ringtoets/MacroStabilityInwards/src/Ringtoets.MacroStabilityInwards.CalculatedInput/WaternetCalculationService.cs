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
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.CalculatedInput
{
    /// <summary>
    /// This class can be used to calculate Waternet for a macro stability inwards
    /// calculation based on other input parameters.
    /// </summary>
    public static class WaternetCalculationService
    {
        /// <summary>
        /// Calculated the Waternet with extreme circumstances based on the values 
        /// of the <see cref="IMacroStabilityInwardsWaternetInput"/>.
        /// </summary>
        /// <param name="input">The input to get the values from.</param>
        /// <returns>A calculated <see cref="MacroStabilityInwardsWaternet"/>,
        /// or <c>null</c> when the Waternet could be calculated.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsWaternet CalculateExtreme(IMacroStabilityInwardsWaternetInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IWaternetCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance
                                                                                   .CreateWaternetExtremeCalculator(
                                                                                       CreateCalculatorInput(input, false),
                                                                                       MacroStabilityInwardsKernelWrapperFactory.Instance);

            try
            {
                WaternetCalculatorResult result = calculator.Calculate();
                return MacroStabilityInwardsWaternetConverter.Convert(result);
            }
            catch (WaternetCalculatorException)
            {
                return new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0],
                                                         new MacroStabilityInwardsWaternetLine[0]);
            }
        }

        /// <summary>
        /// Calculated the Waternet with daily circumstances based on the values 
        /// of the <see cref="IMacroStabilityInwardsWaternetInput"/>.
        /// </summary>
        /// <param name="input">The input to get the values from.</param>
        /// <returns>A calculated <see cref="MacroStabilityInwardsWaternet"/>,
        /// or <c>null</c> when the Waternet could be calculated.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsWaternet CalculateDaily(IMacroStabilityInwardsWaternetInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IWaternetCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance
                                                                                   .CreateWaternetDailyCalculator(
                                                                                       CreateCalculatorInput(input, true),
                                                                                       MacroStabilityInwardsKernelWrapperFactory.Instance);

            try
            {
                WaternetCalculatorResult result = calculator.Calculate();
                return MacroStabilityInwardsWaternetConverter.Convert(result);
            }
            catch (WaternetCalculatorException)
            {
                return new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0],
                                                         new MacroStabilityInwardsWaternetLine[0]);
            }
        }

        private static WaternetCalculatorInput CreateCalculatorInput(IMacroStabilityInwardsWaternetInput input, bool daily)
        {
            return new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                PlLineCreationMethod = PlLineCreationMethod.RingtoetsWti2017,
                AssessmentLevel = input.AssessmentLevel,
                LandwardDirection = LandwardDirection.PositiveX,
                SurfaceLine = input.SurfaceLine,
                SoilProfile = SoilProfileConverter.Convert(input.SoilProfileUnderSurfaceLine),
                DrainageConstruction = DrainageConstructionConverter.Convert(input),
                PhreaticLineOffsets = daily
                                          ? PhreaticLineOffsetsConverter.Convert(input.LocationInputDaily)
                                          : PhreaticLineOffsetsConverter.Convert(input.LocationInputExtreme),
                DikeSoilScenario = input.DikeSoilScenario,
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                WaterLevelPolder = daily
                                       ? input.LocationInputDaily.WaterLevelPolder
                                       : input.LocationInputExtreme.WaterLevelPolder,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                LeakageLengthOutwardsPhreaticLine3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPhreaticLine3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPhreaticLine4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPhreaticLine4 = input.LeakageLengthInwardsPhreaticLine4,
                PiezometricHeadPhreaticLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                PiezometricHeadPhreaticLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                PenetrationLength = daily
                                        ? input.LocationInputDaily.PenetrationLength
                                        : input.LocationInputExtreme.PenetrationLength,
                AdjustPhreaticLine3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift
            });
        }
    }
}