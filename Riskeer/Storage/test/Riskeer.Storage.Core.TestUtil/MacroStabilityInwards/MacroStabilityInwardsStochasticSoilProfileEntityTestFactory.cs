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

using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil.MacroStabilityInwards
{
    /// <summary>
    /// Factory for creating a <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> that can 
    /// be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsStochasticSoilProfileEntityTestFactory
    {
        /// <summary>
        /// Creates a valid <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.
        /// </summary>
        /// <returns>A valid <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.</returns>
        public static MacroStabilityInwardsStochasticSoilProfileEntity CreateStochasticSoilProfileEntity()
        {
            return new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = 0.34,
                MacroStabilityInwardsSoilProfileOneDEntity = new MacroStabilityInwardsSoilProfileOneDEntity
                {
                    Name = "Profile Name",
                    MacroStabilityInwardsSoilLayerOneDEntities =
                    {
                        new MacroStabilityInwardsSoilLayerOneDEntity()
                    }
                }
            };
        }
    }
}