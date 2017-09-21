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

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// A <see cref="MacroStabilityInwardsOutput"/> configured to be used immediately for testing purposes.
    /// </summary>
    /// <seealso cref="MacroStabilityInwardsOutput" />
    public class TestMacroStabilityInwardsOutput : MacroStabilityInwardsOutput
    {
        public TestMacroStabilityInwardsOutput()
            : this(new ConstructionProperties
                   {
                       FactorOfStability = 1.1
                   }) {}

        public TestMacroStabilityInwardsOutput(ConstructionProperties properties)
            : base (new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                         MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                         new MacroStabilityInwardsSlice[0], 0, 0),
                   new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridOutputTestFactory.Create(),
                                                               MacroStabilityInwardsGridOutputTestFactory.Create(),
                                                               new double[0]),
                   properties)
        {
            
        }
    }
}