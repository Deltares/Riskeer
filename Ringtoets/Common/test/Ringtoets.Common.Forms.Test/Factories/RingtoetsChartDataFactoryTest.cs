﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;
using Ringtoets.Common.Forms.Factories;

namespace Ringtoets.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RingtoetsChartDataFactoryTest
    {
        [Test]
        public void CreateForeshoreGeometryChartData_ReturnsChartDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = RingtoetsChartDataFactory.CreateForeshoreGeometryChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Voorlandprofiel", data.Name);
            AssertEqualStyle(data.Style, Color.DarkOrange, 2, ChartLineDashStyle.Solid);
        }

        [Test]
        public void CreateSurfaceLineChartData_ReturnsChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = RingtoetsChartDataFactory.CreateSurfaceLineChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Profielschematisatie", data.Name);
            AssertEqualStyle(data.Style, Color.Sienna, 2, ChartLineDashStyle.Solid);
        }

        [Test]
        public void CreateSoilProfileChartData_ReturnsChartDataCollectionWithDefaultStyling()
        {
            // Call
            ChartDataCollection data = RingtoetsChartDataFactory.CreateSoilProfileChartData();

            // Assert
            Assert.IsEmpty(data.Collection);
            Assert.AreEqual("Ondergrondschematisatie", data.Name);
        }

        private static void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, ChartLineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }
    }
}