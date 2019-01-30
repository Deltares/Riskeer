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

using Riskeer.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="SoilLayer1D"/> instances that can be used
    /// for testing.
    /// </summary>
    public static class SoilLayer1DTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer1D"/> with a valid value for the 
        /// <see cref="SoilLayer1D.IsAquifer"/>.
        /// </summary>
        /// <param name="top">The value of the <see cref="SoilLayer1D.Top"/>.</param>
        /// <returns>The created <see cref="SoilLayer1D"/>.</returns>
        public static SoilLayer1D CreateSoilLayer1DWithValidAquifer(double top = 3.14)
        {
            return new SoilLayer1D(top)
            {
                IsAquifer = 0.0
            };
        }
    }
}