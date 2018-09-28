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
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.TestUtil
{
    /// <summary>
    /// Factory which creates instances of <see cref="StochasticSoilModel"/> which
    /// can be used for testing purposes.
    /// </summary>
    public static class StochasticSoilModelTestFactory
    {
        /// <summary>
        /// Creates a <see cref="StochasticSoilModel"/> with a predefined geometry.
        /// </summary>
        /// <param name="soilModelName">The name of the stochastic soil model.</param>
        /// <param name="failureMechanismType">The failure mechanism type of the stochastic soil model.</param>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles belonging to the soil model.</param>
        /// <returns>A configured <see cref="StochasticSoilModel"/> with a predefined geometry.</returns>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="soilModelName"/> or 
        /// <paramref name="stochasticSoilProfiles"/> is <c>null</c>.</exception>
        public static StochasticSoilModel CreateStochasticSoilModelWithGeometry(string soilModelName,
                                                                                FailureMechanismType failureMechanismType,
                                                                                IEnumerable<StochasticSoilProfile> stochasticSoilProfiles)
        {
            var model = new StochasticSoilModel(soilModelName, failureMechanismType)
            {
                Geometry =
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                }
            };
            model.StochasticSoilProfiles.AddRange(stochasticSoilProfiles);

            return model;
        }
    }
}