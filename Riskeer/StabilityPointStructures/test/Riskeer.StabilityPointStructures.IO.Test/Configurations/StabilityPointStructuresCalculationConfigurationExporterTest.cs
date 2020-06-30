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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
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
                        new CalculationGroup
                        {
                            Name = "Testmap",
                            Children =
                            {
                                CreateFullCalculation(),
                                new CalculationGroup
                                {
                                    Name = "Nested",
                                    Children =
                                    {
                                        CreateSparseCalculation()
                                    }
                                }
                            }
                        }
                    })
                    .SetName("Calculation configuration with hierarchy");
                yield return new TestCaseData("configurationWithForeshoreProfile", new[]
                    {
                        CreateCalculationWithForeshoreProfile()
                    })
                    .SetName("Calculation configuration with foreshore profile");
                yield return new TestCaseData("configurationWithUseBreakWater", new[]
                    {
                        CreateCalculationWithUseBreakWater()
                    })
                    .SetName("Calculation configuration with use breakwater true");
                yield return new TestCaseData("configurationWithStructure", new[]
                    {
                        CreateCalculationWithStructure()
                    })
                    .SetName("Calculation configuration with a structure set");
                yield return new TestCaseData("configurationWithStructureAndLoadSchematizationTypeLinear", new[]
                    {
                        CreateCalculationWithStructureAndLoadSchematizationLinearAndInflowModelFloodedCulvert()
                    })
                    .SetName("Calculation configuration with LoadSchematizationType set to Linear and InflowModelType to FloodedCulvert");

                yield return new TestCaseData("configurationWithStructureAndLoadSchematizationTypeQadratic", new[]
                    {
                        CreateCalculationWithStructureAndLoadSchematizationQadraticAndInflowModelLowSill()
                    })
                    .SetName("Calculation configuration with LoadSchematizationType set to Qadratic and InflowModelType to LowSil");
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Export_ValidData_ReturnTrueAndWritesFile(string fileName, ICalculationBase[] calculations)
        {
            // Setup
            string testDirectory = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.StabilityPointStructures.IO,
                nameof(StabilityPointStructuresCalculationConfigurationExporter));

            string expectedXmlFilePath = Path.Combine(testDirectory, $"{fileName}.xml");

            // Call and Assert
            WriteAndValidate(calculations, expectedXmlFilePath);
        }

        private static StructuresCalculation<StabilityPointStructuresInput> CreateCalculationWithStructureAndLoadSchematizationLinearAndInflowModelFloodedCulvert()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "with LoadSchematizationType set to Linear and InflowModelType to FloodedCulvert",
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1"),
                    LoadSchematizationType = LoadSchematizationType.Linear,
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert
                }
            };
        }

        private static StructuresCalculation<StabilityPointStructuresInput> CreateCalculationWithStructureAndLoadSchematizationQadraticAndInflowModelLowSill()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "with LoadSchematizationType set to Qadratic and InflowModelType to LowSil",
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1"),
                    LoadSchematizationType = LoadSchematizationType.Quadratic,
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill
                }
            };
        }

        private static StructuresCalculation<StabilityPointStructuresInput> CreateCalculationWithStructure()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "with structure",
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1")
                }
            };
        }

        private static StructuresCalculation<StabilityPointStructuresInput> CreateCalculationWithForeshoreProfile()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "with foreshore profile",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("profiel1")
                }
            };
        }

        private static StructuresCalculation<StabilityPointStructuresInput> CreateCalculationWithUseBreakWater()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "with use breakwater",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    BreakWater =
                    {
                        Type = BreakWaterType.Wall,
                        Height = (RoundedDouble) 1.24
                    },
                    UseBreakWater = true,
                    UseForeshore = false
                }
            };
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
                    Structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1"),
                    ShouldIllustrationPointsBeCalculated = true,
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
            return new TestStabilityPointStructuresCalculationScenario();
        }

        protected override StabilityPointStructuresCalculationConfigurationExporter CallConfigurationFilePathConstructor(
            IEnumerable<ICalculationBase> calculations,
            string filePath)
        {
            return new StabilityPointStructuresCalculationConfigurationExporter(calculations, filePath);
        }
    }
}