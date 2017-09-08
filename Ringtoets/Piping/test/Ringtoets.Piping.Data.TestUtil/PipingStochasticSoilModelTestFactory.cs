﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives.TestUtil;

namespace Ringtoets.Piping.Data.TestUtil
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
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel(string name)
        {
            return new PipingStochasticSoilModel(name)
            {
                StochasticSoilProfiles =
                {
                    new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("A")),
                    new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("B"))
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        public static PipingStochasticSoilModel CreatePipingStochasticSoilModel()
        {
            return CreatePipingStochasticSoilModel(string.Empty);
        }
    }
}