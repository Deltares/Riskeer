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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for converting <see cref=" MacroStabilityInwardsLocationInputBase"/> to 
    /// <see cref="MacroStabilityInwardsLocationInputConfiguration"/>.
    /// </summary>
    public static class MacroStabilityInwardsLocationInputConversionExtensions
    {
        /// <summary>
        /// Configure a new <see cref="MacroStabilityInwardsLocationInputConfiguration"/> with 
        /// values taken from <paramref name="inputDaily"/>.
        /// </summary>
        /// <param name="inputDaily">The input to take the values from.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsLocationInputConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputDaily"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsLocationInputConfiguration ToMacroStabilityInwardsLocationInputConfiguration(
            this IMacroStabilityInwardsLocationInputDaily inputDaily)
        {
            if (inputDaily == null)
            {
                throw new ArgumentNullException(nameof(inputDaily));
            }

            var configuration = new MacroStabilityInwardsLocationInputConfiguration();

            SetMacroStabilityInwardsLocationInputParameters(configuration, inputDaily);

            return configuration;
        }

        /// <summary>
        /// Configure a new <see cref="MacroStabilityInwardsLocationInputExtremeConfiguration"/> with 
        /// values taken from <paramref name="inputExtreme"/>.
        /// </summary>
        /// <param name="inputExtreme">The input to take the values from.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsLocationInputExtremeConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputExtreme"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsLocationInputExtremeConfiguration ToMacroStabilityInwardsLocationInputExtremeConfiguration(
            this IMacroStabilityInwardsLocationInputExtreme inputExtreme)
        {
            if (inputExtreme == null)
            {
                throw new ArgumentNullException(nameof(inputExtreme));
            }

            var configuration = new MacroStabilityInwardsLocationInputExtremeConfiguration
            {
                PenetrationLength = inputExtreme.PenetrationLength
            };

            SetMacroStabilityInwardsLocationInputParameters(configuration, inputExtreme);

            return configuration;
        }

        private static void SetMacroStabilityInwardsLocationInputParameters(MacroStabilityInwardsLocationInputConfiguration configuration,
                                                                            IMacroStabilityInwardsLocationInput input)
        {
            configuration.WaterLevelPolder = input.WaterLevelPolder;
            configuration.UseDefaultOffsets = input.UseDefaultOffsets;
            configuration.PhreaticLineOffsetBelowDikeTopAtRiver = input.PhreaticLineOffsetBelowDikeTopAtRiver;
            configuration.PhreaticLineOffsetBelowDikeTopAtPolder = input.PhreaticLineOffsetBelowDikeTopAtPolder;
            configuration.PhreaticLineOffsetBelowShoulderBaseInside = input.PhreaticLineOffsetBelowShoulderBaseInside;
            configuration.PhreaticLineOffsetBelowDikeToeAtPolder = input.PhreaticLineOffsetBelowDikeToeAtPolder;
        }
    }
}