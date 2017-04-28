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
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.TestUtil
{
    /// <summary>
    /// A <see cref="PipingSoilProfile"/> configured to be usable immediately for testing
    /// purposes.
    /// </summary>
    public class TestPipingSoilProfile : PipingSoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestPipingSoilProfile"/>, which is a <see cref="PipingSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="PipingSoilProfile.Name"/> set to <see cref="string.Empty"/></item>
        /// <item><see cref="PipingSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="PipingSoilProfile.Layers"/> set to a collection with a single <see cref="PipingSoilLayer"/>
        /// with <see cref="PipingSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="PipingSoilProfile.SoilProfileType"/> set to <see cref="SoilProfileType.SoilProfile1D"/>.</item>
        /// </list>
        /// </summary>
        public TestPipingSoilProfile() : this("") {}

        /// <summary>
        /// Creates a new instance of <see cref="TestPipingSoilProfile"/>, which is a <see cref="PipingSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="PipingSoilProfile.Name"/> set to <paramref name="name"/></item>
        /// <item><see cref="PipingSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="PipingSoilProfile.Layers"/> set to a collection with a single <see cref="PipingSoilLayer"/>
        /// with <see cref="PipingSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="PipingSoilProfile.SoilProfileType"/> set to <see cref="SoilProfileType.SoilProfile1D"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name for the profile.</param>
        public TestPipingSoilProfile(string name) : this(name, SoilProfileType.SoilProfile1D) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestPipingSoilProfile"/>, which is a <see cref="PipingSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="PipingSoilProfile.Name"/> set to <paramref name="name"/></item>
        /// <item><see cref="PipingSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="PipingSoilProfile.Layers"/> set to a collection with a single <see cref="PipingSoilLayer"/>
        /// with <see cref="PipingSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// <item><see cref="PipingSoilProfile.SoilProfileType"/> set to <paramref name="soilProfileType"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name for the profile.</param>
        /// <param name="soilProfileType">The type of the profile.</param>
        public TestPipingSoilProfile(string name, SoilProfileType soilProfileType) : base(name, 0.0, new Collection<PipingSoilLayer>
        {
            new PipingSoilLayer(0.0)
            {
                IsAquifer = true
            }
        }, soilProfileType, 0) {}
    }
}