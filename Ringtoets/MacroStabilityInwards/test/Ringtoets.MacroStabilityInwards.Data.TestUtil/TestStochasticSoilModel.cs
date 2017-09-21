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

using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// <see cref="MacroStabilityInwardsStochasticSoilModel"/> for testing purposes.
    /// </summary>
    public class TestStochasticSoilModel : MacroStabilityInwardsStochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the stochastic soil model.</param>
        public TestStochasticSoilModel(string name) : base(name)
        {
            MacroStabilityInwardsSoilProfile1D soilProfileA =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D("A");
            MacroStabilityInwardsSoilProfile1D soilProfileB =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D("B");

            StochasticSoilProfiles.AddRange(new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, soilProfileA),
                new MacroStabilityInwardsStochasticSoilProfile(0.5, soilProfileB)
            });
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestStochasticSoilModel"/>.
        /// </summary>
        public TestStochasticSoilModel() : this(string.Empty) {}
    }
}