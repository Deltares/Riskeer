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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapPointDataTest
    {
        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Setup
            const string name = "test data";

            // Call
            var data = new MapPointData(name);

            // Assert
            Assert.AreEqual(name, data.Name);
            CollectionAssert.IsEmpty(data.Features);
            Assert.IsInstanceOf<FeatureBasedMapData>(data);
            Assert.AreEqual(Color.Black, data.Style.Color);
            Assert.AreEqual(2, data.Style.Size);
            Assert.AreEqual(PointSymbol.Square, data.Style.Symbol);
            Assert.AreEqual(Color.Black, data.Style.StrokeColor);
            Assert.AreEqual(1, data.Style.StrokeThickness);
            Assert.IsNull(data.Theme);
        }

        [Test]
        public void Constructor_Always_CreatesNewInstanceOfDefaultStyle()
        {
            // Setup
            var dataA = new MapPointData("test data");

            // Call
            var dataB = new MapPointData("test data");

            // Assert
            Assert.AreNotSame(dataA.Style, dataB.Style);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new MapPointData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the map data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_StyleNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPointData("test data", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("style", exception.ParamName);
        }

        [Test]
        public void Constructor_WithStyle_ExpectedValues()
        {
            // Setup
            const string name = "test data";
            Color color = Color.Aqua;
            var style = new PointStyle
            {
                Color = color,
                Size = 3,
                Symbol = PointSymbol.Circle,
                StrokeColor = color,
                StrokeThickness = 1
            };

            // Call
            var data = new MapPointData(name, style);

            // Assert
            Assert.AreEqual(name, data.Name);
            CollectionAssert.IsEmpty(data.Features);
            Assert.IsInstanceOf<FeatureBasedMapData>(data);
            Assert.AreSame(style, data.Style);
            Assert.IsNull(data.Theme);
        }

        [Test]
        public void Features_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new MapPointData("test data");
            var features = new[]
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    }),
                    new MapGeometry(new[]
                    {
                        CreateTestPoints()
                    })
                })
            };

            // Call
            data.Features = features;

            // Assert
            Assert.AreSame(features, data.Features);
        }

        [Test]
        public void Features_SetNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new MapPointData("test data");

            // Call
            TestDelegate test = () => data.Features = null;

            // Assert
            const string expectedMessage = "The array of features cannot be null or contain null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Features_SetCollectionWithNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new MapPointData("test data");

            // Call
            TestDelegate test = () => data.Features = new MapFeature[]
            {
                null
            };

            // Assert
            const string expectedMessage = "The array of features cannot be null or contain null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(7)]
        [TestCase(null)]
        public void Features_SetInvalidValue_ThrowsArgumentException(int numberOfPointCollections)
        {
            // Setup
            var data = new MapPointData("test data");
            var invalidPointsCollections = new IEnumerable<Point2D>[numberOfPointCollections];

            for (var i = 0; i < numberOfPointCollections; i++)
            {
                invalidPointsCollections[i] = CreateTestPoints();
            }

            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(invalidPointsCollections)
                })
            };

            // Call
            TestDelegate test = () => data.Features = features;

            // Assert
            const string expectedMessage = "MapPointData only accepts MapFeature instances whose MapGeometries contain a single point-collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        private static Point2D[] CreateTestPoints()
        {
            return new[]
            {
                new Point2D(0.0, 1.1),
                new Point2D(1.0, 2.1),
                new Point2D(1.6, 1.6)
            };
        }
    }
}