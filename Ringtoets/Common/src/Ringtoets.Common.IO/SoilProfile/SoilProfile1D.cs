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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class represents a one dimensional soil profile.
    /// </summary>
    public class SoilProfile1D : ISoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile1D"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        public SoilProfile1D(string name, double bottom)
        {
            Name = name;
            Bottom = bottom;
        }

        /// <summary>
        /// Gets the bottom level of the <see cref="SoilProfile1D"/>.
        /// </summary>
        public double Bottom { get; }

        public string Name { get; }
    }
}