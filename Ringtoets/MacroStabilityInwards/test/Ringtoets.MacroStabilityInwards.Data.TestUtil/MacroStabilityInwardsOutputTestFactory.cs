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

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Factory that creates instances of <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>
    /// that can be used for testing.
    /// </summary>
    public class MacroStabilityInwardsOutputTestFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/> with default values.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsOutput"/>.</returns>
        public static MacroStabilityInwardsOutput CreateOutput()
        {
            return CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties());
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="properties">The container of the properties for the <see cref="MacroStabilityInwardsOutput"/>.</param>
        /// <returns>A <see cref="MacroStabilityInwardsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsOutput CreateOutput(MacroStabilityInwardsOutput.ConstructionProperties properties)
        {
            return new MacroStabilityInwardsOutput(new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                                         MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                                         new MacroStabilityInwardsSlice[0], 0, 0),
                                                   new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                                               MacroStabilityInwardsGridTestFactory.Create(),
                                                                                               new double[0]),
                                                   properties);
        }
    }
}