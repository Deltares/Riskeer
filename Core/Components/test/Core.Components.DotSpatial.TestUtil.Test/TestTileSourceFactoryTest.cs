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

using System.Collections.Generic;
using System.Linq;
using BruTile;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.TestUtil.Test
{
    [TestFixture]
    public class TestTileSourceFactoryTest
    {
        [Test]
        public void Constructor_ForWmtsMapData_ExpectedValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            var factory = new TestTileSourceFactory(mapData);

            // Assert
            Assert.IsInstanceOf<ITileSourceFactory>(factory);
        }

        [Test]
        public void GetWmtsTileSources_FromUninitializedWmtsMapData_ReturnEmpty()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

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
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

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
    }
}