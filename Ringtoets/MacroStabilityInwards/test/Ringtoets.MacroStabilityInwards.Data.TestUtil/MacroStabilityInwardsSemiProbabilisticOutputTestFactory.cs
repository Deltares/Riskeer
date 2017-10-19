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

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Factory that creates instances of <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>
    /// that can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsSemiProbabilisticOutputTestFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="probability">The optional probability to set (set to <c>0</c> when not specified).</param>
        /// <returns>A <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>.</returns>
        public static MacroStabilityInwardsSemiProbabilisticOutput CreateOutput(double probability = 0)
        {
            return new MacroStabilityInwardsSemiProbabilisticOutput(0, 0, 0, probability, 0, 0);
        }
    }
}