// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.IO.Test
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
            IEnumerable<DuneLocation> locations = reader.ReadDuneLocations();

            // Assert
            Assert.AreEqual(3431, locations.Count());
            foreach (DuneLocation duneLocation in locations)
            {
                Assert.IsNotNullOrEmpty(duneLocation.Name);
                Assert.IsNotNull(duneLocation.Location);
                Assert.IsNotNull(duneLocation.Location.X);
                Assert.IsNotNull(duneLocation.Location.Y);
                Assert.IsNotNull(duneLocation.CoastalAreaId);
                Assert.IsNotNull(duneLocation.Offset);
                Assert.IsNotNull(duneLocation.Orientation);
                Assert.IsNotNull(duneLocation.D50);
            }
        }
    }
}