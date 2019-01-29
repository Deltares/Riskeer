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
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Features;
using Core.Components.Gis.Style;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data.Removable
{
    [TestFixture]
    public class RemovableMapDataConverterTest
    {
        [Test]
        public void FromFeatureBasedMapData_WithMapPointData_ReturnsNewRemovableDataWithSameProperties()
        {
            // Setup
            const string name = "test";
            var mapFeatures = new MapFeature[0];
            Color color = Color.AliceBlue;
            var pointStyle = new PointStyle
            {
                Color = color,
                Size = 3,
                Symbol = PointSymbol.Diamond,
                StrokeColor = color,
                StrokeThickness = 1
            };

            var mapData = new MapPointData(name, pointStyle)
            {
                Features = mapFeatures
            };

            // Call
            var convertedData = RemovableMapDataConverter.FromFeatureBasedMapData(mapData) as RemovableMapPointData;

            // Assert
            Assert.NotNull(convertedData);
            Assert.AreEqual(name, convertedData.Name);
            Assert.AreEqual(pointStyle, convertedData.Style);
            Assert.AreSame(mapFeatures, convertedData.Features);
        }

        [Test]
        public void FromFeatureBasedMapData_WithMapLineData_ReturnsNewRemovableDataWithSameProperties()
        {
            // Setup
            const string name = "test";
            var mapFeatures = new MapFeature[0];
            var lineStyle = new LineStyle
            {
                Color = Color.AliceBlue,
                Width = 3,
                DashStyle = LineDashStyle.Dash
            };

            var mapData = new MapLineData(name, lineStyle)
            {
                Features = mapFeatures
            };

            // Call
            var convertedData = RemovableMapDataConverter.FromFeatureBasedMapData(mapData) as RemovableMapLineData;

            // Assert
            Assert.NotNull(convertedData);
            Assert.AreEqual(name, convertedData.Name);
            Assert.AreEqual(lineStyle, convertedData.Style);
            Assert.AreSame(mapFeatures, convertedData.Features);
        }

        [Test]
        public void FromFeatureBasedMapData_WithMapPolygonData_ReturnsNewRemovableDataWithSameProperties()
        {
            // Setup
            const string name = "test";
            var mapFeatures = new MapFeature[0];
            var polygonStyle = new PolygonStyle
            {
                FillColor = Color.AliceBlue,
                StrokeColor = Color.DeepSkyBlue,
                StrokeThickness = 3
            };

            var mapData = new MapPolygonData(name, polygonStyle)
            {
                Features = mapFeatures
            };

            // Call
            var convertedData = RemovableMapDataConverter.FromFeatureBasedMapData(mapData) as RemovableMapPolygonData;

            // Assert
            Assert.NotNull(convertedData);
            Assert.AreEqual(name, convertedData.Name);
            Assert.AreEqual(polygonStyle, convertedData.Style);
            Assert.AreSame(mapFeatures, convertedData.Features);
        }

        [Test]
        public void FromFeatureBasedMapData_WithUnsupportedFeatureBasedMapData_ThrowsNotSupportedException()
        {
            // Setup
            var testData = new TestFeatureBasedMapData("test");

            // Call
            TestDelegate test = () => RemovableMapDataConverter.FromFeatureBasedMapData(testData);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(test);
            Assert.AreEqual("The given mapData was not convertible to IRemovable data.", exception.Message);
        }

        [Test]
        public void FromFeatureBasedMapData_WithoutData_ThrowsNotSupportedException()
        {
            // Call
            TestDelegate test = () => RemovableMapDataConverter.FromFeatureBasedMapData(null);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }
    }
}