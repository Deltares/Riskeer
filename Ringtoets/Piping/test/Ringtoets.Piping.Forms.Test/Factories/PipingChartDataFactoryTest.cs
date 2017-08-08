// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base.Geometry;
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
        public void CreateEntryPointChartData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = PipingChartDataFactory.CreateEntryPointChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Intredepunt", data.Name);
            AssertEqualStyle(data.Style, Color.Gold, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateExitPointChartData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = PipingChartDataFactory.CreateExitPointChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Uittredepunt", data.Name);
            AssertEqualStyle(data.Style, Color.Tomato, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateSoilLayerChartData_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingChartDataFactory.CreateSoilLayerChartData(0, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilProfile", paramName);
        }

        [Test]
        [TestCase("A", 0)]
        [TestCase("B", 3)]
        [TestCase("Random", 5)]
        public void CreateSoilLayerChartData_ValidSoilProfileAndSoilLayerIndex_ReturnsEmptyChartDataCollectionWithDefaultStyling(string name, int soilLayerIndex)
        {
            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var layers = new List<PipingSoilLayer>();
            for (var i = 0; i < soilLayerIndex; i++)
            {
                layers.Add(new PipingSoilLayer((double) i / 10));
            }
            layers.Add(new PipingSoilLayer(-1.0)
            {
                MaterialName = name,
                Color = Color.Aquamarine
            });

            var profile = new PipingSoilProfile("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartMultipleAreaData data = PipingChartDataFactory.CreateSoilLayerChartData(soilLayerIndex, profile);

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual($"{soilLayerIndex + 1} {name}", data.Name);
            AssertEqualStyle(data.Style, Color.Aquamarine, Color.Black, 1);
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
            var surfaceLine = new PipingSurfaceLine
            {
                Name = "surface line name"
            };

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
                                                    }, SoilProfileType.SoilProfile1D, 0);

            // Call
            PipingChartDataFactory.UpdateSoilProfileChartDataName(chartData, soilProfile);

            // Assert
            Assert.AreEqual("soil profile name", chartData.Name);
        }

        [TestCase(-1)]
        [TestCase(2)]
        [TestCase(3)]
        public void CreateSoilLayerChartData_InvalidSoilLayerIndex_ThrowsArgumentOutOfRangeException(int soilLayerIndex)
        {
            // Setup
            var layers = new[]
            {
                new PipingSoilLayer(0),
                new PipingSoilLayer(1)
            };
            var profile = new PipingSoilProfile("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => PipingChartDataFactory.CreateSoilLayerChartData(soilLayerIndex, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentOutOfRangeException>(test).ParamName;
            Assert.AreEqual("soilLayerIndex", paramName);
        }

        private static void AssertEqualStyle(ChartPointStyle pointStyle, Color color, int size, Color strokeColor, int strokeThickness, ChartPointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }

        private static void AssertEqualStyle(ChartAreaStyle areaStyle, Color fillColor, Color strokeColor, int width)
        {
            Assert.AreEqual(fillColor, areaStyle.FillColor);
            Assert.AreEqual(strokeColor, areaStyle.StrokeColor);
            Assert.AreEqual(width, areaStyle.StrokeThickness);
        }
    }
}