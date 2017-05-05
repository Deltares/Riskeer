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
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateLowerRevetmentBoundaryChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Ondergrens bekleding", data.Name);
            AssertEqualStyle(data.Style, Color.Gray, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateUpperRevetmentBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateUpperRevetmentBoundaryChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bovengrens bekleding", data.Name);
            AssertEqualStyle(data.Style, Color.Gray, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateRevetmentChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateRevetmentChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bekleding", data.Name);
            AssertEqualStyle(data.Style, Color.Gray, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateRevetmentBaseChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateRevetmentBaseChartData();

            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bekleding", data.Name);
            AssertEqualStyle(data.Style, Color.Gray, 2, DashStyle.Dash);
        }

        [Test]
        public void CreateLowerWaterLevelBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateLowerWaterLevelsBoundaryChartdata();
            
            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Ondergrens waterstanden", data.Name);
            AssertEqualStyle(data.Style, Color.Blue, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateUpperWaterLevelBoundaryChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateUpperWaterLevelsBoundaryChartdata();
           
            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Bovengrens waterstanden", data.Name);
            AssertEqualStyle(data.Style, Color.Blue, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateDesignwaterLevelChartData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = WaveConditionsChartDataFactory.CreateDesignWaterLevelChartdata();
            
            // Assert
            Assert.IsEmpty(data.Points);
            Assert.AreEqual("Toetspeil", data.Name);
            AssertEqualStyle(data.Style, Color.Red, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateWaterLevelsChartData_ReturnsEmptyChartMultipleLineDataWithDefaultStyling()
        {
            // Call
            ChartMultipleLineData data = WaveConditionsChartDataFactory.CreateWaterLevelsChartData();
            
            // Assert
            Assert.IsEmpty(data.Lines);
            Assert.AreEqual("Waterstanden", data.Name);
            AssertEqualStyle(data.Style, Color.Blue, 2, DashStyle.Dash);
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
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreTrue_NameSetToDikeProfileName()
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
            Assert.AreEqual(style, lineStyle.Style);
        }
    }
}