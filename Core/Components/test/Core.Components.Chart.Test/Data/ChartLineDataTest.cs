﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Data
{
    [TestFixture]
    public class ChartLineDataTest
    {
        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Call
            var data = new ChartLineData("test data");

            // Assert
            Assert.AreEqual("test data", data.Name);
            CollectionAssert.IsEmpty(data.Points);
            Assert.IsInstanceOf<PointBasedChartData>(data);
            Assert.AreEqual(Color.Black, data.Style.Color);
            Assert.AreEqual(2, data.Style.Width);
            Assert.AreEqual(ChartLineDashStyle.Solid, data.Style.DashStyle);
        }

        [Test]
        public void Constructor_Always_CreatesNewInstanceOfDefaultStyle()
        {
            // Setup
            var dataA = new ChartLineData("test data");

            // Call
            var dataB = new ChartLineData("test data");

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
            TestDelegate test = () => new ChartLineData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the chart data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
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
            var style = new ChartLineStyle
            {
                Color = Color.Red,
                Width = 3,
                DashStyle = ChartLineDashStyle.DashDot
            };

            // Call
            var data = new ChartLineData("test data", style);

            // Assert
            Assert.AreEqual("test data", data.Name);
            CollectionAssert.IsEmpty(data.Points);
            Assert.IsInstanceOf<PointBasedChartData>(data);
            Assert.AreSame(style, data.Style);
        }
    }
}