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
using System.ComponentModel;
using Deltares.WaternetCreator;
using Deltares.WTIStability;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="StabilityLocation"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class StabilityLocationCreator
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
        /// is a valid value but unsupported.</exception>
        public static StabilityLocation CreateExtreme(UpliftVanCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new StabilityLocation
            {
                DikeSoilScenario = ConvertDikeSoilScenario(input.DikeSoilScenario),
                WaternetCreationMode = ConvertWaternetCreationMode(input.WaternetCreationMode),
                PlLineCreationMethod = ConvertPlLineCreationMethod(input.PlLineCreationMethod),
                WaterLevelRiver = input.AssessmentLevel,
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                WaterLevelPolder = input.WaterLevelPolderExtreme,
                DrainageConstructionPresent = input.DrainageConstruction.IsPresent,
                XCoordMiddleDrainageConstruction = input.DrainageConstruction.XCoordinate,
                ZCoordMiddleDrainageConstruction = input.DrainageConstruction.ZCoordinate,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                UseDefaultOffsets = input.PhreaticLineOffsetsExtreme.UseDefaults,
                PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetsExtreme.BelowDikeTopAtRiver,
                PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetsExtreme.BelowDikeTopAtPolder,
                PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetsExtreme.BelowShoulderBaseInside,
                PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetsExtreme.BelowDikeToeAtPolder,
                AdjustPl3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                LeakageLengthOutwardsPl3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPl3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPl4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPl4 = input.LeakageLengthInwardsPhreaticLine4,
                HeadInPlLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                HeadInPlLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                PenetrationLength = input.PenetrationLength
            };
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
        /// is a valid value but unsupported.</exception>
        public static StabilityLocation CreateDaily(UpliftVanCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new StabilityLocation
            {
                DikeSoilScenario = ConvertDikeSoilScenario(input.DikeSoilScenario),
                WaternetCreationMode = ConvertWaternetCreationMode(input.WaternetCreationMode),
                PlLineCreationMethod = ConvertPlLineCreationMethod(input.PlLineCreationMethod),
                WaterLevelRiver = input.WaterLevelRiverAverage,
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                WaterLevelPolder = input.WaterLevelPolderDaily,
                DrainageConstructionPresent = input.DrainageConstruction.IsPresent,
                XCoordMiddleDrainageConstruction = input.DrainageConstruction.XCoordinate,
                ZCoordMiddleDrainageConstruction = input.DrainageConstruction.ZCoordinate,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                UseDefaultOffsets = input.PhreaticLineOffsetsDaily.UseDefaults,
                PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetsDaily.BelowDikeTopAtRiver,
                PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetsDaily.BelowDikeTopAtPolder,
                PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetsDaily.BelowShoulderBaseInside,
                PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetsDaily.BelowDikeToeAtPolder,
                AdjustPl3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                LeakageLengthOutwardsPl3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPl3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPl4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPl4 = input.LeakageLengthInwardsPhreaticLine4,
                HeadInPlLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                HeadInPlLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                PenetrationLength = 0.0
            };
        }

        /// <summary>
        /// Converts a <see cref="MacroStabilityInwardsDikeSoilScenario"/> into a <see cref="DikeSoilScenario"/>.
        /// </summary>
        /// <param name="dikeSoilScenario">The <see cref="MacroStabilityInwardsDikeSoilScenario"/> to convert.</param>
        /// <returns>A <see cref="DikeSoilScenario"/> based on <paramref name="dikeSoilScenario"/>.</returns>
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

        /// <summary>
        /// Converts a <see cref="UpliftVanWaternetCreationMode"/> into a <see cref="WaternetCreationMode"/>.
        /// </summary>
        /// <param name="waternetCreationMode">The <see cref="UpliftVanWaternetCreationMode"/> to convert.</param>
        /// <returns>A <see cref="WaternetCreationMode"/> based on <paramref name="waternetCreationMode"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="waternetCreationMode"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="waternetCreationMode"/>
        /// is a valid value but unsupported.</exception>
        private static WaternetCreationMode ConvertWaternetCreationMode(UpliftVanWaternetCreationMode waternetCreationMode)
        {
            if (!Enum.IsDefined(typeof(UpliftVanWaternetCreationMode), waternetCreationMode))
            {
                throw new InvalidEnumArgumentException(nameof(waternetCreationMode),
                                                       (int) waternetCreationMode,
                                                       typeof(UpliftVanWaternetCreationMode));
            }

            switch (waternetCreationMode)
            {
                case UpliftVanWaternetCreationMode.CreateWaternet:
                    return WaternetCreationMode.CreateWaternet;
                case UpliftVanWaternetCreationMode.FillInWaternetValues:
                    return WaternetCreationMode.FillInWaternetValues;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="UpliftVanPlLineCreationMethod"/> into a <see cref="PlLineCreationMethod"/>.
        /// </summary>
        /// <param name="plLineCreationMethod">The <see cref="UpliftVanPlLineCreationMethod"/> to convert.</param>
        /// <returns>A <see cref="PlLineCreationMethod"/> based on <paramref name="plLineCreationMethod"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="plLineCreationMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="plLineCreationMethod"/>
        /// is a valid value but unsupported.</exception>
        private static PlLineCreationMethod ConvertPlLineCreationMethod(UpliftVanPlLineCreationMethod plLineCreationMethod)
        {
            if (!Enum.IsDefined(typeof(UpliftVanPlLineCreationMethod), plLineCreationMethod))
            {
                throw new InvalidEnumArgumentException(nameof(plLineCreationMethod),
                                                       (int) plLineCreationMethod,
                                                       typeof(UpliftVanPlLineCreationMethod));
            }

            switch (plLineCreationMethod)
            {
                case UpliftVanPlLineCreationMethod.ExpertKnowledgeRrd:
                    return PlLineCreationMethod.ExpertKnowledgeRrd;
                case UpliftVanPlLineCreationMethod.ExpertKnowledgeLinearInDike:
                    return PlLineCreationMethod.ExpertKnowledgeLinearInDike;
                case UpliftVanPlLineCreationMethod.RingtoetsWti2017:
                    return PlLineCreationMethod.RingtoetsWti2017;
                case UpliftVanPlLineCreationMethod.DupuitStatic:
                    return PlLineCreationMethod.DupuitStatic;
                case UpliftVanPlLineCreationMethod.DupuitDynamic:
                    return PlLineCreationMethod.DupuitDynamic;
                case UpliftVanPlLineCreationMethod.Sensors:
                    return PlLineCreationMethod.Sensors;
                case UpliftVanPlLineCreationMethod.None:
                    return PlLineCreationMethod.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}