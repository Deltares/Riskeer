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

using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Factories;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Factories
{
    [TestFixture]
    public class GrassCoverErosionInwardsChartDataFactoryTest
    {
        [Test]
        public void CreateDikeGeometryChartData_ReturnsChartDataWithExpectedStyling()
        {
            // Call
            ChartLineData data = GrassCoverErosionInwardsChartDataFactory.CreateDikeGeometryChartData();

            // Assert
            Assert.AreEqual("Dijkprofiel", data.Name);
            AssertEqualStyle(data.Style, Color.SaddleBrown, 2, ChartLineDashStyle.Solid);
        }

        [Test]
        public void CreateDikeHeightChartData_ReturnsChartDataWithExpectedStyling()
        {
            // Call
            ChartLineData data = GrassCoverErosionInwardsChartDataFactory.CreateDikeHeightChartData();

            // Assert
            Assert.AreEqual("Dijkhoogte", data.Name);
            AssertEqualStyle(data.Style, Color.MediumSeaGreen, 2, ChartLineDashStyle.Dash);
        }

        [Test]
        public void UpdateDikeGeometryChartDataName_DikeProfileNull_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateDikeGeometryChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Dijkprofiel", chartData.Name);
        }

        [Test]
        public void UpdateDikeGeometryChartDataName_DikeProfile_NameSetToDikeProfileName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile("dike profile name");

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateDikeGeometryChartDataName(chartData, dikeProfile);

            // Assert
            string expectedName = $"{dikeProfile.Name} - Dijkprofiel";
            Assert.AreEqual(expectedName, chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_InputNull_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, null);

            // Assert
            Assert.AreEqual("Voorlandprofiel", chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileNull_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new GrassCoverErosionInwardsInput
            {
                UseForeshore = true
            };

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            Assert.AreEqual("Voorlandprofiel", chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreFalse_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile("dike profile name"),
                UseForeshore = false
            };

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            Assert.AreEqual("Voorlandprofiel", chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreTrue_NameSetToDikeProfileName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile("dike profile name"),
                UseForeshore = true
            };

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            string expectedName = $"{input.DikeProfile.Name} - Voorlandprofiel";
            Assert.AreEqual(expectedName, chartData.Name);
        }

        private static void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, ChartLineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
            Assert.IsTrue(lineStyle.IsEditable);
        }
    }
}