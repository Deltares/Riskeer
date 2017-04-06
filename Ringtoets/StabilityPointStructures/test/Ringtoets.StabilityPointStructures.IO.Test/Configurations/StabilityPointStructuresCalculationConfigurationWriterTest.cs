﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Common.IO.Writers;
using Ringtoets.StabilityPointStructures.IO.Configurations;

namespace Ringtoets.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationWriterTest
        : CustomSchemaCalculationConfigurationWriterDesignGuidelinesTestFixture<
            StabilityPointStructuresCalculationConfigurationWriter,
            StabilityPointStructuresCalculationConfiguration>
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.StabilityPointStructures.IO,
            nameof(StabilityPointStructuresCalculationConfigurationWriter));

        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                yield return new TestCaseData("sparseConfiguration", new[]
                    {
                        new StabilityPointStructuresCalculationConfiguration("sparse config")
                    })
                    .SetName("Calculation configuration with none of its parameters set");
                yield return new TestCaseData("completeConfiguration", new[]
                    {
                        CreateFullCalculation()
                    })
                    .SetName("Calculation configuration with all parameters set");
                yield return new TestCaseData("folderWithSubfolderAndCalculation", new IConfigurationItem[]
                    {
                        new CalculationGroupConfiguration("Testmap", new IConfigurationItem[]
                        {
                            CreateFullCalculation(),
                            new CalculationGroupConfiguration("Nested", new IConfigurationItem[]
                            {
                                new StabilityPointStructuresCalculationConfiguration("Berekening 2")
                            })
                        })
                    })
                    .SetName("Two nested calculation configurations");
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculation_ValidFile(string expectedFileName, IConfigurationItem[] configuration)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(StabilityPointStructuresCalculationConfigurationWriterTest)}" +
                                                           $".{nameof(Write_ValidCalculation_ValidFile)}.{expectedFileName}.xml");

            try
            {
                var writer = new StabilityPointStructuresCalculationConfigurationWriter(filePath);

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

        private static StabilityPointStructuresCalculationConfiguration CreateFullCalculation()
        {
            return new StabilityPointStructuresCalculationConfiguration("Berekening 1")
            {
                AllowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.01,
                    StandardDeviation = 0.11
                },
                AreaFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.02,
                    StandardDeviation = 0.12
                },
                BankWidth = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.03,
                    StandardDeviation = 0.13
                },
                ConstructiveStrengthLinearLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.04,
                    VariationCoefficient = 0.14
                },
                ConstructiveStrengthQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.05,
                    VariationCoefficient = 0.15
                },
                CriticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.06,
                    VariationCoefficient = 0.16
                },
                DrainCoefficient = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.01,
                    StandardDeviation = 0.02
                },
                EvaluationLevel = 1.1,
                FactorStormDurationOpenStructure = 1.123,
                FailureCollisionEnergy = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.07,
                    VariationCoefficient = 0.17
                },
                FailureProbabilityRepairClosure = 2.2,
                FailureProbabilityStructureWithErosion = 3.3,
                FlowVelocityStructureClosable = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.08,
                    VariationCoefficient = 0.18
                },
                FlowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.09,
                    StandardDeviation = 0.19
                },
                ForeshoreProfileName = "profiel1",
                HydraulicBoundaryLocationName = "Locatie1",
                InflowModelType = ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert,
                InsideWaterLevel = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.1,
                    StandardDeviation = 0.20
                },
                InsideWaterLevelFailureConstruction = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.11,
                    StandardDeviation = 0.21
                },
                LevelCrestStructure = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.12,
                    StandardDeviation = 0.22
                },
                LevellingCount = 4,
                LoadSchematizationType = ConfigurationStabilityPointStructuresLoadSchematizationType.Linear,
                ModelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.13,
                    StandardDeviation = 0.23
                },
                ProbabilityCollisionSecondaryStructure = 5.5,
                ShipMass = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.14,
                    VariationCoefficient = 0.24
                },
                ShipVelocity = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.15,
                    VariationCoefficient = 0.25
                },
                StabilityLinearLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.16,
                    VariationCoefficient = 0.26
                },
                StabilityQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.17,
                    VariationCoefficient = 0.27
                },
                StormDuration = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.18,
                    VariationCoefficient = 0.28
                },
                StructureName = "kunstwerk1",
                StructureNormalOrientation = 6.6,
                StorageStructureArea = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 0.19,
                    VariationCoefficient = 0.29
                },
                ThresholdHeightOpenWeir = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.20,
                    StandardDeviation = 0.30
                },
                VerticalDistance = 7.7,
                VolumicWeightWater = 9.9,
                WidthFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.21,
                    StandardDeviation = 0.31
                },
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = true,
                    BreakWaterType = ConfigurationBreakWaterType.Dam,
                    BreakWaterHeight = 8.8,
                    UseForeshoreProfile = true
                }
            };
        }

        protected override StabilityPointStructuresCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new StabilityPointStructuresCalculationConfigurationWriter(filePath);
        }

        protected override void AssertDefaultConstructedInstance(StabilityPointStructuresCalculationConfigurationWriter writer)
        {
            Assert.IsInstanceOf<StructureCalculationConfigurationWriter<StabilityPointStructuresCalculationConfiguration>>(writer);
        }
    }
}