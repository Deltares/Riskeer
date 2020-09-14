// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="WaternetCreatorInput"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class UpliftVanWaternetCreatorInputCreator
    {
        /// <summary>
        /// Creates a <see cref="WaternetCreatorInput"/> based on the given <paramref name="input"/> under extreme circumstances,
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="WaternetCreatorInput"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static WaternetCreatorInput CreateExtreme(UpliftVanCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            WaternetCreatorInput creatorInput = CreateBaseLocation(input);
            creatorInput.WaterLevelRiver = input.AssessmentLevel;
            creatorInput.HeadInPlLine3 = input.AssessmentLevel;
            creatorInput.HeadInPlLine4 = input.AssessmentLevel;
            creatorInput.WaterLevelPolder = input.WaterLevelPolderExtreme;
            creatorInput.UseDefaultOffsets = input.PhreaticLineOffsetsExtreme.UseDefaults;
            creatorInput.PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetsExtreme.BelowDikeTopAtRiver;
            creatorInput.PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetsExtreme.BelowDikeTopAtPolder;
            creatorInput.PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetsExtreme.BelowShoulderBaseInside;
            creatorInput.PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetsExtreme.BelowDikeToeAtPolder;
            creatorInput.PenetrationLength = input.PenetrationLengthExtreme;
            return creatorInput;
        }

        /// <summary>
        /// Creates a <see cref="Location"/> based on the given <paramref name="input"/> under daily circumstances,
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="Location"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="UpliftVanCalculatorInput.DikeSoilScenario"/>,
        /// <see cref="UpliftVanCalculatorInput.WaternetCreationMode"/> or <see cref="UpliftVanCalculatorInput.PlLineCreationMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static WaternetCreatorInput CreateDaily(UpliftVanCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            WaternetCreatorInput creatorInput = CreateBaseLocation(input);
            creatorInput.WaterLevelRiver = input.WaterLevelRiverAverage;
            creatorInput.HeadInPlLine3 = input.WaterLevelRiverAverage;
            creatorInput.HeadInPlLine4 = input.WaterLevelRiverAverage;
            creatorInput.WaterLevelPolder = input.WaterLevelPolderDaily;
            creatorInput.UseDefaultOffsets = input.PhreaticLineOffsetsDaily.UseDefaults;
            creatorInput.PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsetsDaily.BelowDikeTopAtRiver;
            creatorInput.PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetsDaily.BelowDikeTopAtPolder;
            creatorInput.PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetsDaily.BelowShoulderBaseInside;
            creatorInput.PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetsDaily.BelowDikeToeAtPolder;
            creatorInput.PenetrationLength = input.PenetrationLengthDaily;
            return creatorInput;
        }

        private static WaternetCreatorInput CreateBaseLocation(UpliftVanCalculatorInput input)
        {
            return new WaternetCreatorInput
            {
                DikeSoilScenario = LocationCreatorHelper.ConvertDikeSoilScenario(input.DikeSoilScenario),
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                DrainageConstructionPresent = input.DrainageConstruction.IsPresent,
                DrainageConstruction = input.DrainageConstruction.IsPresent 
                                           ? new Point2D(input.DrainageConstruction.XCoordinate, input.DrainageConstruction.ZCoordinate)
                                           : null,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                AdjustPl3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                LeakageLengthOutwardsPl3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPl3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPl4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPl4 = input.LeakageLengthInwardsPhreaticLine4,
                HeadInPlLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                HeadInPlLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                UnitWeightWater = 9.81
            };
        }
    }
}