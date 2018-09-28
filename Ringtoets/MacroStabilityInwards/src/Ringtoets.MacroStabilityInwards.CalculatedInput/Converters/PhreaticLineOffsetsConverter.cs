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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.CalculatedInput.Converters
{
    /// <summary>
    /// Converter to convert <see cref="IMacroStabilityInwardsLocationInput"/> phreatic line offset properties
    /// into <see cref="PhreaticLineOffsets"/>.
    /// </summary>
    public static class PhreaticLineOffsetsConverter
    {
        /// <summary>
        /// Converts <see cref="IMacroStabilityInwardsLocationInput"/> phreatic line offset properties
        /// into <see cref="PhreaticLineOffsets"/>.
        /// </summary>
        /// <param name="input">The input to get the properties from.</param>
        /// <returns>The converted <see cref="PhreaticLineOffsets"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        public static PhreaticLineOffsets Convert(IMacroStabilityInwardsLocationInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.UseDefaultOffsets
                       ? new PhreaticLineOffsets()
                       : new PhreaticLineOffsets(input.PhreaticLineOffsetBelowDikeTopAtRiver,
                                                 input.PhreaticLineOffsetBelowDikeTopAtPolder,
                                                 input.PhreaticLineOffsetBelowDikeToeAtPolder,
                                                 input.PhreaticLineOffsetBelowShoulderBaseInside);
        }
    }
}