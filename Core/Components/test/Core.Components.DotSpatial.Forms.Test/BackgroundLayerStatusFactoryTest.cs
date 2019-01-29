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
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using DotSpatial.Symbology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class BackgroundLayerStatusFactoryTest
    {
        [Test]
        public void CreateBackgroundLayerStatus_MapDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => BackgroundLayerStatusFactory.CreateBackgroundLayerStatus(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("mapData", paramName);
        }

        [Test]
        public void CreateBackgroundLayerStatus_InvalidImageBasedMapData_ThrowsNotSupportedException()
        {
            // Setup
            var mapData = new SimpleImageBasedMapData();

            // Call
            TestDelegate test = () => BackgroundLayerStatusFactory.CreateBackgroundLayerStatus(mapData);

            // Assert
            string message = Assert.Throws<NotSupportedException>(test).Message;
            Assert.AreEqual("Unsupported type of mapData", message);
        }

        [Test]
        public void CreateBackgroundLayerStatus_WmtsMapData_ReturnsWmtsBackgroundLayerStatus()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            // Call
            BackgroundLayerStatus backgroundLayerStatus = BackgroundLayerStatusFactory.CreateBackgroundLayerStatus(mapData);

            // Assert
            Assert.IsInstanceOf<WmtsBackgroundLayerStatus>(backgroundLayerStatus);
        }

        [Test]
        public void CreateBackgroundLayerStatus_WellKnownTileSourceMapData_ReturnsWellKnownBackgroundLayerStatus()
        {
            // Setup
            var mapData = new WellKnownTileSourceMapData(new Random().NextEnum<WellKnownTileSource>());

            // Call
            BackgroundLayerStatus backgroundLayerStatus = BackgroundLayerStatusFactory.CreateBackgroundLayerStatus(mapData);

            // Assert
            Assert.IsInstanceOf<WellKnownBackgroundLayerStatus>(backgroundLayerStatus);
        }

        private class SimpleImageBasedMapData : ImageBasedMapData
        {
            public SimpleImageBasedMapData() : base("SimpleImageBasedMapData") {}
        }
    }
}