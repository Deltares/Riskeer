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
using System.IO;
using System.Xml;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Configurations;

namespace Ringtoets.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            WaveConditionsCalculationConfigurationWriter,
            WaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Write_SparseCalculation_WritesSparseConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(Write_SparseCalculation_WritesSparseConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Revetment.IO,
                Path.Combine(nameof(WaveConditionsCalculationConfigurationWriter), "sparseConfiguration.xml"));

            var calculation = new WaveConditionsCalculationConfiguration("Berekening 1");

            try
            {
                var writer = new WaveConditionsCalculationConfigurationWriter(filePath);

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
                TestDataPath.Ringtoets.Revetment.IO,
                Path.Combine(nameof(WaveConditionsCalculationConfigurationWriter), "completeConfiguration.xml"));

            var calculation = new WaveConditionsCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocation = "Locatie1",
                UpperBoundaryRevetment = (RoundedDouble) 1.5,
                LowerBoundaryRevetment = (RoundedDouble) 0.5,
                UpperBoundaryWaterLevels = (RoundedDouble) 1.4,
                LowerBoundaryWaterLevels = (RoundedDouble) 0.6,
                StepSize = ConfigurationWaveConditionsInputStepSize.One,
                ForeshoreProfile = "profiel1",
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
                var writer = new WaveConditionsCalculationConfigurationWriter(filePath);

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

        protected override WaveConditionsCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new WaveConditionsCalculationConfigurationWriter(filePath);
        }
    }

    public class SimpleWaveConditionsCalculation : Observable, ICalculation
    {
        public WaveConditionsInput Input { get; } = new WaveConditionsInput();
        public string Name { get; set; }
        public bool HasOutput { get; }
        public Comment Comments { get; }

        public void ClearOutput()
        {
            throw new NotImplementedException();
        }
    }
}