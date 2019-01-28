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

using System;
using System.Collections.Generic;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Piping.IO.TestUtil
{
    /// <summary>
    /// Factory which creates instances of <see cref="StochasticSoilModel"/> configured
    /// for piping failure mechanism which can be used for testing purposes.
    /// </summary>
    public static class PipingStochasticSoilModelTestFactory
    {
        /// <summary>
        /// Creates a <see cref="StochasticSoilModel"/> with a predefined geometry.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles belonging to the soil model.</param>
        /// <returns>A configured <see cref="StochasticSoilModel"/> with a predefined geometry.</returns>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="stochasticSoilProfiles"/> is <c>null</c>.</exception>
        public static StochasticSoilModel CreatePipingStochasticSoilModelWithGeometry(IEnumerable<StochasticSoilProfile> stochasticSoilProfiles)
        {
            return CreatePipingStochasticSoilModelWithGeometry("Piping Stochastic Soil Model", stochasticSoilProfiles);
        }

        /// <summary>
        /// Creates a <see cref="StochasticSoilModel"/> with a predefined geometry.
        /// </summary>
        /// <param name="soilModelName">The name of the stochastic soil model.</param>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles belonging to the soil model.</param>
        /// <returns>A configured <see cref="StochasticSoilModel"/> with a predefined geometry.</returns>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="soilModelName"/> or 
        /// <paramref name="stochasticSoilProfiles"/> is <c>null</c>.</exception>
        public static StochasticSoilModel CreatePipingStochasticSoilModelWithGeometry(string soilModelName,
                                                                                      IEnumerable<StochasticSoilProfile> stochasticSoilProfiles)
        {
            return StochasticSoilModelTestFactory.CreateStochasticSoilModelWithGeometry(soilModelName,
                                                                                        FailureMechanismType.Piping,
                                                                                        stochasticSoilProfiles);
        }
    }
}