// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.IO;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.WaveImpactAsphaltCover.IO.Configurations;

namespace Riskeer.WaveImpactAsphaltCover.IO.Test.Configurations
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriterTest : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
        WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter, WaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Write_SparseCalculation_WritesSparseConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(Write_SparseCalculation_WritesSparseConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.WaveImpactAsphaltCover.IO,
                Path.Combine(nameof(WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter), "sparseConfiguration.xml"));

            var calculation = new WaveConditionsCalculationConfiguration("Berekening 1");

            try
            {
                WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter writer = CreateWriterInstance(filePath);

                // Call
                writer.Write(new[]
                {
                    calculation
                });

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Write_CompleteCalculation_WritesCompleteConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(Write_CompleteCalculation_WritesCompleteConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.WaveImpactAsphaltCover.IO,
                Path.Combine(nameof(WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter), "completeConfiguration.xml"));

            var calculation = new WaveConditionsCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                UpperBoundaryRevetment = (RoundedDouble) 1.5,
                LowerBoundaryRevetment = (RoundedDouble) 0.5,
                UpperBoundaryWaterLevels = (RoundedDouble) 1.4,
                LowerBoundaryWaterLevels = (RoundedDouble) 0.6,
                StepSize = ConfigurationWaveConditionsInputStepSize.One,
                ForeshoreProfileId = "profiel1",
                Orientation = (RoundedDouble) 67.1,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseForeshoreProfile = true,
                    UseBreakWater = true,
                    BreakWaterHeight = (RoundedDouble) 1.23,
                    BreakWaterType = ConfigurationBreakWaterType.Dam
                }
            };

            try
            {
                WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter writer = CreateWriterInstance(filePath);

                // Call
                writer.Write(new[]
                {
                    calculation
                });

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        protected override WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter(filePath);
        }
    }
}