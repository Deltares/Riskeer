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
using System.ComponentModel;
using Deltares.WTIStability;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="StabilityLocation"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class UpliftVanStabilityLocationCreator
    {
        /// <summary>
        /// Creates a <see cref="StabilityLocation"/> based on the given <paramref name="input"/> under extreme circumstances,
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="StabilityLocation"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static StabilityLocation CreateExtreme(UpliftVanCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StabilityLocation location = CreateBaseLocation(input);
            location.WaterLevelRiver = input.AssessmentLevel;
            location.WaterLevelPolder = input.WaterLevelPolderExtreme;
            location.UseDefaultOffsets = input.PhreaticLineOffsetsExtreme.UseDefaults;
            location.PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetsExtreme.BelowDikeTopAtRiver;
            location.PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetsExtreme.BelowDikeTopAtPolder;
            location.PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetsExtreme.BelowShoulderBaseInside;
            location.PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetsExtreme.BelowDikeToeAtPolder;
            location.PenetrationLength = input.PenetrationLengthExtreme;
            return location;
        }

        /// <summary>
        /// Creates a <see cref="StabilityLocation"/> based on the given <paramref name="input"/> under daily circumstances,
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="StabilityLocation"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static StabilityLocation CreateDaily(UpliftVanCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StabilityLocation location = CreateBaseLocation(input);
            location.WaterLevelRiver = input.WaterLevelRiverAverage;
            location.WaterLevelPolder = input.WaterLevelPolderDaily;
            location.UseDefaultOffsets = input.PhreaticLineOffsetsDaily.UseDefaults;
            location.PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetsDaily.BelowDikeTopAtRiver;
            location.PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetsDaily.BelowDikeTopAtPolder;
            location.PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetsDaily.BelowShoulderBaseInside;
            location.PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetsDaily.BelowDikeToeAtPolder;
            location.PenetrationLength = input.PenetrationLengthDaily;
            return location;
        }

        private static StabilityLocation CreateBaseLocation(UpliftVanCalculatorInput input)
        {
            return new StabilityLocation
            {
                DikeSoilScenario = StabilityLocationCreatorHelper.ConvertDikeSoilScenario(input.DikeSoilScenario),
                WaternetCreationMode = StabilityLocationCreatorHelper.ConvertWaternetCreationMode(input.WaternetCreationMode),
                PlLineCreationMethod = StabilityLocationCreatorHelper.ConvertPlLineCreationMethod(input.PlLineCreationMethod),
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                DrainageConstructionPresent = input.DrainageConstruction.IsPresent,
                XCoordMiddleDrainageConstruction = input.DrainageConstruction.XCoordinate,
                ZCoordMiddleDrainageConstruction = input.DrainageConstruction.ZCoordinate,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                AdjustPl3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                LeakageLengthOutwardsPl3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPl3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPl4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPl4 = input.LeakageLengthInwardsPhreaticLine4,
                HeadInPlLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                HeadInPlLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards
            };
        }
    }
}