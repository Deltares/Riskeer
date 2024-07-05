﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.TestUtil;
using Riskeer.GrassCoverErosionOutwards.IO.Configurations;

namespace Riskeer.GrassCoverErosionOutwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriterTest : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
        GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter,
        GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Write_SparseCalculation_WritesSparseConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(Write_SparseCalculation_WritesSparseConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.GrassCoverErosionOutwards.IO,
                Path.Combine(nameof(GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter), "sparseConfiguration.xml"));

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration("Berekening 1");

            try
            {
                var writer = new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter(filePath);

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
                TestDataPath.Riskeer.GrassCoverErosionOutwards.IO,
                Path.Combine(nameof(GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter), "completeConfiguration.xml"));

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                TargetProbability = 0.01,
                UpperBoundaryRevetment = (RoundedDouble) 1.5,
                LowerBoundaryRevetment = (RoundedDouble) 0.5,
                UpperBoundaryWaterLevels = (RoundedDouble) 1.4,
                LowerBoundaryWaterLevels = (RoundedDouble) 0.6,
                StepSize = (RoundedDouble) 0.15,
                ForeshoreProfileId = "profiel1",
                Orientation = (RoundedDouble) 67.1,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseForeshoreProfile = true,
                    UseBreakWater = true,
                    BreakWaterHeight = (RoundedDouble) 1.23,
                    BreakWaterType = ConfigurationBreakWaterType.Dam
                },
                CalculationType = ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact
            };

            try
            {
                var writer = new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter(filePath);

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
        public void Write_InvalidCalculationType_ThrowsCriticalFileWriteException()
        {
            // Setup
            var configuration = new GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration("fail")
            {
                CalculationType = (ConfigurationGrassCoverErosionOutwardsCalculationType?) 99
            };

            var writer = new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter("valid");

            // Call
            void Call() => writer.Write(new[]
            {
                configuration
            });

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(Call);
            Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
        }

        protected override GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationWriter(filePath);
        }
    }
}