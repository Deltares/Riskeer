// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Riskeer.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationsReaderTest
    {
        [Test]
        public void ReadDuneLocations_Always_ReadsEmbeddedDuneLocations()
        {
            // Setup
            var reader = new DuneLocationsReader();

            // Call
            IEnumerable<ReadDuneLocation> readDuneLocations = reader.ReadDuneLocations();

            // Assert
            Assert.AreEqual(2054, readDuneLocations.Count());

            AssertReadDuneLocation(readDuneLocations.ElementAt(27), "001-01_0028_SCHR_02_jr003200", new Point2D(205089, 611656), 2, 3200);
            AssertReadDuneLocation(readDuneLocations.ElementAt(301), "003-01_0001_TERS_04_jr000000", new Point2D(140589, 596462), 4, 0);
            AssertReadDuneLocation(readDuneLocations.ElementAt(1350), "extra_0005_MSVL_10_jrm000300", new Point2D(61531, 445004), 10, -300);
        }

        private static void AssertReadDuneLocation(ReadDuneLocation readDuneLocation, string expectedName, Point2D expectedLocation, int expectedCoastalAreaId, int expectedOffset)
        {
            Assert.AreEqual(expectedName, readDuneLocation.Name);
            Assert.AreEqual(expectedLocation, readDuneLocation.Location);
            Assert.AreEqual(expectedCoastalAreaId, readDuneLocation.CoastalAreaId);
            Assert.AreEqual(expectedOffset, readDuneLocation.Offset);
        }
    }
}