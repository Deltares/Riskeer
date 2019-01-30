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
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.Primitives.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="Ring"/> instances that can be used
    /// for testing.
    /// </summary>
    public static class RingTestFactory
    {
        /// <summary>
        /// Creates a new random instance of <see cref="Ring"/>.
        /// </summary>
        /// <param name="seed">The seed to use for the pseudo-random number generator.</param>
        /// <returns>The created <see cref="Ring"/>.</returns>
        public static Ring CreateRandomRing(int seed = 39)
        {
            var random = new Random(seed);
            return new Ring(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });
        }
    }
}