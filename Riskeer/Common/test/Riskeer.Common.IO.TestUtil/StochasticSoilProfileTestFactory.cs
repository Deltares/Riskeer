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

using System;
using Riskeer.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.TestUtil
{
    /// <summary>
    /// Factory which creates simple instance of <see cref="StochasticSoilProfile"/>
    /// that can be used for testing.
    /// </summary>
    public static class StochasticSoilProfileTestFactory
    {
        /// <summary>
        /// Creates a <see cref="StochasticSoilProfile"/> with a valid probability.
        /// </summary>
        /// <param name="soilProfile">The soil profile.</param>
        /// <returns>A <see cref="StochasticSoilProfile"/> with the specified <paramref name="soilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static StochasticSoilProfile CreateStochasticSoilProfileWithValidProbability(ISoilProfile soilProfile)
        {
            return new StochasticSoilProfile(0.5, soilProfile);
        }
    }
}