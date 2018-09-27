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

using System.ComponentModel;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.GrassCoverErosionInwards.IO.Configurations;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            GrassCoverErosionInwardsCalculationConfigurationWriter,
            GrassCoverErosionInwardsCalculationConfiguration>
    {
        [Test]
        public void WriteConfiguration_SparseConfiguration_WritesSparseConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteConfiguration_SparseConfiguration_WritesSparseConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                Path.Combine(nameof(GrassCoverErosionInwardsCalculationConfigurationWriter), "sparseConfiguration.xml"));

            var calculation = new GrassCoverErosionInwardsCalculationConfiguration("Berekening 1");

            try
            {
                var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter(filePath);

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
        public void WriteConfiguration_CompleteConfiguration_WritesCompleteConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(
                $"{nameof(WriteConfiguration_CompleteConfiguration_WritesCompleteConfigurationToFile)}.xml");

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                Path.Combine(nameof(GrassCoverErosionInwardsCalculationConfigurationWriter), "completeConfiguration.xml"));

            GrassCoverErosionInwardsCalculationConfiguration calculation = CreateCompleteCalculation();

            try
            {
                var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter(filePath);

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
        public void Write_NestedConfiguration_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            GrassCoverErosionInwardsCalculationConfiguration calculation = CreateCompleteCalculation();
            var calculation2 = new GrassCoverErosionInwardsCalculationConfiguration("Berekening 2");
            var calculationGroup2 = new CalculationGroupConfiguration("Nested", new IConfigurationItem[]
            {
                calculation2
            });

            var calculationGroup = new CalculationGroupConfiguration("Testmap", new IConfigurationItem[]
            {
                calculation,
                calculationGroup2
            });

            try
            {
                var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter(filePath);

                // Call
                writer.Write(new[]
                {
                    calculationGroup
                });

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                        Path.Combine("GrassCoverErosionInwardsCalculationConfigurationWriter",
                                                                                     "folderWithSubfolderAndCalculation.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Write_InvalidDikeHeightCalculationType_ThrowsCriticalFileWriteException()
        {
            // Setup
            var configuration = new GrassCoverErosionInwardsCalculationConfiguration("fail")
            {
                DikeHeightCalculationType = (ConfigurationHydraulicLoadsCalculationType?) 9000
            };

            var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter("valid");

            // Call
            TestDelegate call = () => writer.Write(new[]
            {
                configuration
            });

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
        }

        [Test]
        public void Write_InvalidOvertoppingRateCalculationType_ThrowsCriticalFileWriteException()
        {
            // Setup
            var configuration = new GrassCoverErosionInwardsCalculationConfiguration("fail")
            {
                OvertoppingRateCalculationType = (ConfigurationHydraulicLoadsCalculationType?) 9000
            };

            var writer = new GrassCoverErosionInwardsCalculationConfigurationWriter("valid");

            // Call
            TestDelegate call = () => writer.Write(new[]
            {
                configuration
            });

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
        }

        private static GrassCoverErosionInwardsCalculationConfiguration CreateCompleteCalculation()
        {
            return new GrassCoverErosionInwardsCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                DikeProfileId = "id",
                Orientation = 67.1,
                DikeHeightCalculationType = ConfigurationHydraulicLoadsCalculationType.NoCalculation,
                OvertoppingRateCalculationType = ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm,
                DikeHeight = 0,
                ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                ShouldDikeHeightIllustrationPointsBeCalculated = false,
                ShouldOvertoppingRateIllustrationPointsBeCalculated = true,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseForeshoreProfile = true,
                    UseBreakWater = true,
                    BreakWaterHeight = 1.23,
                    BreakWaterType = ConfigurationBreakWaterType.Caisson
                },
                CriticalFlowRate = new StochastConfiguration
                {
                    Mean = 0.1,
                    StandardDeviation = 0.1
                }
            };
        }

        protected override GrassCoverErosionInwardsCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new GrassCoverErosionInwardsCalculationConfigurationWriter(filePath);
        }
    }
}