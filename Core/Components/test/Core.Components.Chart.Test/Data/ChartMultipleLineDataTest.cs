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
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Data
{
    [TestFixture]
    public class ChartMultipleLineDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var data = new ChartMultipleLineData("test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            CollectionAssert.IsEmpty(data.Lines);
            Assert.AreEqual(Color.Black, data.Style.Color);
            Assert.AreEqual(2, data.Style.Width);
            Assert.AreEqual(ChartLineDashStyle.Solid, data.Style.DashStyle);
            Assert.IsFalse(data.Style.IsEditable);
        }

        [Test]
        public void Constructor_Always_CreatesNewInstanceOfDefaultStyle()
        {
            // Setup
            var dataA = new ChartMultipleLineData("test data");

            // Call
            var dataB = new ChartMultipleLineData("test data");

            // Assert
            Assert.AreNotSame(dataA.Style, dataB.Style);
        }

        [Test]
        public void Constructor_StyleNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartLineData("test data", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("style", exception.ParamName);
        }

        [Test]
        public void Constructor_WithStyle_ExpectedValue()
        {
            // Setup
            var style = new ChartLineStyle();

            // Call
            var data = new ChartMultipleLineData("test data", style);

            // Assert
            Assert.AreEqual("test data", data.Name);
            CollectionAssert.IsEmpty(data.Lines);
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreSame(style, data.Style);
        }

        [Test]
        public void Lines_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new ChartMultipleLineData("test data");
            var lines = new List<Point2D[]>();

            // Call
            data.Lines = lines;

            // Assert
            Assert.AreSame(lines, data.Lines);
        }

        [Test]
        public void Lines_SetNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new ChartMultipleLineData("test data");

            // Call
            TestDelegate test = () => data.Lines = null;

            // Assert
            const string expectedMessage = "The collection of point arrays cannot be null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Lines_SetValueContainingNullValue_ThrowsArgumentException()
        {
            // Setup
            var data = new ChartMultipleLineData("test data");

            // Call
            TestDelegate test = () => data.Lines = new List<Point2D[]>
            {
                null
            };

            // Assert
            const string expectedMessage = "The collection of point arrays cannot contain null values.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}