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
using System.Collections.Generic;
using System.Linq;
using BruTile;
using BruTile.Predefined;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.BruTile.TestUtil.Test
{
    [TestFixture]
    public class TestTileSourceFactoryTest
    {
        [Test]
        public void Constructor_ForWmtsMapData_ExpectedValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            var factory = new TestTileSourceFactory(mapData);

            // Assert
            Assert.IsInstanceOf<ITileSourceFactory>(factory);
        }

        [Test]
        public void GetWmtsTileSources_FromUninitializedWmtsMapData_ReturnEmpty()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Precondition
            Assert.IsFalse(mapData.IsConfigured);

            var factory = new TestTileSourceFactory(mapData);

            // Call
            IEnumerable<ITileSource> tileSources = factory.GetWmtsTileSources(null);

            // Assert
            CollectionAssert.IsEmpty(tileSources);
        }

        [Test]
        public void GetWmtsTileSources_FromConfiguredWmtsMapData_ReturnTileSource()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            // Precondition
            Assert.IsTrue(mapData.IsConfigured);

            var factory = new TestTileSourceFactory(mapData);

            // Call
            ITileSource[] tileSources = factory.GetWmtsTileSources(null).ToArray();

            // Assert
            Assert.AreEqual(1, tileSources.Length);
            ITileSource tileSource = tileSources[0];
            Assert.IsInstanceOf<TestWmtsTileSource>(tileSource);
            Assert.AreEqual(mapData.PreferredFormat, tileSource.Schema.Format);
        }

        [Test]
        public void Constructor_ForWellKnownTileSourceMapData_ExpectedValues()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(new Random(341).NextEnumValue<WellKnownTileSource>());

            // Call
            var factory = new TestTileSourceFactory(mapData);

            // Assert
            Assert.IsInstanceOf<ITileSourceFactory>(factory);
        }

        [Test]
        public void GetWellKnownTileSources_NotInitialized_ThrowsNotSupportedException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();
            var factory = new TestTileSourceFactory(mapData);

            // Call
            TestDelegate test = () => factory.GetKnownTileSource(KnownTileSource.BingHybrid);

            // Assert
            string message = Assert.Throws<NotSupportedException>(test).Message;
            Assert.AreEqual("use TestTileSourceFactory() to set the expected well known tile source", message);
        }

        [Test]
        public void GetWellKnownTileSources_FromConfiguredWellKnownTileSourceMapData_ReturnTileSource()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(new Random(341).NextEnumValue<WellKnownTileSource>());

            // Precondition
            Assert.IsTrue(mapData.IsConfigured);

            var factory = new TestTileSourceFactory(mapData);

            // Call
            ITileSource tileSource = factory.GetKnownTileSource(new Random(341).NextEnumValue<KnownTileSource>());

            // Assert
            Assert.IsInstanceOf<TestWellKnownTileSource>(tileSource);
        }
    }
}