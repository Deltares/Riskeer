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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Factory that creates valid <see cref="MacroStabilityInwardsStochasticSoilModel"/> which 
    /// can be used for testing purposes.
    /// </summary>
    public static class MacroStabilityInwardsStochasticSoilModelTestFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/> with an empty name.
        /// </summary>
        /// <returns>A valid instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</returns>
        public static MacroStabilityInwardsStochasticSoilModel CreateValidStochasticSoilModel()
        {
            return CreateValidStochasticSoilModel(string.Empty);
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="soilModelName">The name of the soil model.</param>
        /// <returns>A valid configured <see cref="MacroStabilityInwardsStochasticSoilModel"/> with the 
        /// specified <paramref name="soilModelName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilModelName"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsStochasticSoilModel CreateValidStochasticSoilModel(string soilModelName)
        {
            return CreateValidStochasticSoilModel(soilModelName, new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            });
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="soilModelName">The name of the soil model.</param>
        /// <param name="geometry">The geometry of the soil model.</param>
        /// <returns>A valid configured <see cref="MacroStabilityInwardsStochasticSoilModel"/> with the 
        /// specified <paramref name="soilModelName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilModelName"/> or 
        /// <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <oaramref name="geometry"/> is 
        /// <c>empty</c>.</exception>
        public static MacroStabilityInwardsStochasticSoilModel CreateValidStochasticSoilModel(string soilModelName,
                                                                                              IEnumerable<Point2D> geometry)
        {
            MacroStabilityInwardsSoilProfile1D soilProfileA =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D("A");
            MacroStabilityInwardsSoilProfile1D soilProfileB =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D("B");

            var model = new MacroStabilityInwardsStochasticSoilModel(soilModelName, geometry, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, soilProfileA),
                new MacroStabilityInwardsStochasticSoilProfile(0.5, soilProfileB)
            });

            return model;
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="soilModelName">The name of the soil model.</param>
        /// <param name="stochasticSoilProfiles">The geometry of the soil model.</param>
        /// <returns>A valid configured <see cref="MacroStabilityInwardsStochasticSoilModel"/> with the 
        /// specified <paramref name="soilModelName"/> and <paramref name="stochasticSoilProfiles"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilModelName"/> or 
        /// <paramref name="stochasticSoilProfiles"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <oaramref name="stochasticSoilProfiles"/> is 
        /// <c>empty</c>.</exception>
        public static MacroStabilityInwardsStochasticSoilModel CreateValidStochasticSoilModel(string soilModelName,
                                                                                              IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles)
        {
            var model = new MacroStabilityInwardsStochasticSoilModel(soilModelName,
                                                                     new[]
                                                                     {
                                                                         new Point2D(1, 1),
                                                                         new Point2D(2, 2)
                                                                     }, stochasticSoilProfiles);
            return model;
        }

        /// <summary>
        /// Creates an instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/> 
        /// with specified stochastic soil profiles and an empty name.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The geometry of the soil model.</param>
        /// <returns>A valid configured <see cref="MacroStabilityInwardsStochasticSoilModel"/> with the 
        /// specified <paramref name="stochasticSoilProfiles"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilProfiles"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <oaramref name="stochasticSoilProfiles"/> is 
        /// <c>empty</c>.</exception>
        public static MacroStabilityInwardsStochasticSoilModel CreateValidStochasticSoilModel(IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles)
        {
            return CreateValidStochasticSoilModel(string.Empty, stochasticSoilProfiles);
        }
    }
}