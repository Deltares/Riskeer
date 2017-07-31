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
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsChartDataFactoryTest
    {
        [Test]
        public void CreateShoulderBaseInsideChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateShoulderBaseInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Insteek binnenberm", data.Name);
            AssertEqualStyle(data.Style, Color.BlueViolet, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateDikeTopAtPolderChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtPolderChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Kruin binnentalud", data.Name);
            AssertEqualStyle(data.Style, Color.LightSkyBlue, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateShoulderTopInsideChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateShoulderTopInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Kruin binnenberm", data.Name);
            AssertEqualStyle(data.Style, Color.DeepSkyBlue, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateSurfaceLevelInsideChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Maaiveld binnenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.ForestGreen, 8, Color.Black, 1, ChartPointSymbol.Square);
        }

        [Test]
        public void CreateSurfaceLevelOutsideChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelOutsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Maaiveld buitenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.LightSeaGreen, 8, Color.Black, 1, ChartPointSymbol.Square);
        }

        [Test]
        public void CreateTrafficLoadInsideChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateTrafficLoadInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Verkeersbelasting kant binnenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.LightSlateGray, 8, Color.White, 1, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateTrafficLoadOutsideChartData_ReturnsChartPointDataWithDefaultStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateTrafficLoadOutsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Verkeersbelasting kant buitenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.DarkSlateGray, 8, Color.White, 1, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateSoilLayerChartData_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(0, null);

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
            var surfaceLine = new MacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var layers = new List<MacroStabilityInwardsSoilLayer1D>();
            for (var i = 0; i < soilLayerIndex; i++)
            {
                layers.Add(new MacroStabilityInwardsSoilLayer1D((double) i / 10));
            }
            layers.Add(new MacroStabilityInwardsSoilLayer1D(-1.0)
            {
                MaterialName = name,
                Color = Color.Aquamarine
            });

            var profile = new MacroStabilityInwardsSoilProfile1D("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(soilLayerIndex, profile);

            // Assert
            Assert.IsEmpty(data.Areas);
            Assert.AreEqual($"{soilLayerIndex + 1} {name}", data.Name);
            AssertEqualStyle(data.Style, Color.Aquamarine, Color.Black, 1);
        }

        [Test]
        public void UpdateSurfaceLineChartDataName_SurfaceLineNull_NameSetToDefaultSurfaceLineName()
        {
            // Setup
            var chartData = new ChartLineData("test name");

            // Call
            MacroStabilityInwardsChartDataFactory.UpdateSurfaceLineChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Profielschematisatie", chartData.Name);
        }

        [Test]
        public void UpdateSurfaceLineChartDataName_SurfaceLine_NameSetToSurfaceLineName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var surfaceLine = new MacroStabilityInwardsSurfaceLine
            {
                Name = "surface line name"
            };

            // Call
            MacroStabilityInwardsChartDataFactory.UpdateSurfaceLineChartDataName(chartData, surfaceLine);

            // Assert
            Assert.AreEqual("surface line name", chartData.Name);
        }

        [Test]
        public void UpdateSoilProfileChartDataName_WithoutSoilProfile_NameSetToDefaultSoilProfileName()
        {
            // Setup
            var chartData = new ChartDataCollection("test name");

            // Call
            MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Ondergrondschematisatie", chartData.Name);
        }

        [Test]
        public void UpdateSoilProfileChartDataName_WithSoilProfile_NameSetToSoilProfileName()
        {
            // Setup
            var chartData = new ChartDataCollection("test name");
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("soil profile name", 2.0,
                                                                     new[]
                                                                     {
                                                                         new MacroStabilityInwardsSoilLayer1D(3.2)
                                                                     }, SoilProfileType.SoilProfile1D, 0);

            // Call
            MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(chartData, soilProfile);

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
                new MacroStabilityInwardsSoilLayer1D(0),
                new MacroStabilityInwardsSoilLayer1D(1)
            };
            var profile = new MacroStabilityInwardsSoilProfile1D("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(soilLayerIndex, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentOutOfRangeException>(test).ParamName;
            Assert.AreEqual("soilLayerIndex", paramName);
        }

        private static void AssertEqualStyle(ChartPointStyle pointStyle, Color fillColor, int size,
                                             Color strokeColor, int strokeThickness, ChartPointSymbol symbol)
        {
            Assert.AreEqual(fillColor, pointStyle.Color);
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