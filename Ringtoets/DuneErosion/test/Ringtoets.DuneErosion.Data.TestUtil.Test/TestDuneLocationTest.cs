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

using System;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.DuneErosion.Data.TestUtil.Test
{
    [TestFixture]
    public class TestDuneLocationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var testLocation = new TestDuneLocation();

            // Assert
            Assert.AreEqual(string.Empty, testLocation.Name);
            Assert.AreEqual(0, testLocation.Location.X);
            Assert.AreEqual(0, testLocation.Location.Y);
            Assert.AreEqual(0, testLocation.Offset.Value);
            Assert.AreEqual(0, testLocation.Orientation.Value);
            Assert.AreEqual(0, testLocation.CoastalAreaId);
            Assert.AreEqual(0, testLocation.D50.Value);

            Assert.IsNull(testLocation.Output);
        }

        [Test]
        public void CreateTestDuneLocationForExport_WithParameters_ReturnsWithExpectedValues()
        {
            // Setup 
            var random = new Random(21);
            int coastalAreaId = random.Next();
            double offset = random.NextDouble();
            double d50 = random.NextDouble();

            // Call 
            TestDuneLocation location = TestDuneLocation.CreateDuneLocationForExport(coastalAreaId, offset, d50);

            // Assert
            Assert.AreEqual(coastalAreaId, location.CoastalAreaId);
            Assert.AreEqual(offset, location.Offset, location.Offset.GetAccuracy());
            Assert.AreEqual(d50, location.D50, location.D50.GetAccuracy());

            Assert.AreEqual(string.Empty, location.Name);
            Assert.AreEqual(0, location.Location.X);
            Assert.AreEqual(0, location.Location.Y);
            Assert.AreEqual(0, location.Orientation.Value);

            Assert.IsNull(location.Output);
        }
    }
}