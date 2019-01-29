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
using System.Drawing;
using System.IO;
using BruTile;
using BruTile.Web;
using BruTile.Wmts;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.BruTile.TestUtil.Test
{
    [TestFixture]
    public class TestWmtsTileSourceTest
    {
        [Test]
        public void Constructor_ConfiguredWmtsMapData_ExpectedValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            // Call
            var source = new TestWmtsTileSource(mapData);

            // Assert
            Assert.IsInstanceOf<HttpTileSource>(source);
            Assert.AreEqual("Stub schema", source.Name);
            Assert.IsNotNull(source.Attribution);
            Assert.IsNotNull(source.PersistentCache);

            var wmtsSchema = (WmtsTileSchema) source.Schema;
            Assert.AreEqual(mapData.Name, wmtsSchema.Title);
            Assert.AreEqual(mapData.PreferredFormat, wmtsSchema.Format);
        }

        [Test]
        public void Constructor_UnconfiguredWmtsMapData_ThrowArgumentException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => new TestWmtsTileSource(mapData);

            // Assert
            const string message = "Only configured WmtsMapData instances can be used to create a schema for.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("mapData", paramName);
        }

        [Test]
        public void GetTile_ForAnyTileInfo_ReturnsStubTileData()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            var source = new TestWmtsTileSource(mapData);

            // Call
            byte[] tileData = source.GetTile(new TileInfo());

            // Assert
            using (var stream = new MemoryStream(tileData))
            using (var tileImage = new Bitmap(stream))
            {
                Assert.AreEqual(256, tileImage.Width);
                Assert.AreEqual(256, tileImage.Height);
            }
        }

        [Test]
        public void GetUri_ForAnyTileInfo_ReturnStubUrl()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            var source = new TestWmtsTileSource(mapData);

            // Call
            Uri url = source.GetUri(new TileInfo());

            // Assert
            Assert.AreEqual(@"https://www.stub.org/", url.ToString());
        }
    }
}