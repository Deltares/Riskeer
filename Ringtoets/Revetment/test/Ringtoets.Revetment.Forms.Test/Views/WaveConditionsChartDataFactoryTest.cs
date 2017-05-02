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

using Core.Components.Charting.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Forms.Test.Views
{
    [TestFixture]
    public class WaveConditionsChartDataFactoryTest
    {
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
    }
}