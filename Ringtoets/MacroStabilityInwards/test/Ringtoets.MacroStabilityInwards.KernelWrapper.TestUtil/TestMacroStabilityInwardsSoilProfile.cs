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
    /// A <see cref="MacroStabilityInwardsSoilProfile"/> configured to be usable immediately for testing
    /// purposes.
    /// </summary>
    public class TestMacroStabilityInwardsSoilProfile : MacroStabilityInwardsSoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsSoilProfile"/>, which is a <see cref="MacroStabilityInwardsSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Name"/> set to <see cref="string.Empty"/></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer"/>
        /// with <see cref="MacroStabilityInwardsSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.SoilProfileType"/> set to <see cref="SoilProfileType.SoilProfile1D"/>.</item>
        /// </list>
        /// </summary>
        public TestMacroStabilityInwardsSoilProfile() : this("") {}

        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsSoilProfile"/>, which is a <see cref="MacroStabilityInwardsSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Name"/> set to <paramref name="name"/></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer"/>
        /// with <see cref="MacroStabilityInwardsSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.SoilProfileType"/> set to <see cref="SoilProfileType.SoilProfile1D"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name for the profile.</param>
        public TestMacroStabilityInwardsSoilProfile(string name) : this(name, SoilProfileType.SoilProfile1D) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsSoilProfile"/>, which is a <see cref="MacroStabilityInwardsSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Name"/> set to <paramref name="name"/></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.Layers"/> set to a collection with a single <see cref="MacroStabilityInwardsSoilLayer"/>
        /// with <see cref="MacroStabilityInwardsSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="MacroStabilityInwardsSoilProfile.SoilProfileType"/> set to <paramref name="soilProfileType"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name for the profile.</param>
        /// <param name="soilProfileType">The type of the profile.</param>
        public TestMacroStabilityInwardsSoilProfile(string name, SoilProfileType soilProfileType) : base(name, 0.0, new Collection<MacroStabilityInwardsSoilLayer>
        {
            new MacroStabilityInwardsSoilLayer(0.0)
            {
                IsAquifer = true
            }
        }, soilProfileType, 0) {}
    }
}