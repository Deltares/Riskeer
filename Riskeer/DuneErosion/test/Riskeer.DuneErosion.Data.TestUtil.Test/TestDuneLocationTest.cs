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

using NUnit.Framework;

namespace Riskeer.DuneErosion.Data.TestUtil.Test
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
            Assert.IsEmpty(testLocation.Name);
            Assert.AreEqual(0, testLocation.Location.X);
            Assert.AreEqual(0, testLocation.Location.Y);
            Assert.AreEqual(0, testLocation.Offset.Value);
            Assert.AreEqual(0, testLocation.Orientation.Value);
            Assert.AreEqual(0, testLocation.CoastalAreaId);
            Assert.AreEqual(0, testLocation.D50.Value);
        }

        [Test]
        public void ConstructorWithName_ExpectedValues()
        {
            // Setup
            const string name = "new name";

            // Call
            var testLocation = new TestDuneLocation(name);

            // Assert
            Assert.AreEqual(name, testLocation.Name);
            Assert.AreEqual(0, testLocation.Location.X);
            Assert.AreEqual(0, testLocation.Location.Y);
            Assert.AreEqual(0, testLocation.Offset.Value);
            Assert.AreEqual(0, testLocation.Orientation.Value);
            Assert.AreEqual(0, testLocation.CoastalAreaId);
            Assert.AreEqual(0, testLocation.D50.Value);
        }
    }
}