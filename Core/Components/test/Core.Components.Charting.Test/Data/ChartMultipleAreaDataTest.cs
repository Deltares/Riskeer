// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Call
            var data = new ChartMultipleAreaData("test data");

            // Assert
            Assert.AreEqual("test data", data.Name);
            Assert.IsInstanceOf<ItemBasedChartData>(data);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new ChartMultipleAreaData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the chart data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Areas_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new ChartMultipleAreaData("test data");
            var areas = new List<Point2D[]>();

            // Call
            data.Areas = areas;

            // Assert
            Assert.AreSame(areas, data.Areas);
        }

        [Test]
        public void Areas_SetNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new ChartMultipleAreaData("test data");

            // Call
            TestDelegate test = () => data.Areas = null;

            // Assert
            const string expectedMessage = "The collection of point arrays cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Areas_SetValueContainingNullValue_ThrowsArgumentException()
        {
            // Setup
            var data = new ChartMultipleAreaData("test data");

            // Call
            TestDelegate test = () => data.Areas = new List<Point2D[]>
            {
                null
            };

            // Assert
            const string expectedMessage = "The collection of point arrays cannot contain null values.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}