// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Common.IO.TestUtil;
using Riskeer.HeightStructures.IO.Configurations;

namespace Riskeer.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            HeightStructuresCalculationConfigurationWriter,
            HeightStructuresCalculationConfiguration>
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Riskeer.HeightStructures.IO,
            nameof(HeightStructuresCalculationConfigurationWriter));

        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                yield return new TestCaseData("completeConfiguration", new[]
                    {
                        CreateFullCalculation()
                    })
                    .SetName("Calculation configuration with all parameters set");
                yield return new TestCaseData("sparseConfiguration", new[]
                    {
                        new HeightStructuresCalculationConfiguration("sparse config")
                    })
                    .SetName("Calculation configuration with none of its parameters set");
                yield return new TestCaseData("folderWithSubfolderAndCalculation", new IConfigurationItem[]
                    {
                        new CalculationGroupConfiguration("Testmap", new IConfigurationItem[]
                        {
                            CreateFullCalculation(),
                            new CalculationGroupConfiguration("Nested", new IConfigurationItem[]
                            {
                                new HeightStructuresCalculationConfiguration("Berekening 2")
                            })
                        })
                    })
                    .SetName("Calculation configurations in a hierarchy");
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculation_ValidFile(string expectedFileName, IConfigurationItem[] configuration)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(HeightStructuresCalculationConfigurationWriterTest)}.{nameof(Write_ValidCalculation_ValidFile)}.{expectedFileName}.xml");

            try
            {
                var writer = new HeightStructuresCalculationConfigurationWriter(filePath);

                // Call
                writer.Write(configuration);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = Path.Combine(testDataPath, $"{expectedFileName}.xml");
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        protected override void AssertDefaultConstructedInstance(HeightStructuresCalculationConfigurationWriter writer)
        {
            Assert.IsInstanceOf<StructureCalculationConfigurationWriter<HeightStructuresCalculationConfiguration>>(writer);
        }

        private static HeightStructuresCalculationConfiguration CreateFullCalculation()
        {
            return new HeightStructuresCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                StructureId = "kunstwerk1",
                ForeshoreProfileId = "profiel1",
                FailureProbabilityStructureWithErosion = 1e-6,
                StructureNormalOrientation = 67.1,
                ShouldIllustrationPointsBeCalculated = true,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = true,
                    BreakWaterType = ConfigurationBreakWaterType.Dam,
                    BreakWaterHeight = 1.23,
                    UseForeshoreProfile = true
                },
                StormDuration = new StochastConfiguration
                {
                    Mean = 6.0,
                    VariationCoefficient = 0.22
                },
                ModelFactorSuperCriticalFlow = new StochastConfiguration
                {
                    Mean = 1.1,
                    StandardDeviation = 0.14
                },
                FlowWidthAtBottomProtection = new StochastConfiguration
                {
                    Mean = 15.2,
                    StandardDeviation = 0.1
                },
                WidthFlowApertures = new StochastConfiguration
                {
                    Mean = 13.2,
                    StandardDeviation = 0.3
                },
                StorageStructureArea = new StochastConfiguration
                {
                    Mean = 15000,
                    VariationCoefficient = 0.01
                },
                AllowedLevelIncreaseStorage = new StochastConfiguration
                {
                    Mean = 0.2,
                    StandardDeviation = 0.01
                },
                LevelCrestStructure = new StochastConfiguration
                {
                    Mean = 4.3,
                    StandardDeviation = 0.2
                },
                CriticalOvertoppingDischarge = new StochastConfiguration
                {
                    Mean = 2,
                    VariationCoefficient = 0.1
                }
            };
        }

        protected override HeightStructuresCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new HeightStructuresCalculationConfigurationWriter(filePath);
        }
    }
}