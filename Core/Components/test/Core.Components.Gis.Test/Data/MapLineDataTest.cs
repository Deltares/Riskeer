﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapLineDataTest
    {
        [Test]
        public void Constructor_WithValidName_ExpectedValues()
        {
            // Setup
            const string name = "test data";

            // Call
            var data = new MapLineData(name);

            // Assert
            Assert.AreEqual(name, data.Name);
            CollectionAssert.IsEmpty(data.Features);
            Assert.IsInstanceOf<FeatureBasedMapData<LineCategoryTheme>>(data);
            Assert.AreEqual(Color.Black, data.Style.Color);
            Assert.AreEqual(2, data.Style.Width);
            Assert.AreEqual(LineDashStyle.Solid, data.Style.DashStyle);
            Assert.IsNull(data.Theme);
        }

        [Test]
        public void Constructor_Always_CreatesNewInstanceOfDefaultStyle()
        {
            // Setup
            var dataA = new MapLineData("test data");

            // Call
            var dataB = new MapLineData("test data");

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
            TestDelegate test = () => new MapLineData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the map data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_StyleNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLineData("test data", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("style", exception.ParamName);
        }

        [Test]
        public void Constructor_WithStyle_ExpectedValues()
        {
            // Setup
            const string name = "test data";
            var style = new LineStyle
            {
                Color = Color.Red,
                Width = 5,
                DashStyle = LineDashStyle.Dash
            };

            // Call
            var data = new MapLineData(name, style);

            // Assert
            Assert.AreEqual(name, data.Name);
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreSame(style, data.Style);
            Assert.IsNull(data.Theme);
        }

        [Test]
        public void Constructor_StyleNullAndWithMapTheme_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapLineData("test data",
                                                      null,
                                                      new MapTheme<LineCategoryTheme>("attribute", new[]
                                                      {
                                                          new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                                                                new LineStyle())
                                                      }));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("style", exception.ParamName);
        }

        [Test]
        public void Constructor_WithStyleAndMapTheme_ExpectedValues()
        {
            // Setup
            const string name = "test data";
            var style = new LineStyle
            {
                Color = Color.Red,
                Width = 5,
                DashStyle = LineDashStyle.Dash
            };

            var mapTheme = new MapTheme<LineCategoryTheme>("attribute", new[]
            {
                new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                      new LineStyle())
            });

            // Call
            var data = new MapLineData(name, style, mapTheme);

            // Assert
            Assert.AreEqual(name, data.Name);
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreSame(style, data.Style);
            Assert.AreSame(mapTheme, data.Theme);
        }

        [Test]
        public void Features_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new MapLineData("test data");
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
            var data = new MapLineData("test data");

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
            var data = new MapLineData("test data");

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
        public void Features_SetInvalidValue_ThrowsArgumentException(int numberOfPointCollections)
        {
            // Setup
            var data = new MapLineData("test data");
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
            const string expectedMessage = "MapLineData only accepts MapFeature instances whose MapGeometries contain a single point-collection.";
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