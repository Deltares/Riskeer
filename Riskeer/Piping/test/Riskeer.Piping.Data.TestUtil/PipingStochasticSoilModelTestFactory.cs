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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="PipingStochasticSoilModel"/> instances that can be used for testing.
    /// </summary>
    public static class PipingStochasticSoilModelTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the stochastic soil model.</param>
        /// <returns>A valid instance of <see cref="PipingStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel(string name)
        {
            return CreatePipingStochasticSoilModel(name, new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            });
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <returns>A valid instance of <see cref="PipingStochasticSoilModel"/>.</returns>
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel()
        {
            return CreatePipingStochasticSoilModel(string.Empty);
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the stochastic soil model.</param>
        /// <param name="geometry">The geometry of the stochastic soil model.</param>
        /// <returns>A valid instance of <see cref="PipingStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="geometry"/> is empty.</exception>
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel(string name, IEnumerable<Point2D> geometry)
        {
            return new PipingStochasticSoilModel(name, geometry, new[]
            {
                new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("A")),
                new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("B"))
            });
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the stochastic soil model.</param>
        /// <param name="pipingStochasticSoilProfiles">The stochastic soil profiles of the soil model.</param>
        /// <returns>A valid instance of <see cref="PipingStochasticSoilModel"/>..</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="pipingStochasticSoilProfiles"/> is empty.</exception>
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel(string name, IEnumerable<PipingStochasticSoilProfile> pipingStochasticSoilProfiles)
        {
            return new PipingStochasticSoilModel(name, new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, pipingStochasticSoilProfiles);
        }

        /// <summary>
        /// Creates an instance of <see cref="PipingStochasticSoilModel"/> 
        /// with specified stochastic soil profiles and an empty name.
        /// </summary>
        /// <param name="stochasticSoilProfiles">The geometry of the soil model.</param>
        /// <returns>A valid configured <see cref="PipingStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochasticSoilProfiles"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <oaramref name="stochasticSoilProfiles"/> is 
        /// <c>empty</c>.</exception>
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel(IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles)
        {
            return CreatePipingStochasticSoilModel(string.Empty, stochasticSoilProfiles);
        }
    }
}