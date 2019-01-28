// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;
using Ringtoets.Piping.Forms.Factories;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Factories
{
    [TestFixture]
    public class PipingChartDataFactoryTest
    {
        [Test]
        public void CreateEntryPointChartData_ReturnsEmptyChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = PipingChartDataFactory.CreateEntryPointChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Intredepunt", data.Name);
            AssertEqualStyle(data.Style, Color.Gold, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateExitPointChartData_ReturnsEmptyChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = PipingChartDataFactory.CreateExitPointChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Uittredepunt", data.Name);
            AssertEqualStyle(data.Style, Color.Tomato, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateSoilLayerChartData_LayerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateSoilLayerChartData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("layer", exception.ParamName);
        }

        [Test]
        public void CreateSoilLayerChartData_WithLayer_ReturnsEmptyChartDataCollectionWithExpectedStyling()
        {
            // Setup
            const string name = "Soil layer test name";
            Color fillColor = Color.Firebrick;

            // Call
            ChartMultipleAreaData data = PipingChartDataFactory.CreateSoilLayerChartData(new PipingSoilLayer(0)
            {
                MaterialName = name,
                Color = fillColor
            });

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual(name, data.Name);
            AssertEqualStyle(data.Style, fillColor, Color.Black, 1);
        }

        [Test]
        public void CreateSoilLayerChartData_LayerWithEmptyNameAndColor_ReturnsEmptyChartDataCollectionWithExpectedStyling()
        {
            // Call
            ChartMultipleAreaData data = PipingChartDataFactory.CreateSoilLayerChartData(new PipingSoilLayer(0)
            {
                MaterialName = string.Empty,
                Color = Color.Empty
            });

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Onbekend", data.Name);
            AssertEqualStyle(data.Style, Color.White, Color.Black, 1);
        }

        [Test]
        public void UpdateSurfaceLineChartDataName_SurfaceLineNull_NameSetToDefaultSurfaceLineName()
        {
            // Setup
            var chartData = new ChartLineData("test name");

            // Call
            PipingChartDataFactory.UpdateSurfaceLineChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Profielschematisatie", chartData.Name);
        }

        [Test]
        public void UpdateSurfaceLineChartDataName_SurfaceLine_NameSetToSurfaceLineName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var surfaceLine = new PipingSurfaceLine("surface line name");

            // Call
            PipingChartDataFactory.UpdateSurfaceLineChartDataName(chartData, surfaceLine);

            // Assert
            Assert.AreEqual("surface line name", chartData.Name);
        }

        [Test]
        public void UpdateSoilProfileChartDataName_WithoutSoilProfile_NameSetToDefaultSoilProfileName()
        {
            // Setup
            var chartData = new ChartDataCollection("test name");

            // Call
            PipingChartDataFactory.UpdateSoilProfileChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Ondergrondschematisatie", chartData.Name);
        }

        [Test]
        public void UpdateSoilProfileChartDataName_WithSoilProfile_NameSetToSoilProfileName()
        {
            // Setup
            var chartData = new ChartDataCollection("test name");
            var soilProfile = new PipingSoilProfile("soil profile name", 2.0,
                                                    new[]
                                                    {
                                                        new PipingSoilLayer(3.2)
                                                    }, SoilProfileType.SoilProfile1D);

            // Call
            PipingChartDataFactory.UpdateSoilProfileChartDataName(chartData, soilProfile);

            // Assert
            Assert.AreEqual("soil profile name", chartData.Name);
        }

        private static void AssertEqualStyle(ChartPointStyle pointStyle, Color color, int size, Color strokeColor, int strokeThickness, ChartPointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
            Assert.IsTrue(pointStyle.IsEditable);
        }

        private static void AssertEqualStyle(ChartAreaStyle areaStyle, Color fillColor, Color strokeColor, int width)
        {
            Assert.AreEqual(fillColor, areaStyle.FillColor);
            Assert.AreEqual(strokeColor, areaStyle.StrokeColor);
            Assert.AreEqual(width, areaStyle.StrokeThickness);
            Assert.IsFalse(areaStyle.IsEditable);
        }
    }
}