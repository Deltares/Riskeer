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
        public void CreateSurfaceLineChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = MacroStabilityInwardsChartDataFactory.CreateSurfaceLineChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Profielschematisatie", data.Name);
            AssertEqualStyle(data.Style, Color.Sienna, 2, ChartLineDashStyle.Solid);
        }

        [Test]
        public void CreateSoilLayerChartData_ReturnsEmptyChartDataCollectionWithDefaultStyling()
        {
            // Call
            ChartDataCollection data = MacroStabilityInwardsChartDataFactory.CreateSoilProfileChartData();

            // Assert
            Assert.IsEmpty(data.Collection);
            Assert.AreEqual("Ondergrondschematisatie", data.Name);
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var layers = new List<MacroStabilityInwardsSoilLayer>();
            for (var i = 0; i < soilLayerIndex; i++)
            {
                layers.Add(new MacroStabilityInwardsSoilLayer((double) i / 10));
            }
            layers.Add(new MacroStabilityInwardsSoilLayer(-1.0)
            {
                MaterialName = name,
                Color = Color.Aquamarine
            });

            var profile = new MacroStabilityInwardsSoilProfile("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            var soilProfile = new MacroStabilityInwardsSoilProfile("soil profile name", 2.0,
                                                                   new[]
                                                                   {
                                                                       new MacroStabilityInwardsSoilLayer(3.2)
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
                new MacroStabilityInwardsSoilLayer(0),
                new MacroStabilityInwardsSoilLayer(1)
            };
            var profile = new MacroStabilityInwardsSoilProfile("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(soilLayerIndex, profile);

            // Assert
            string paramName = Assert.Throws<ArgumentOutOfRangeException>(test).ParamName;
            Assert.AreEqual("soilLayerIndex", paramName);
        }

        private static void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, ChartLineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }

        private static void AssertEqualStyle(ChartAreaStyle areaStyle, Color fillColor, Color strokeColor, int width)
        {
            Assert.AreEqual(fillColor, areaStyle.FillColor);
            Assert.AreEqual(strokeColor, areaStyle.StrokeColor);
            Assert.AreEqual(width, areaStyle.StrokeThickness);
        }
    }
}