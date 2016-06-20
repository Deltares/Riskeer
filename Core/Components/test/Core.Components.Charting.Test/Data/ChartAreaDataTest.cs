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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartAreaDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartAreaData(null, "test data");

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Setup
            var points = new Collection<Point2D>();

            // Call
            TestDelegate test = () => new ChartAreaData(points, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to chart data");
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewICharData()
        {
            // Setup
            var points = new Collection<Point2D>();

            // Call
            var data = new ChartAreaData(points, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(points, data.Points);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new ChartAreaData(points, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
        }

        private Collection<Point2D> CreateTestPoints()
        {
            return new Collection<Point2D>
            {
                new Point2D(0.0, 1.1),
                new Point2D(1.0, 2.1),
                new Point2D(1.6, 1.6)    
            };
        }
    }
} 