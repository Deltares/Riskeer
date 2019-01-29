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
using Core.Common.Base.Data;
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput
{
    /// <summary>
    /// This class can be used to calculate Waternet for a macro stability inwards
    /// calculation based on other input parameters.
    /// </summary>
    public static class WaternetCalculationService
    {
        /// <summary>
        /// Calculates the Waternet with extreme circumstances based on the values 
        /// of the <see cref="IMacroStabilityInwardsWaternetInput"/>.
        /// </summary>
        /// <param name="input">The input to get the values from.</param>
        /// <param name="assessmentLevel">The assessment level to use.</param>
        /// <returns>A calculated <see cref="MacroStabilityInwardsWaternet"/>,
        /// or an empty <see cref="MacroStabilityInwardsWaternet"/> when the Waternet
        /// could not be calculated.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsWaternet CalculateExtreme(IMacroStabilityInwardsWaternetInput input,
                                                                     RoundedDouble assessmentLevel)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IWaternetCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance
                                                                                   .CreateWaternetExtremeCalculator(
                                                                                       CreateExtremeCalculatorInput(input, assessmentLevel),
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
        /// Calculates the Waternet with daily circumstances based on the values 
        /// of the <see cref="IMacroStabilityInwardsWaternetInput"/>.
        /// </summary>
        /// <param name="input">The input to get the values from.</param>
        /// <returns>A calculated <see cref="MacroStabilityInwardsWaternet"/>,
        /// or an empty <see cref="MacroStabilityInwardsWaternet"/> when the Waternet
        /// could not be calculated.</returns>
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
                                                                                       CreateDailyCalculatorInput(input),
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

        private static WaternetCalculatorInput CreateDailyCalculatorInput(IMacroStabilityInwardsWaternetInput input)
        {
            WaternetCalculatorInput.ConstructionProperties properties = CreateCalculatorInputConstructionProperties(input);
            properties.PhreaticLineOffsets = PhreaticLineOffsetsConverter.Convert(input.LocationInputDaily);
            properties.AssessmentLevel = input.WaterLevelRiverAverage;
            properties.WaterLevelPolder = input.LocationInputDaily.WaterLevelPolder;
            properties.PenetrationLength = input.LocationInputDaily.PenetrationLength;

            return new WaternetCalculatorInput(properties);
        }

        private static WaternetCalculatorInput CreateExtremeCalculatorInput(IMacroStabilityInwardsWaternetInput input, RoundedDouble assessmentLevel)
        {
            WaternetCalculatorInput.ConstructionProperties properties = CreateCalculatorInputConstructionProperties(input);
            properties.PhreaticLineOffsets = PhreaticLineOffsetsConverter.Convert(input.LocationInputExtreme);
            properties.AssessmentLevel = assessmentLevel;
            properties.WaterLevelPolder = input.LocationInputExtreme.WaterLevelPolder;
            properties.PenetrationLength = input.LocationInputExtreme.PenetrationLength;

            return new WaternetCalculatorInput(properties);
        }

        private static WaternetCalculatorInput.ConstructionProperties CreateCalculatorInputConstructionProperties(IMacroStabilityInwardsWaternetInput input)
        {
            return new WaternetCalculatorInput.ConstructionProperties
            {
                WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                PlLineCreationMethod = PlLineCreationMethod.RingtoetsWti2017,
                LandwardDirection = LandwardDirection.PositiveX,
                SurfaceLine = input.SurfaceLine,
                SoilProfile = SoilProfileConverter.Convert(input.SoilProfileUnderSurfaceLine),
                DrainageConstruction = DrainageConstructionConverter.Convert(input),
                DikeSoilScenario = input.DikeSoilScenario,
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                LeakageLengthOutwardsPhreaticLine3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPhreaticLine3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPhreaticLine4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPhreaticLine4 = input.LeakageLengthInwardsPhreaticLine4,
                PiezometricHeadPhreaticLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                PiezometricHeadPhreaticLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                AdjustPhreaticLine3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift
            };
        }
    }
}