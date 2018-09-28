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
using System.Drawing;
using System.IO;
using BruTile;
using BruTile.Web;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.BruTile.TestUtil.Test
{
    [TestFixture]
    public class TestWellKnownTileSourceTest
    {
        [Test]
        public void Constructor_NullWellKnownMapData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestWellKnownTileSource(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("mapData", paramName);
        }

        [Test]
        public void Constructor_WellKnownMapData_ExpectedValues()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(new Random(6754).NextEnumValue<WellKnownTileSource>());

            // Call
            var source = new TestWellKnownTileSource(mapData);

            // Assert
            Assert.IsInstanceOf<HttpTileSource>(source);
            Assert.AreEqual("Stub schema", source.Name);
            Assert.IsNotNull(source.Attribution);
            Assert.IsNotNull(source.PersistentCache);

            ITileSchema wellKnownSchema = source.Schema;
            Assert.AreEqual(mapData.Name, wellKnownSchema.Name);
            Assert.AreEqual("png", wellKnownSchema.Format);
            Assert.AreEqual(YAxis.TMS, wellKnownSchema.YAxis);
        }

        [Test]
        public void GetTile_ForAnyTileInfo_ReturnsStubTileData()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(new Random(6754).NextEnumValue<WellKnownTileSource>());
            var source = new TestWellKnownTileSource(mapData);

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
            var mapData = new WellKnownTileSourceMapData(new Random(6754).NextEnumValue<WellKnownTileSource>());
            var source = new TestWellKnownTileSource(mapData);

            // Call
            Uri url = source.GetUri(new TileInfo());

            // Assert
            Assert.AreEqual(@"https://www.stub.org/tiles/someTile.png", url.ToString());
        }
    }
}