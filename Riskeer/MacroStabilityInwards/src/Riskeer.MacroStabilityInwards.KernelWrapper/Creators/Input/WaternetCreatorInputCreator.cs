﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="WaternetCreatorInput"/> instances which are required by <see cref="IWaternetKernel"/>.
    /// </summary>
    internal static class WaternetCreatorInputCreator
    {
        /// <summary>
        /// Creates a <see cref="WaternetCreatorInput"/> based on the given <paramref name="input"/>
        /// which can be used by <see cref="IWaternetKernel"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaternetCalculatorInput"/> to get the information from.</param>
        /// <returns>A new <see cref="WaternetCreatorInput"/> with the given information from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="WaternetCalculatorInput.DikeSoilScenario"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="WaternetCalculatorInput.DikeSoilScenario"/>
        /// is a valid value, but unsupported.</exception>
        public static WaternetCreatorInput Create(WaternetCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new WaternetCreatorInput
            {
                DikeSoilScenario = WaternetCreatorInputHelper.ConvertDikeSoilScenario(input.DikeSoilScenario),
                WaterLevelRiver = input.AssessmentLevel,
                HeadInPlLine3 = input.AssessmentLevel,
                HeadInPlLine4 = input.AssessmentLevel,
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                WaterLevelPolder = input.WaterLevelPolder,
                DrainageConstructionPresent = input.DrainageConstruction.IsPresent,
                DrainageConstruction = input.DrainageConstruction.IsPresent
                                           ? new Point2D(input.DrainageConstruction.XCoordinate, input.DrainageConstruction.ZCoordinate)
                                           : null,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                UseDefaultOffsets = input.PhreaticLineOffsets.UseDefaults,
                PlLineOffsetBelowPointBRingtoetsWti2017 = input.PhreaticLineOffsets.BelowDikeTopAtRiver,
                PlLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsets.BelowDikeTopAtPolder,
                PlLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsets.BelowShoulderBaseInside,
                PlLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsets.BelowDikeToeAtPolder,
                AdjustPl3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                LeakageLengthOutwardsPl3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPl3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPl4 = input.LeakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPl4 = input.LeakageLengthInwardsPhreaticLine4,
                HeadInPlLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                HeadInPlLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                PenetrationLength = input.PenetrationLength,
                UnitWeightWater = input.WaterVolumetricWeight
            };
        }
    }
}