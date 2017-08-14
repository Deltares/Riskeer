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
        public void CreateForeshoreGeometryChartData_ReturnsChartDataWithEditableStyling()
        {
            // Call
            ChartLineData data = RingtoetsChartDataFactory.CreateForeshoreGeometryChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Voorlandprofiel", data.Name);
            AssertEqualStyle(data.Style, Color.DarkOrange, 2, ChartLineDashStyle.Solid, true);
        }

        [Test]
        public void CreateSurfaceLineChartData_ReturnsChartLineDataWithEditableStyling()
        {
            // Call
            ChartLineData data = RingtoetsChartDataFactory.CreateSurfaceLineChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Profielschematisatie", data.Name);
            AssertEqualStyle(data.Style, Color.Sienna, 2, ChartLineDashStyle.Solid, true);
        }

        [Test]
        public void CreateSoilProfileChartData_ReturnsChartDataCollectionWithDefaultStyling()
        {
            // Call
            ChartDataCollection data = RingtoetsChartDataFactory.CreateSoilProfileChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Ondergrondschematisatie", data.Name);
        }

        [Test]
        public void CreateDitchPolderSideChartData_ReturnsChartPointDataWithEditableStyling()
        {
            // Call
            ChartPointData data = RingtoetsChartDataFactory.CreateDitchPolderSideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Insteek sloot polderzijde", data.Name);
            AssertEqualStyle(data.Style, Color.IndianRed, 8, Color.Transparent, 0, ChartPointSymbol.Circle, true);
        }

        [Test]
        public void CreateBottomDitchPolderSideChartData_ReturnsChartPointDataWithEditableStyling()
        {
            // Call
            ChartPointData data = RingtoetsChartDataFactory.CreateBottomDitchPolderSideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Slootbodem polderzijde", data.Name);
            AssertEqualStyle(data.Style, Color.Teal, 8, Color.Transparent, 0, ChartPointSymbol.Circle, true);
        }

        [Test]
        public void CreateBottomDitchDikeSideChartData_ReturnsChartPointDataWithEditableStyling()
        {
            // Call
            ChartPointData data = RingtoetsChartDataFactory.CreateBottomDitchDikeSideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Slootbodem dijkzijde", data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 8, Color.Transparent, 0, ChartPointSymbol.Circle, true);
        }

        [Test]
        public void CreateDitchDikeSideChartData_ReturnsChartPointDataWithEditableStyling()
        {
            // Call
            ChartPointData data = RingtoetsChartDataFactory.CreateDitchDikeSideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Insteek sloot dijkzijde", data.Name);
            AssertEqualStyle(data.Style, Color.MediumPurple, 8, Color.Transparent, 0, ChartPointSymbol.Circle, true);
        }

        [Test]
        public void CreateDikeToeAtPolderChartData_ReturnsChartPointDataWithEditableStyling()
        {
            // Call
            ChartPointData data = RingtoetsChartDataFactory.CreateDikeToeAtPolderChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Teen dijk binnenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.LightGray, 8, Color.Black, 1, ChartPointSymbol.Square, true);
        }

        [Test]
        public void CreateDikeToeAtRiverChartData_ReturnsChartPointDataWithEditableStyling()
        {
            // Call
            ChartPointData data = RingtoetsChartDataFactory.CreateDikeToeAtRiverChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Teen dijk buitenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.DarkGray, 8, Color.Black, 1, ChartPointSymbol.Square, true);
        }

        private static void AssertEqualStyle(ChartPointStyle pointStyle, Color fillColor, int size,
                                             Color strokeColor, int strokeThickness, ChartPointSymbol symbol, bool isEditable)
        {
            Assert.AreEqual(fillColor, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
            Assert.AreEqual(isEditable, pointStyle.IsEditable);
        }

        private static void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, ChartLineDashStyle style, bool isEditable)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
            Assert.AreEqual(isEditable, lineStyle.IsEditable);
        }
    }
}