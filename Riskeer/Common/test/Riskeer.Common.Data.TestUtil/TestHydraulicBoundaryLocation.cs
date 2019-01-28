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

using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class that creates simple instances of <see cref="HydraulicBoundaryLocation"/>, that
    /// can be used during testing.
    /// </summary>
    public class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/>.
        /// </summary>
        public TestHydraulicBoundaryLocation() : this(string.Empty) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/>
        /// with the given name.
        /// </summary>
        /// <param name="name">The name for the <see cref="TestHydraulicBoundaryLocation"/>.</param>
        public TestHydraulicBoundaryLocation(string name) : base(0, name, 0, 0) {}
    }
}