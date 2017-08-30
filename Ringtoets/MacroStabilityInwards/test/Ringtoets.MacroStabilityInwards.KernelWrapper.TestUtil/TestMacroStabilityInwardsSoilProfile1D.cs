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

using System.Collections.ObjectModel;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil
{
    /// <summary>
    /// A <see cref="MacroStabilityInwardsSoilProfile1D"/> configured to be usable immediately for testing
    /// purposes.
    /// </summary>
    public class TestMacroStabilityInwardsSoilProfile1D : MacroStabilityInwardsSoilProfile1D
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsSoilProfile1D"/>, which is a <see cref="MacroStabilityInwardsSoilProfile1D"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Name"/> set to <see cref="string.Empty"/></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// with <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> set to <c>0.0</c>.</item>
        /// </list>
        /// </summary>
        public TestMacroStabilityInwardsSoilProfile1D() : this("") {}

        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsSoilProfile1D"/>, which is a <see cref="MacroStabilityInwardsSoilProfile1D"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Name"/> set to <paramref name="name"/></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// with <see cref="MacroStabilityInwardsSoilLayer1D.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile1D.SoilProfileType"/> set to <paramref name="soilProfileType"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name for the profile.</param>
        public TestMacroStabilityInwardsSoilProfile1D(string name) : base(name, 0.0, new Collection<MacroStabilityInwardsSoilLayer1D>
        {
            new MacroStabilityInwardsSoilLayer1D(0.0)
            {
                Properties =
                {
                    IsAquifer = true
                }
            }
        }) {}
    }
}