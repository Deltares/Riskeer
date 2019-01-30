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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.TestUtil.Test
{
    [TestFixture]
    public class ReadHydraulicBoundaryDatabaseTestFactoryTest
    {
        [Test]
        public void Create_WithoutLocations_ExpectedValues()
        {
            // Call
            ReadHydraulicBoundaryDatabase readDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Assert
            Assert.IsNotNull(readDatabase.TrackId);
            Assert.AreEqual("version", readDatabase.Version);

            ReadHydraulicBoundaryLocation[] locations = readDatabase.Locations.ToArray();
            Assert.AreEqual(2, locations.Length);

            for (var i = 0; i < locations.Length; i++)
            {
                Assert.AreEqual(i + 1, locations[i].Id);
                Assert.AreEqual($"location{i + 1}", locations[i].Name);
                Assert.IsFalse(double.IsNaN(locations[i].CoordinateX));
                Assert.IsFalse(double.IsNaN(locations[i].CoordinateY));
            }
        }

        [Test]
        public void Create_WithLocations_ExpectedValues()
        {
            // Setup
            IEnumerable<ReadHydraulicBoundaryLocation> locations = Enumerable.Empty<ReadHydraulicBoundaryLocation>();

            // Call
            ReadHydraulicBoundaryDatabase readDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create(locations);

            // Assert
            Assert.AreNotEqual(0, readDatabase.TrackId);
            Assert.AreEqual("version", readDatabase.Version);
            Assert.AreSame(locations, readDatabase.Locations);
        }
    }
}