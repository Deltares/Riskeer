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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.IO.Configurations;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.ClosingStructures.IO.Test.Configurations
{
    [TestFixture]
    public class ClosingStructuresCalculationConfigurationWriterTest
        : CustomSchemaCalculationConfigurationWriterDesignGuidelinesTestFixture<
            ClosingStructuresCalculationConfigurationWriter,
            ClosingStructuresCalculationConfiguration>
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.ClosingStructures.IO,
            nameof(ClosingStructuresCalculationConfigurationWriter));

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
                        new ClosingStructuresCalculationConfiguration("sparse config")
                    })
                    .SetName("Calculation configuration with none of its parameters set");
                yield return new TestCaseData("folderWithSubfolderAndCalculation", new IConfigurationItem[]
                    {
                        new CalculationGroupConfiguration("Testmap", new IConfigurationItem[]
                        {
                            CreateFullCalculation(),
                            new CalculationGroupConfiguration("Nested", new IConfigurationItem[]
                            {
                                new ClosingStructuresCalculationConfiguration("Berekening 2")
                            })
                        })
                    })
                    .SetName("Calculation configurations in a hierarchy");
            }
        }

        protected override void AssertDefaultConstructedInstance(ClosingStructuresCalculationConfigurationWriter writer)
        {
            Assert.IsInstanceOf<StructureCalculationConfigurationWriter<ClosingStructuresCalculationConfiguration>>(writer);
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculation_ValidFile(string expectedFileName, IConfigurationItem[] configuration)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(ClosingStructuresCalculationConfigurationWriterTest)}.{nameof(Write_ValidCalculation_ValidFile)}.{expectedFileName}.xml");

            try
            {
                var writer = new ClosingStructuresCalculationConfigurationWriter(filePath);

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

        private static ClosingStructuresCalculationConfiguration CreateFullCalculation()
        {
            return new ClosingStructuresCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                StructureName = "kunstwerk1",
                ForeshoreProfileName = "profiel1",
                FailureProbabilityStructureWithErosion = 1e-4,
                StructureNormalOrientation = 67.1,
                FactorStormDurationOpenStructure = 1.0,
                IdenticalApertures = 1,
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = 1e-2,
                FailureProbabilityOpenStructure = 1e-3,
                FailureProbabilityReparation = 1e-2,
                InflowModelType = ConfigurationClosingStructureInflowModelType.LowSill,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = true,
                    BreakWaterType = ConfigurationBreakWaterType.Dam,
                    BreakWaterHeight = 1.23,
                    UseForeshoreProfile = true
                },
                FlowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 15.2,
                    StandardDeviation = 0.1
                },
                WidthFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 13.2,
                    StandardDeviation = 0.3
                },
                StorageStructureArea = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 15000,
                    VariationCoefficient = 0.01
                },
                CriticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 2,
                    VariationCoefficient = 0.1
                },
                ModelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 1.1,
                    StandardDeviation = 0.14
                },
                AllowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.2,
                    StandardDeviation = 0.01
                },
                StormDuration = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 6.0,
                    VariationCoefficient = 0.22
                },
                DrainCoefficient = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 1.1,
                    StandardDeviation = 0.02
                },
                InsideWaterLevel = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.5,
                    StandardDeviation = 0.1
                },
                AreaFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 80.5,
                    StandardDeviation = 1
                },
                ThresholdHeightOpenWeir = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 1.2,
                    StandardDeviation = 0.1
                },
                LevelCrestStructureNotClosing = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 4.3,
                    StandardDeviation = 0.2
                }
            };
        }

        protected override ClosingStructuresCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new ClosingStructuresCalculationConfigurationWriter(filePath);
        }
    }
}