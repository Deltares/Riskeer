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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Components.OxyPlot.DataSeries;
using Core.Components.OxyPlot.DataSeries.Stack;
using Core.Components.Stack.Data;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.DataSeries.Stack
{
    [TestFixture]
    public class StackChartDataSeriesTest
    {
        [Test]
        public void Constructor_RowChartDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StackChartDataSeries(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("rowChartData", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            Color color = Color.Blue;
            var values = new List<double>
            {
                0.3,
                0.5,
                0.2
            };

            // Call
            var series = new StackChartDataSeries(new RowChartData("data", values, color));

            // Assert
            Assert.IsInstanceOf<ColumnSeries>(series);
            Assert.IsInstanceOf<IChartDataSeries>(series);
            Assert.IsTrue(series.IsStacked);
            Assert.AreEqual(1, series.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), series.FillColor);
            CollectionAssert.AreEqual(values, series.Items.Select(i => i.Value));
        }
    }
}