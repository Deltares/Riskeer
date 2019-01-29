// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new MapDataChild(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the map data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "Some name";

            // Call
            var data = new MapDataChild(name);

            // Assert
            Assert.IsInstanceOf<Observable>(data);
            Assert.AreEqual(name, data.Name);
            Assert.IsTrue(data.IsVisible);
        }

        [Test]
        public void Name_SetName_ReturnsNewName()
        {
            // Setup
            const string name = "Some name";
            const string newName = "Something";
            var data = new MapDataChild(name);

            // Precondition
            Assert.AreEqual(name, data.Name);

            // Call
            data.Name = newName;

            // Assert
            Assert.AreNotEqual(name, data.Name);
            Assert.AreEqual(newName, data.Name);
        }

        private class MapDataChild : MapData
        {
            public MapDataChild(string name) : base(name) {}
        }
    }
}