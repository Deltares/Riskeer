﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.ComponentModel;
using System.IO;
using System.Xml;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Revetment.IO.Configurations;

namespace Riskeer.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>,
            WaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Write_SparseCalculation_WritesSparseConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(Write_SparseCalculation_WritesSparseConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.Revetment.IO,
                Path.Combine(nameof(WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>), "sparseConfiguration.xml"));

            var calculation = new WaveConditionsCalculationConfiguration("Berekening 1");

            try
            {
                var writer = new TestWaveConditionsCalculationConfigurationWriter(filePath);

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
                TestDataPath.Riskeer.Revetment.IO,
                Path.Combine(nameof(WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>), "completeConfiguration.xml"));

            var calculation = new WaveConditionsCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                TargetProbability = 0.01,
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
                var writer = new TestWaveConditionsCalculationConfigurationWriter(filePath);

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
        public void Write_InvalidStepSize_ThrowsCriticalFileWriteException()
        {
            // Setup
            var configuration = new WaveConditionsCalculationConfiguration("fail")
            {
                StepSize = (ConfigurationWaveConditionsInputStepSize?) 9000
            };

            var writer = new TestWaveConditionsCalculationConfigurationWriter("valid");

            // Call
            TestDelegate call = () => writer.Write(new[]
            {
                configuration
            });

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
        }

        protected override WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration> CreateWriterInstance(string filePath)
        {
            return new TestWaveConditionsCalculationConfigurationWriter(filePath);
        }

        private class TestWaveConditionsCalculationConfigurationWriter : WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>
        {
            public TestWaveConditionsCalculationConfigurationWriter(string filePath) : base(filePath) {}

            protected override void WriteWaveConditionsSpecificParameters(XmlWriter writer, WaveConditionsCalculationConfiguration configuration) {}

            protected override int GetConfigurationVersion()
            {
                return 2;
            }
        }
    }
}