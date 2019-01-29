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
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class WellKnownTileSourceMapDataTest
    {
        [Test]
        public void Constructor_ValidValues_ExpectedValues()
        {
            // Setup
            const WellKnownTileSource value = WellKnownTileSource.BingAerial;

            // Call
            var mapData = new WellKnownTileSourceMapData(value);

            // Assert
            Assert.IsInstanceOf<ImageBasedMapData>(mapData);

            Assert.AreEqual("Bing Maps - Satelliet", mapData.Name);
            Assert.IsTrue(mapData.IsVisible);
            Assert.IsTrue(mapData.IsConfigured);
            Assert.AreEqual(0, mapData.Transparency.Value);
            Assert.AreEqual(value, mapData.TileSource);
        }

        [Test]
        public void Constructor_ForAllWellKnownTileSources_ProperlySetName()
        {
            // Setup
            foreach (WellKnownTileSource wellKnownTileSource in Enum.GetValues(typeof(WellKnownTileSource)))
            {
                // Call
                var mapData = new WellKnownTileSourceMapData(wellKnownTileSource);

                // Assert
                Assert.AreEqual(TypeUtils.GetDisplayName(wellKnownTileSource), mapData.Name);
                Assert.AreEqual(wellKnownTileSource, mapData.TileSource);
            }
        }

        [Test]
        public void Constructor_UnknownTileSource_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => new WellKnownTileSourceMapData((WellKnownTileSource) 99);

            // Assert
            const string message = "The value of argument 'enumValue' (99) is invalid for Enum type 'WellKnownTileSource'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        public void SetTileSource_UnknownTileSource_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);

            // Call
            TestDelegate test = () => mapData.SetTileSource((WellKnownTileSource) 99);

            // Assert
            const string message = "The value of argument 'enumValue' (99) is invalid for Enum type 'WellKnownTileSource'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        public void SetTileSource_ForAllWellKnownTileSources_ProperlySetName()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);

            foreach (WellKnownTileSource wellKnownTileSource in Enum.GetValues(typeof(WellKnownTileSource)))
            {
                // Call
                mapData.SetTileSource(wellKnownTileSource);

                // Assert
                Assert.AreEqual(TypeUtils.GetDisplayName(wellKnownTileSource), mapData.Name);
                Assert.AreEqual(wellKnownTileSource, mapData.TileSource);
            }
        }
    }
}