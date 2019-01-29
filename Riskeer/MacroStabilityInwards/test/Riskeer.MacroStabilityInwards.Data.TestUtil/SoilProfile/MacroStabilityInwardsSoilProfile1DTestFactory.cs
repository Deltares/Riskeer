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

using Riskeer.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile
{
    /// <summary>
    /// A factor to create configured <see cref="MacroStabilityInwardsSoilProfile1D"/> that 
    /// can be used for testing.
    /// </summary>
    public static class MacroStabilityInwardsSoilProfile1DTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilProfile1D"/> which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Name"/> set to "Profile".</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Bottom"/> set to <c>0.0</c>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// with <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> set to <c>0.0</c>.</item>
        /// </list>
        /// </summary>
        /// <returns>A configured <see cref="MacroStabilityInwardsSoilProfile1D"/>.</returns>
        public static MacroStabilityInwardsSoilProfile1D CreateMacroStabilityInwardsSoilProfile1D()
        {
            return CreateMacroStabilityInwardsSoilProfile1D("Profile");
        }

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilProfile1D"/> which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Name"/> set to <paramref name="name"/>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Bottom"/> set to <c>0.0</c>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// with <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> set to <c>0.0</c>.</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name for the profile.</param>
        /// <returns>A configured <see cref="MacroStabilityInwardsSoilProfile1D"/>.</returns>
        public static MacroStabilityInwardsSoilProfile1D CreateMacroStabilityInwardsSoilProfile1D(string name)
        {
            return new MacroStabilityInwardsSoilProfile1D(name, 0.0, new[]
            {
                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D()
            });
        }
    }
}