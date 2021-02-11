﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ReadHydraulicBoundaryDatabase"/> instances
    /// that can be used for testing.
    /// </summary>
    public static class ReadHydraulicBoundaryDatabaseTestFactory
    {
        /// <summary>
        /// Creates a <see cref="ReadHydraulicBoundaryDatabase"/> with random values.
        /// </summary>
        /// <returns>The created <see cref="ReadHydraulicBoundaryDatabase"/>.</returns>
        public static ReadHydraulicBoundaryDatabase Create()
        {
            var random = new Random(21);
            return Create(new[]
            {
                new ReadHydraulicBoundaryLocation(1, "location1", random.NextDouble(), random.NextDouble()),
                new ReadHydraulicBoundaryLocation(2, "location2", random.NextDouble(), random.NextDouble())
            });
        }

        /// <summary>
        /// Creates a <see cref="ReadHydraulicBoundaryDatabase"/> with the provided <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The locations to add to the <see cref="ReadHydraulicBoundaryDatabase"/>.</param>
        /// <returns>The created <see cref="ReadHydraulicBoundaryDatabase"/>.</returns>
        public static ReadHydraulicBoundaryDatabase Create(IEnumerable<ReadHydraulicBoundaryLocation> locations)
        {
            var random = new Random(21);
            return new ReadHydraulicBoundaryDatabase(random.Next(), "version", locations);
        }
    }
}