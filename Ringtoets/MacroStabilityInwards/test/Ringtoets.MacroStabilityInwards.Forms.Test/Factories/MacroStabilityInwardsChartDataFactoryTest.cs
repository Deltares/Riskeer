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
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsChartDataFactoryTest
    {
        [Test]
        public void CreateWaternetZonesExtremeChartDataCollection_ReturnsEmptyChartDataCollection()
        {
            // Call
            ChartDataCollection data = MacroStabilityInwardsChartDataFactory.CreateWaternetZonesExtremeChartDataCollection();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Zones extreem", data.Name);
        }

        [Test]
        public void CreateWaternetZonesDailyChartDataCollection_ReturnsEmptyChartDataCollection()
        {
            // Call
            ChartDataCollection data = MacroStabilityInwardsChartDataFactory.CreateWaternetZonesDailyChartDataCollection();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Zones dagelijks", data.Name);
        }

        [Test]
        public void CreateWaternetZoneChartData_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsChartDataFactory.CreateWaternetZoneChartData(null, new Random(21).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateWaternetZoneChartData_WithVisibility_ReturnsEmptyChartLineDataWithExpectedStylingAndVisibility(bool isVisible)
        {
            // Setup
            const string name = "zone";

            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsChartDataFactory.CreateWaternetZoneChartData(name, isVisible);

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual(name, data.Name);
            Assert.AreEqual(isVisible, data.IsVisible);
            AssertEqualStyle(data.Style, Color.FromArgb(60, Color.DeepSkyBlue), Color.Empty, 0, true);
        }

        [Test]
        public void CreatePhreaticLineChartData_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsChartDataFactory.CreatePhreaticLineChartData(null, new Random(21).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreatePhreaticLineChartData_WithVisibility_ReturnsEmptyChartLineDataWithExpectedStylingAndVisibility(bool isVisible)
        {
            // Setup
            const string name = "zone";

            // Call
            ChartLineData data = MacroStabilityInwardsChartDataFactory.CreatePhreaticLineChartData(name, isVisible);

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual(name, data.Name);
            Assert.AreEqual(isVisible, data.IsVisible);
            AssertEqualStyle(data.Style, Color.Blue, 2, ChartLineDashStyle.Solid);
        }

        [Test]
        public void CreateShoulderBaseInsideChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateShoulderBaseInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Insteek binnenberm", data.Name);
            AssertEqualStyle(data.Style, Color.BlueViolet, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateDikeTopAtPolderChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtPolderChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Kruin binnentalud", data.Name);
            AssertEqualStyle(data.Style, Color.LightSkyBlue, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateShoulderTopInsideChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateShoulderTopInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Kruin binnenberm", data.Name);
            AssertEqualStyle(data.Style, Color.DeepSkyBlue, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateSurfaceLevelInsideChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelInsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Maaiveld binnenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.ForestGreen, 8, Color.Black, 1, ChartPointSymbol.Square);
        }

        [Test]
        public void CreateSurfaceLevelOutsideChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelOutsideChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Maaiveld buitenwaarts", data.Name);
            AssertEqualStyle(data.Style, Color.LightSeaGreen, 8, Color.Black, 1, ChartPointSymbol.Square);
        }

        [Test]
        public void CreateDikeTopAtRiverChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtRiverChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Kruin buitentalud", data.Name);
            AssertEqualStyle(data.Style, Color.LightSteelBlue, 8, Color.SeaGreen, 1, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateRightGridChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateRightGridChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Rechter grid", data.Name);
            AssertEqualStyle(data.Style, Color.Black, 6, Color.Black, 2, ChartPointSymbol.Plus);
        }

        [Test]
        public void CreateLeftGridChartData_ReturnsChartPointDataWithExpectedStyling()
        {
            // Call
            ChartPointData data = MacroStabilityInwardsChartDataFactory.CreateLeftGridChartData();

            // Assert
            Assert.IsFalse(data.HasData);
            Assert.AreEqual("Linker grid", data.Name);
            AssertEqualStyle(data.Style, Color.Black, 6, Color.Black, 2, ChartPointSymbol.Plus);
        }

        [Test]
        public void CreateSoilLayerChartData_LayerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(null);

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

            MacroStabilityInwardsSoilLayer2D layer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();
            layer.Data.MaterialName = name;
            layer.Data.Color = fillColor;

            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(layer);

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual(name, data.Name);
            AssertEqualStyle(data.Style, fillColor, Color.Black, 1, false);
        }

        [Test]
        public void CreateSoilLayerChartData_LayerWithEmptyNameAndColor_ReturnsEmptyChartDataCollectionWithExpectedStyling()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();
            layer.Data.MaterialName = string.Empty;
            layer.Data.Color = Color.Empty;

            // Call
            ChartMultipleAreaData data = MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(layer);

            // Assert
            CollectionAssert.IsEmpty(data.Areas);
            Assert.AreEqual("Onbekend", data.Name);
            AssertEqualStyle(data.Style, Color.White, Color.Black, 1, false);
        }

        [Test]
        public void CreateSlipPlaneChartData_ReturnsChartLineData()
        {
            // Call
            ChartLineData data = MacroStabilityInwardsChartDataFactory.CreateSlipPlaneChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Glijvlak", data.Name);
            AssertEqualStyle(data.Style, Color.SaddleBrown, 3, ChartLineDashStyle.Solid);
        }

        [Test]
        public void CreateActiveCircleRadiusChartData_ReturnsChartLineData()
        {
            // Call
            ChartLineData data = MacroStabilityInwardsChartDataFactory.CreateActiveCircleRadiusChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Radius actieve cirkel", data.Name);
            AssertEqualStyle(data.Style, Color.Gray, 1, ChartLineDashStyle.Dash);
        }

        [Test]
        public void CreatePassiveCircleRadiusChartData_ReturnsChartLineData()
        {
            // Call
            ChartLineData data = MacroStabilityInwardsChartDataFactory.CreatePassiveCircleRadiusChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Points);
            Assert.AreEqual("Radius passieve cirkel", data.Name);
            AssertEqualStyle(data.Style, Color.Gray, 1, ChartLineDashStyle.Dash);
        }

        [Test]
        public void CreateTangentLinesChartData_ReturnsChartMultipleLineData()
        {
            // Call
            ChartMultipleLineData data = MacroStabilityInwardsChartDataFactory.CreateTangentLinesChartData();

            // Assert
            CollectionAssert.IsEmpty(data.Lines);
            Assert.AreEqual("Tangentlijnen", data.Name);
            AssertEqualStyle(data.Style, Color.Green, 1, ChartLineDashStyle.Dash);
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
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("surface line name");

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
                                                                     });

            // Call
            MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(chartData, soilProfile);

            // Assert
            Assert.AreEqual("soil profile name", chartData.Name);
        }

        private static void AssertEqualStyle(ChartPointStyle pointStyle, Color fillColor, int size, Color strokeColor, int strokeThickness, ChartPointSymbol symbol)
        {
            Assert.AreEqual(fillColor, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
            Assert.IsTrue(pointStyle.IsEditable);
        }

        private static void AssertEqualStyle(ChartAreaStyle areaStyle, Color fillColor, Color strokeColor, int width, bool isEditable)
        {
            Assert.AreEqual(fillColor, areaStyle.FillColor);
            Assert.AreEqual(strokeColor, areaStyle.StrokeColor);
            Assert.AreEqual(width, areaStyle.StrokeThickness);
            Assert.AreEqual(isEditable, areaStyle.IsEditable);
        }

        private static void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, ChartLineDashStyle dashStyle)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(dashStyle, lineStyle.DashStyle);
            Assert.IsTrue(lineStyle.IsEditable);
        }
    }
}