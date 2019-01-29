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
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.BruTile.TestUtil.Test
{
    [TestFixture]
    public class UseCustomTileSourceFactoryConfigTest
    {
        [Test]
        public void GivenTileSourceFactoryInstance_WhenConfigForFactory_ThenSingletonInstanceTemporarilyChanged()
        {
            // Given
            ITileSourceFactory originalFactory = TileSourceFactory.Instance;

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            mocks.ReplayAll();

            // When
            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                // Then
                Assert.AreSame(factory, TileSourceFactory.Instance);
            }

            Assert.AreSame(originalFactory, TileSourceFactory.Instance);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenTileSourceFactoryInstance_WhenConfigForWmtsMapData_ThenSingletonInstanceTemporarilyChanged()
        {
            // Given
            ITileSourceFactory originalFactory = TileSourceFactory.Instance;

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            // When
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                // Then
                Assert.IsInstanceOf<TestTileSourceFactory>(TileSourceFactory.Instance);
            }

            Assert.AreSame(originalFactory, TileSourceFactory.Instance);
        }

        [Test]
        public void GivenTileSourceFactoryInstance_WhenConfigForWellKnownTileSourceMapData_ThenSingletonInstanceTemporarilyChanged()
        {
            // Given
            ITileSourceFactory originalFactory = TileSourceFactory.Instance;

            var mapData = new WellKnownTileSourceMapData(new Random(341).NextEnumValue<WellKnownTileSource>());

            // When
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                // Then
                Assert.IsInstanceOf<TestTileSourceFactory>(TileSourceFactory.Instance);
            }

            Assert.AreSame(originalFactory, TileSourceFactory.Instance);
        }
    }
}