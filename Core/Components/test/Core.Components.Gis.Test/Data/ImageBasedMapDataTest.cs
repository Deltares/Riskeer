// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class ImageBasedMapDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var mapData = new SimpleImageBasedMapData(name);

            // Assert
            Assert.IsInstanceOf<MapData>(mapData);
            Assert.AreEqual(name, mapData.Name);
            Assert.IsTrue(mapData.IsVisible);
            Assert.IsFalse(mapData.IsConfigured);

            Assert.AreEqual(2, mapData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0, mapData.Transparency.Value);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Constructor_NameInvalid_ThrowArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new SimpleImageBasedMapData(invalidName);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("Name", paramName);
        }

        [Test]
        public void Transparency_SetValue_ValueRounded()
        {
            // Setup
            var mapData = new SimpleImageBasedMapData("A");

            // Call
            mapData.Transparency = (RoundedDouble) 0.9938;

            // Assert
            Assert.AreEqual(2, mapData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0.99, mapData.Transparency.Value);
        }

        private class SimpleImageBasedMapData : ImageBasedMapData
        {
            public SimpleImageBasedMapData(string name) : base(name) {}
        }
    }
}