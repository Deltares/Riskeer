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
using System.ComponentModel;
using Deltares.WaternetCreator;
using Deltares.WTIStability;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="StabilityLocation"/> instances which are required by the <see cref="MacroStabilityInwardsCalculator"/>.
    /// </summary>
    internal static class StabilityLocationCreator
    {
        /// <summary>
        /// Creates a <see cref="StabilityLocation"/> based on the given <paramref name="input"/>,
        /// which can be used in the <see cref="MacroStabilityInwardsCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="MacroStabilityInwardsCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="StabilityLocation"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="MacroStabilityInwardsCalculatorInput.DikeSoilScenario"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="MacroStabilityInwardsCalculatorInput.DikeSoilScenario"/>
        /// is a valid value but unsupported.</exception>
        public static StabilityLocation Create(MacroStabilityInwardsCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new StabilityLocation
            {
                DikeSoilScenario = ConvertDikeSoilScenario(input.DikeSoilScenario),
                WaterLevelRiver = input.AssessmentLevel,
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                WaterLevelPolder = input.WaterLevelPolder,
                WaterLevelRiverLow = double.NaN,
                DrainageConstructionPresent = input.DrainageConstructionPresent,
                XCoordMiddleDrainageConstruction = input.XCoordinateDrainageConstruction,
                ZCoordMiddleDrainageConstruction = input.ZCoordinateDrainageConstruction,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                UseDefaultOffsets = input.UseDefaultOffsets,
                PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetBelowDikeTopAtRiver,
                PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetBelowDikeTopAtPolder,
                PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetBelowShoulderBaseInside,
                PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetBelowDikeToeAtPolder,
                HeadInPlLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                HeadInPlLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                AdjustPl3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                PenetrationLength = input.PenetrationLength,
                LeakageLengthOutwardsPl3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPl3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPl4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPl4 = input.LeakageLengthInwardsPhreaticLine4,
                WaternetCreationMode = WaternetCreationMode.CreateWaternet
            };
        }

        /// <summary>
        /// Converts a <see cref="MacroStabilityInwardsDikeSoilScenario"/> to a <see cref="DikeSoilScenario"/>.
        /// </summary>
        /// <param name="dikeSoilScenario">The <see cref="MacroStabilityInwardsDikeSoilScenario"/> to convert.</param>
        /// <returns>A <see cref="DikeSoilScenario"/> based on the information of <paramref name="dikeSoilScenario"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="dikeSoilScenario"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dikeSoilScenario"/>
        /// is a valid value but unsupported.</exception>
        private static DikeSoilScenario ConvertDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario dikeSoilScenario)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), dikeSoilScenario))
            {
                throw new InvalidEnumArgumentException(nameof(dikeSoilScenario),
                                                       (int) dikeSoilScenario,
                                                       typeof(MacroStabilityInwardsDikeSoilScenario));
            }

            switch (dikeSoilScenario)
            {
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay:
                    return DikeSoilScenario.ClayDikeOnClay;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay:
                    return DikeSoilScenario.SandDikeOnClay;
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand:
                    return DikeSoilScenario.ClayDikeOnSand;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand:
                    return DikeSoilScenario.SandDikeOnSand;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}