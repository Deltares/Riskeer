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
using System.Linq;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class responsible for calculating the derived macro stability inwards input.
    /// </summary>
    public class DerivedMacroStabilityInwardsInput
    {
        private readonly MacroStabilityInwardsInput input;

        /// <summary>
        /// Creates a new instance of <see cref="DerivedMacroStabilityInwardsInput"/>.
        /// </summary>
        /// <param name="input">The input to calculate the derived macro stability inwards input.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        public DerivedMacroStabilityInwardsInput(MacroStabilityInwardsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), @"Cannot create DerivedMacroStabilityInwardsInput without MacroStabilityInwardsInput.");
            }
            this.input = input;
        }

        /// <summary>
        /// 
        /// </summary>
        public MacroStabilityInwardsWaternet WaternetExtreme
        {
            get
            {
                return new MacroStabilityInwardsWaternet(Enumerable.Empty<MacroStabilityInwardsPhreaticLine>(),
                                                         Enumerable.Empty<MacroStabilityInwardsWaternetLine>());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MacroStabilityInwardsWaternet WaternetDaily
        {
            get
            {
                return new MacroStabilityInwardsWaternet(Enumerable.Empty<MacroStabilityInwardsPhreaticLine>(),
                                                         Enumerable.Empty<MacroStabilityInwardsWaternetLine>());
            }
        }
    }
}