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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Data
{
    [TestFixture]
    public class PointBasedChartDataTest
    {
        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Call
            var data = new TestPointBasedChartData("test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreEqual("test data", data.Name);
            CollectionAssert.IsEmpty(data.Points);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new TestPointBasedChartData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the chart data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Points_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new TestPointBasedChartData("test data");
            var points = new[]
            {
                new Point2D(0.0, 1.0),
                new Point2D(2.5, 1.1)
            };

            // Call
            data.Points = points;

            // Assert
            Assert.AreSame(points, data.Points);
        }

        [Test]
        public void Points_SetNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new TestPointBasedChartData("test data");

            // Call
            TestDelegate test = () => data.Points = null;

            // Assert
            const string expectedMessage = "The array of points cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        private class TestPointBasedChartData : PointBasedChartData
        {
            public TestPointBasedChartData(string name) : base(name) {}
        }
    }
}