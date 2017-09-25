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

using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="UpliftVanCalculatorResult"/>
    /// instances that can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsCalculatorResultTestFactory
    {
        /// <summary>
        /// Creates a new <see cref="UpliftVanCalculatorResult"/>.
        /// </summary>
        /// <returns>The created <see cref="UpliftVanCalculatorResult"/>.</returns>
        public static UpliftVanCalculatorResult Create()
        {
            return new UpliftVanCalculatorResult(
                MacroStabilityInwardsSlidingCurveResultTestFactory.Create(),
                new MacroStabilityInwardsUpliftVanCalculationGridResult(
                    MacroStabilityInwardsGridResultTestFactory.Create(),
                    MacroStabilityInwardsGridResultTestFactory.Create(),
                    new[]
                    {
                        3,
                        2,
                        1.5
                    }),
                new UpliftVanCalculatorResult.ConstructionProperties
                {
                    FactorOfStability = 0.1,
                    ZValue = 0.2,
                    ForbiddenZonesXEntryMin = 0.3,
                    ForbiddenZonesXEntryMax = 0.4,
                    GridAutomaticallyCalculated = true,
                    ForbiddenZonesAutomaticallyCalculated = true
                });
        }
    }
}