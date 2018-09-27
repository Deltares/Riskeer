// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.TestUtil.Test
{
    [TestFixture]
    public class TestImageBasedMapDataTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Constructor_InvalidName_ThrowArgumentException(string invalidMapDataName)
        {
            // Call
            TestDelegate call = () => new TestImageBasedMapData(invalidMapDataName, true);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("Name", paramName);
        }

        [Test]
        [TestCase("a", true)]
        [TestCase("b", false)]
        public void Constructor_ValidValues_ExpectedValues(string name, bool isConfigured)
        {
            // Call
            var mapData = new TestImageBasedMapData(name, isConfigured);

            // Assert
            Assert.IsInstanceOf<ImageBasedMapData>(mapData);

            Assert.AreEqual(name, mapData.Name);
            Assert.AreEqual(isConfigured, mapData.IsConfigured);

            Assert.AreEqual(true, mapData.IsVisible);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
        }
    }
}