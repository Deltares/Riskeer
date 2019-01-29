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
using BruTile;
using BruTile.Wmts;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.BruTile.TestUtil.Test
{
    [TestFixture]
    public class TileSchemaFactoryTest
    {
        [Test]
        public void CreateWmtsTileSchema_ForWmtsMapData_ReturnsWmtsTileSchema()
        {
            // Setup
            WmtsMapData wmtsMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            // Call
            WmtsTileSchema schema = TileSchemaFactory.CreateWmtsTileSchema(wmtsMapData);

            // Assert
            Assert.AreEqual(wmtsMapData.Name, schema.Title);
            Assert.AreEqual(wmtsMapData.PreferredFormat, schema.Format);
            Assert.AreEqual("brtachtergrondkaart", schema.Layer);
            Assert.AreEqual("EPSG:28992", schema.TileMatrixSet);
            Assert.AreEqual("EPSG:28992", schema.Srs);

            Assert.AreEqual(1, schema.Resolutions.Count);
            Resolution resolution = schema.Resolutions["1"];
            Assert.AreEqual("1", resolution.Id);
            Assert.AreEqual(1, resolution.UnitsPerPixel);
            Assert.AreEqual(256, resolution.TileWidth);
            Assert.AreEqual(256, resolution.TileHeight);
            Assert.AreEqual(0, resolution.Left);
            Assert.AreEqual(0, resolution.Top);
            Assert.AreEqual(0, resolution.MatrixHeight);
            Assert.AreEqual(0, resolution.MatrixWidth);
            Assert.AreEqual(0, resolution.ScaleDenominator);
        }

        [Test]
        public void CreateWmtsTileSchema_ForWmtsMapDataWithoutClearCoordinateSystem_ReturnsWmtsTileSchemaAtWgs84()
        {
            // Setup
            var mapData = new WmtsMapData("A", "B", "C(D)", "image/jpeg");

            // Call
            WmtsTileSchema schema = TileSchemaFactory.CreateWmtsTileSchema(mapData);

            // Assert
            Assert.AreEqual("A", schema.Title);
            Assert.AreEqual(mapData.PreferredFormat, schema.Format);
            Assert.AreEqual("C", schema.Layer);
            Assert.AreEqual("D", schema.TileMatrixSet);
            Assert.AreEqual("EPSG:3857", schema.Srs);

            Assert.AreEqual(1, schema.Resolutions.Count);
            Resolution resolution = schema.Resolutions["1"];
            Assert.AreEqual("1", resolution.Id);
            Assert.AreEqual(1, resolution.UnitsPerPixel);
            Assert.AreEqual(256, resolution.TileWidth);
            Assert.AreEqual(256, resolution.TileHeight);
            Assert.AreEqual(0, resolution.Left);
            Assert.AreEqual(0, resolution.Top);
            Assert.AreEqual(0, resolution.MatrixHeight);
            Assert.AreEqual(0, resolution.MatrixWidth);
            Assert.AreEqual(0, resolution.ScaleDenominator);
        }

        [Test]
        public void CreateWmtsTileSchema_ForUninitializedWmtsMapData_ThrowArgumentException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => TileSchemaFactory.CreateWmtsTileSchema(mapData);

            // Assert
            const string message = "Only configured WmtsMapData instances can be used to create a schema for.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("mapData", paramName);
        }
    }
}