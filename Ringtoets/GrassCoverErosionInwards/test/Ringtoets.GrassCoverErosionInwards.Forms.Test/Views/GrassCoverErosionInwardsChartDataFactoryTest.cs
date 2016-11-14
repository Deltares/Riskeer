// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsChartDataFactoryTest
    {
        [Test]
        public void CreateDikeGeometryChartData_ReturnsChartDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = GrassCoverErosionInwardsChartDataFactory.CreateDikeGeometryChartData();

            // Assert
            Assert.AreEqual(string.Format(Resources.DikeProfile_DisplayName), data.Name);
            AssertEqualStyle(data.Style, Color.SaddleBrown, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateForeshoreGeometryChartData_ReturnsChartDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = GrassCoverErosionInwardsChartDataFactory.CreateForeshoreGeometryChartData();

            // Assert
            Assert.AreEqual(string.Format(RingtoetsCommonFormsResources.Foreshore_DisplayName), data.Name);
            AssertEqualStyle(data.Style, Color.DarkOrange, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateDikeHeightChartData_ReturnsChartDataWithDefaultStyling()
        {
            // Call
            ChartLineData data = GrassCoverErosionInwardsChartDataFactory.CreateDikeHeightChartData();

            // Assert
            Assert.AreEqual(Resources.DikeHeight_ChartName, data.Name);
            AssertEqualStyle(data.Style, Color.MediumSeaGreen, 2, DashStyle.Dash);
        }

        [Test]
        public void UpdateDikeGeometryChartDataName_DikeProfileNull_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateDikeGeometryChartDataName(chartData, null);

            // Assert
            Assert.AreEqual(Resources.DikeProfile_DisplayName, chartData.Name);
        }

        [Test]
        public void UpdateDikeGeometryChartDataName_DikeProfile_NameSetToDikeProfileName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            DikeProfile dikeProfile = new TestDikeProfile("dike profile name");

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateDikeGeometryChartDataName(chartData, dikeProfile);

            // Assert
            var expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                             dikeProfile.Name,
                                             Resources.DikeProfile_DisplayName);
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
            Assert.AreEqual(RingtoetsCommonFormsResources.Foreshore_DisplayName, chartData.Name);
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
            Assert.AreEqual(RingtoetsCommonFormsResources.Foreshore_DisplayName, chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreFalse_NameSetToDefaultName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile("dike profile name"),
                UseForeshore = false
            };

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            Assert.AreEqual(RingtoetsCommonFormsResources.Foreshore_DisplayName, chartData.Name);
        }

        [Test]
        public void UpdateForeshoreGeometryChartDataName_DikeProfileSetUseForeshoreTrue_NameSetToDikeProfileName()
        {
            // Setup
            var chartData = new ChartLineData("test name");
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile("dike profile name"),
                UseForeshore = true
            };

            // Call
            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(chartData, input);

            // Assert
            var expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                             input.DikeProfile.Name,
                                             RingtoetsCommonFormsResources.Foreshore_DisplayName);
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