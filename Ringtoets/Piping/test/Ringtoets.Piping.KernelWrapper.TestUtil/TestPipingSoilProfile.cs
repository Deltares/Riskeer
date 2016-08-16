﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        /// </list>
        /// </summary>
        public TestPipingSoilProfile() : base("", 0.0, new Collection<PipingSoilLayer>
        {
            new PipingSoilLayer(0.0)
            {
                IsAquifer = true
            }
        }, SoilProfileType.SoilProfile1D, 0) {}
    }
}