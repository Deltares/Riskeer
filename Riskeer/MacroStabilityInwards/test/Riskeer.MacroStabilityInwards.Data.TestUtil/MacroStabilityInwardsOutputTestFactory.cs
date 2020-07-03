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
using System.Linq;
using Core.Common.Base.Data;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Factory that creates instances of <see cref="MacroStabilityInwardsOutput"/>
    /// that can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsOutputTestFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsOutput"/> with default values.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsOutput"/>.</returns>
        public static MacroStabilityInwardsOutput CreateOutput()
        {
            return CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties());
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="properties">The container of the properties for the <see cref="MacroStabilityInwardsOutput"/>.</param>
        /// <returns>A <see cref="MacroStabilityInwardsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsOutput CreateOutput(MacroStabilityInwardsOutput.ConstructionProperties properties)
        {
            return new MacroStabilityInwardsOutput(new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                                         MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                                         new[]
                                                                                         {
                                                                                             MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                                                                                             MacroStabilityInwardsSliceTestFactory.CreateSlice(),
                                                                                             MacroStabilityInwardsSliceTestFactory.CreateSlice()
                                                                                         }, 0, 0),
                                                   new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                                               MacroStabilityInwardsGridTestFactory.Create(),
                                                                                               new[]
                                                                                               {
                                                                                                   (RoundedDouble) (-3.5),
                                                                                                   (RoundedDouble) 0.0,
                                                                                                   (RoundedDouble) 2.0
                                                                                               }),
                                                   properties);
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsOutput"/> without slices.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsOutput"/>.</returns>
        public static MacroStabilityInwardsOutput CreateOutputWithoutSlices()
        {
            return new MacroStabilityInwardsOutput(
                new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                      MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                      Enumerable.Empty<MacroStabilityInwardsSlice>(), 0, 0),
                new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                            MacroStabilityInwardsGridTestFactory.Create(),
                                                            new RoundedDouble[0]),
                new MacroStabilityInwardsOutput.ConstructionProperties());
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsOutput"/> with initialized values
        /// for all properties.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsOutput"/>.</returns>
        public static MacroStabilityInwardsOutput CreateRandomOutput()
        {
            var random = new Random(21);
            var properties = new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = random.NextDouble(),
                ForbiddenZonesXEntryMax = random.NextDouble(),
                ForbiddenZonesXEntryMin = random.NextDouble()
            };

            return CreateOutput(properties);
        }
    }
}