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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartMultipleAreaDataTest
    {
        [Test]
        public void Constructor_NullAreas_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartMultipleAreaData(null, "test data");

            // Assert
            var expectedMessage = "A collection of areas is required when creating ChartMultipleAreaData.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_NullAreaInAreas_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartMultipleAreaData(new Collection<Collection<Point2D>> { null }, "test data");

            // Asserrt
            var expectedMessage = "Every area in the collection needs a value when creating ChartMultipleAreaData.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Setup
            var areas = new Collection<Collection<Point2D>>();

            // Call
            TestDelegate test = () => new ChartMultipleAreaData(areas, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to chart data");
        }

        [Test]
        public void Constructor_WithEmptyAreas_CreatesNewICharData()
        {
            // Setup
            var areas = new Collection<Collection<Point2D>>();

            // Call
            var data = new ChartMultipleAreaData(areas, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(areas, data.Areas);
            Assert.IsTrue(data.IsVisible);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var areas = CreateTestAreas();

            // Call
            var data = new ChartMultipleAreaData(areas, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(areas, data.Areas);
            for (int i = 0; i < areas.Count; i++)
            {
                Assert.AreNotSame(areas[i], data.Areas.ElementAt(i));
                Assert.AreEqual(areas[i], data.Areas.ElementAt(i));
            }
            CollectionAssert.AreEqual(areas, data.Areas);
        }

        private Collection<Collection<Point2D>> CreateTestAreas()
        {
            return new Collection<Collection<Point2D>>
            {
                new Collection<Point2D>
                {
                    new Point2D(0.0, 1.1),
                    new Point2D(1.0, 2.1),
                    new Point2D(1.6, 1.6)
                },
                new Collection<Point2D>
                {
                    new Point2D(0.4, 1.1),
                    new Point2D(1.6, 2.2),
                    new Point2D(1.2, 4.6)

                }
            };
        }
    }
}