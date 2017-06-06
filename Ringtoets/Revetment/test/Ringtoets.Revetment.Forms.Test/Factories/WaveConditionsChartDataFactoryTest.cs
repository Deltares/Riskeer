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
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Factories;

namespace Ringtoets.Revetment.Forms.Test.Factories
{
    [TestFixture]
    public class WaveConditionsChartDataFactoryTest
    {
        [Test]
        public void CreateLowerRevetmentBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Setup
            Color lineColor = Color.Gray;

            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateLowerRevetmentBoundaryChartData(lineColor);

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Ondergrens bekleding", data.Name);
            AssertEqualStyle(data.Style, lineColor, 3, DashStyle.Solid);
        }

        [Test]
        public void CreateUpperRevetmentBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Setup
            Color lineColor = Color.Gray;

            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateUpperRevetmentBoundaryChartData(lineColor);

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bovengrens bekleding", data.Name);
            AssertEqualStyle(data.Style, lineColor, 3, DashStyle.Solid);
        }

        [Test]
        public void CreateRevetmentChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Setup
            Color lineColor = Color.Gray;

            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateRevetmentChartData(lineColor);

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bekleding", data.Name);
            AssertEqualStyle(data.Style, lineColor, 8, DashStyle.Solid);
        }

        [Test]
        public void CreateRevetmentBaseChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Setup
            Color lineColor = Color.FromArgb(120, Color.Gray);

            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateRevetmentBaseChartData(lineColor);

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Hulplijn bekleding", data.Name);
            AssertEqualStyle(data.Style, lineColor, 8, DashStyle.Dash);
        }

        [Test]
        public void CreateLowerWaterLevelsBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateLowerWaterLevelsBoundaryChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Ondergrens waterstanden", data.Name);
            AssertEqualStyle(data.Style, Color.MediumBlue, 3, DashStyle.Solid);
        }

        [Test]
        public void CreateUpperWaterLevelsBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateUpperWaterLevelsBoundaryChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bovengrens waterstanden", data.Name);
            AssertEqualStyle(data.Style, Color.MediumBlue, 3, DashStyle.Solid);
        }

        [Test]
        public void CreateDesignWaterLevelChartData_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => WaveConditionsChartDataFactory.CreateDesignWaterLevelChartData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("chartDataName", exception.ParamName);
        }

        [Test]
        public void CreateDesignWaterLevelChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Setup
            const string designWaterLevelName = "Toetspeil";

            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateDesignWaterLevelChartData(designWaterLevelName);

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual(designWaterLevelName, data.Name);
            AssertEqualStyle(data.Style, Color.LightCoral, 3, DashStyle.Solid);
        }

        [Test]
        public void CreateWaterLevelsChartData_ReturnsEmptyChartMultipleLineDataWithDefaultStyling()
        {
            // Call
            ChartMultipleLineData data = WaveConditionsChartDataFactory.CreateWaterLevelsChartData();

            // Assert
            Assert.IsEmpty(data.Lines);
            Assert.AreEqual("Waterstanden in berekening", data.Name);
            AssertEqualStyle(data.Style, Color.DarkTurquoise, 3, DashStyle.DashDotDot);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_InputNull_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");

            // Call
            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Voorlandprofiel", chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileNull_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new WaveConditionsInput
            {
                UseForeshore = true
            };

            // Call
            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            Assert.AreEqual("Voorlandprofiel", chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreFalse_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile("profile name"),
                UseForeshore = false
            };

            // Call
            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            Assert.AreEqual("Voorlandprofiel", chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreTrue_NameSetToForeshoreProfileName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile("profile name"),
                UseForeshore = true
            };

            // Call
            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            string expectedName = $"{input.ForeshoreProfile.Name} - Voorlandprofiel";
            Assert.AreEqual(expectedName, chartData.Name);
        }

        private static void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }
    }
}