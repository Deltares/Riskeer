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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.IO.Exporters;
using Ringtoets.StabilityPointStructures.IO.Writers;

namespace Ringtoets.StabilityPointStructures.IO.Test.Exporters
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationExporterTest
        : CustomSchemaCalculationConfigurationExporterDesignGuidelinesTestFixture<
            StabilityPointStructuresCalculationConfigurationExporter,
            StabilityPointStructuresCalculationConfigurationWriter,
            StructuresCalculation<StabilityPointStructuresInput>,
            StabilityPointStructuresCalculationConfiguration>
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                yield return new TestCaseData("sparseConfiguration", new[]
                    {
                        CreateSparseCalculation()
                    })
                    .SetName("Calculation configuration with none of its parameters set");
                yield return new TestCaseData("completeConfiguration", new[]
                    {
                        CreateFullCalculation()
                    })
                    .SetName("Calculation configuration with all parameters set");
                yield return new TestCaseData("folderWithSubfolderAndCalculation", new[]
                    {
                        new CalculationGroup("Testmap", false)
                        {
                            Children =
                            {
                                CreateFullCalculation(),
                                new CalculationGroup("Nested", false)
                                {
                                    Children =
                                    {
                                        CreateSparseCalculation()
                                    }
                                }
                            }
                        }
                    })
                    .SetName("Calculation configuration with hierarchy");
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Export_ValidData_ReturnTrueAndWritesFile(string fileName, ICalculationBase[] calculations)
        {
            // Setup
            string testDirectory = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.StabilityPointStructures.IO,
                nameof(StabilityPointStructuresCalculationConfigurationExporter));

            string expectedXmlFilePath = Path.Combine(testDirectory, $"{fileName}.xml");

            // Call and Assert
            WriteAndValidate(calculations, expectedXmlFilePath);
        }

        private static ICalculationBase CreateSparseCalculation()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "sparse config"
            };
        }

        private static ICalculationBase CreateFullCalculation()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "full config",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    Structure = new TestStabilityPointStructure("kunstwerk1"),
                    AllowedLevelIncreaseStorage = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    AreaFlowApertures = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 80.5,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    BankWidth = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = (RoundedDouble) 1.234
                    },
                    ConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    DrainCoefficient = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 0.1
                    },
                    EvaluationLevel = (RoundedDouble) 1e-1,
                    FactorStormDurationOpenStructure = (RoundedDouble) 1e-2,
                    FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    FailureProbabilityRepairClosure = 1e-3,
                    FailureProbabilityStructureWithErosion = 1e-4,
                    FlowVelocityStructureClosable = new VariationCoefficientNormalDistribution
                    {
                        Mean = (RoundedDouble) 1.1
                    },
                    FlowWidthAtBottomProtection = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1"),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    InsideWaterLevel = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    InsideWaterLevelFailureConstruction = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 0.7,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    LevelCrestStructure = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 4.3,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    LevellingCount = 1,
                    LoadSchematizationType = LoadSchematizationType.Linear,
                    ModelFactorSuperCriticalFlow = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 0.1
                    },
                    ProbabilityCollisionSecondaryStructure = 1e-5,
                    VolumicWeightWater = (RoundedDouble) 9.81,
                    StormDuration = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    ShipMass = new VariationCoefficientNormalDistribution
                    {
                        Mean = (RoundedDouble) 16000,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ShipVelocity = new VariationCoefficientNormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StorageStructureArea = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 15000,
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    StructureNormalOrientation = (RoundedDouble) 7,
                    ThresholdHeightOpenWeir = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    UseBreakWater = true,
                    UseForeshore = false,
                    VerticalDistance = (RoundedDouble) 2,
                    WidthFlowApertures = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                }
            };
        }

        protected override StructuresCalculation<StabilityPointStructuresInput> CreateCalculation()
        {
            return new TestStabilityPointStructuresCalculation();
        }

        protected override StabilityPointStructuresCalculationConfigurationExporter CallConfigurationFilePathConstructor(
            IEnumerable<ICalculationBase> calculations,
            string filePath)
        {
            return new StabilityPointStructuresCalculationConfigurationExporter(calculations, filePath);
        }

        private static StabilityPointStructuresCalculationConfiguration CreateFullCalculation2()
        {
            return new StabilityPointStructuresCalculationConfiguration("full config")
            {
                AllowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.2,
                    StandardDeviation = 0.01
                },
                AreaFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 80.5,
                    StandardDeviation = 1
                },
                BankWidth = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 1.2,
                    StandardDeviation = 0.1
                },
                ConstructiveStrengthLinearLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 2,
                    VariationCoefficient = 0.1
                },
                ConstructiveStrengthQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 2,
                    VariationCoefficient = 0.1
                },
                CriticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 2,
                    VariationCoefficient = 0.1
                },
                DrainCoefficient = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.1
                },
                EvaluationLevel = 1e-1,
                FactorStormDurationOpenStructure = 1e-2,
                FailureCollisionEnergy = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 1.2,
                    VariationCoefficient = 0.1
                },
                FailureProbabilityRepairClosure = 1e-3,
                FailureProbabilityStructureWithErosion = 1e-4,
                FlowVelocityStructureClosable = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 1.1
                },
                FlowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 15.2,
                    StandardDeviation = 0.1
                },
                ForeshoreProfileName = "profiel1",
                HydraulicBoundaryLocationName = "Locatie1",
                InflowModelType = ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert,
                InsideWaterLevel = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.5,
                    StandardDeviation = 0.1
                },
                InsideWaterLevelFailureConstruction = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.7,
                    StandardDeviation = 0.1
                },
                LevelCrestStructure = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 4.3,
                    StandardDeviation = 0.1
                },
                LevellingCount = 1,
                LoadSchematizationType = ConfigurationStabilityPointStructuresLoadSchematizationType.Linear,
                ModelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 0.1
                },
                ProbabilityCollisionSecondaryStructure = 1e-5,
                VolumicWeightWater = 1e-6,
                StormDuration = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 6.0
                },
                ShipMass = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 16000,
                    VariationCoefficient = 0.1
                },
                ShipVelocity = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 1.2,
                    VariationCoefficient = 0.1
                },
                StabilityLinearLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 1.2,
                    VariationCoefficient = 0.1
                },
                StabilityQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 1.2,
                    VariationCoefficient = 0.1
                },
                StorageStructureArea = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = 15000,
                    VariationCoefficient = 0.01
                },
                StructureName = "kunstwerk1",
                StructureNormalOrientation = 1e-7,
                ThresholdHeightOpenWeir = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 1.2,
                    StandardDeviation = 0.1
                },
                VerticalDistance = 1e-8,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = true,
                    BreakWaterType = ConfigurationBreakWaterType.Dam,
                    BreakWaterHeight = 1.23,
                    UseForeshoreProfile = false
                },
                WidthFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = 15.2,
                    StandardDeviation = 0.1
                }
            };
        }
    }
}