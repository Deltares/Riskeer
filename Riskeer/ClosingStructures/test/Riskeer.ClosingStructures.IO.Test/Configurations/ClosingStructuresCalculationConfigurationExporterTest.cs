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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.IO.Configurations;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;

namespace Riskeer.ClosingStructures.IO.Test.Configurations
{
    [TestFixture]
    public class ClosingStructuresCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            ClosingStructuresCalculationConfigurationExporter,
            ClosingStructuresCalculationConfigurationWriter,
            StructuresCalculationScenario<ClosingStructuresInput>,
            ClosingStructuresCalculationConfiguration>
    {
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
                        CreateSparseCalculation()
                    })
                    .SetName("Calculation configuration with none of its parameters set");
                yield return new TestCaseData("configurationWithStructure", new[]
                    {
                        CreateCalculationWithStructure()
                    })
                    .SetName("Calculation configuration with a structure set");
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
                yield return new TestCaseData("calculationScenarioIrrelevant", new[]
                    {
                        CreateIrrelevantCalculationScenario()
                    })
                    .SetName("Calculation configuration with scenario is relevant false");
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Export_ValidData_ReturnTrueAndWritesFile(string fileName, ICalculationBase[] calculations)
        {
            // Setup
            string testDirectory = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.ClosingStructures.IO,
                nameof(ClosingStructuresCalculationConfigurationExporter));

            string expectedXmlFilePath = Path.Combine(testDirectory, $"{fileName}.xml");

            // Call and Assert
            WriteAndValidate(calculations, expectedXmlFilePath);
        }

        private static ICalculation CreateSparseCalculation()
        {
            return new StructuresCalculationScenario<ClosingStructuresInput>
            {
                Name = "sparse config"
            };
        }

        private static StructuresCalculationScenario<ClosingStructuresInput> CreateFullCalculation()
        {
            return new TestClosingStructuresCalculationScenario
            {
                Name = "full config",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1"),
                    Structure = new TestClosingStructure("kunstwerk1", "kunstwerk1"),
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    FailureProbabilityStructureWithErosion = 1e-4,
                    StructureNormalOrientation = (RoundedDouble) 67.1,
                    FactorStormDurationOpenStructure = (RoundedDouble) 1.0,
                    IdenticalApertures = 1,
                    ProbabilityOpenStructureBeforeFlooding = 1e-2,
                    FailureProbabilityOpenStructure = 1e-3,
                    FailureProbabilityReparation = 1e-2,
                    InflowModelType = ClosingStructureInflowModelType.LowSill,
                    UseBreakWater = true,
                    UseForeshore = true,
                    ShouldIllustrationPointsBeCalculated = true,
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = (RoundedDouble) 1.234
                    },
                    StormDuration = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 6.0,
                        CoefficientOfVariation = (RoundedDouble) 0.22
                    },
                    ModelFactorSuperCriticalFlow = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 1.10,
                        StandardDeviation = (RoundedDouble) 0.14
                    },
                    FlowWidthAtBottomProtection = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 13.2,
                        StandardDeviation = (RoundedDouble) 0.3
                    },
                    StorageStructureArea = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 15000,
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    AllowedLevelIncreaseStorage = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    DrainCoefficient = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 1.1,
                        StandardDeviation = (RoundedDouble) 0.02
                    },
                    InsideWaterLevel = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    AreaFlowApertures = new LogNormalDistribution
                    {
                        Mean = (RoundedDouble) 80.5,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    ThresholdHeightOpenWeir = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    LevelCrestStructureNotClosing = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 4.3,
                        StandardDeviation = (RoundedDouble) 0.2
                    }
                }
            };
        }

        private static StructuresCalculationScenario<ClosingStructuresInput> CreateCalculationWithForeshoreProfile()
        {
            return new StructuresCalculationScenario<ClosingStructuresInput>
            {
                Name = "with foreshore profile",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    UseForeshore = true
                }
            };
        }

        private static StructuresCalculationScenario<ClosingStructuresInput> CreateCalculationWithUseBreakWater()
        {
            return new StructuresCalculationScenario<ClosingStructuresInput>
            {
                Name = "with use breakwater",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    BreakWater =
                    {
                        Type = BreakWaterType.Caisson,
                        Height = (RoundedDouble) 1.23
                    },
                    UseBreakWater = true,
                    UseForeshore = false
                }
            };
        }

        private static StructuresCalculationScenario<ClosingStructuresInput> CreateCalculationWithStructure()
        {
            return new StructuresCalculationScenario<ClosingStructuresInput>
            {
                Name = "with structure",
                InputParameters =
                {
                    Structure = new TestClosingStructure("kunstwerk1", "kunstwerk1")
                }
            };
        }

        private static StructuresCalculationScenario<ClosingStructuresInput> CreateIrrelevantCalculationScenario()
        {
            return new StructuresCalculationScenario<ClosingStructuresInput>
            {
                Name = "irrelevant",
                Contribution = (RoundedDouble) 0.5432,
                IsRelevant = false
            };
        }

        protected override StructuresCalculationScenario<ClosingStructuresInput> CreateCalculation()
        {
            return new TestClosingStructuresCalculationScenario();
        }

        protected override ClosingStructuresCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new ClosingStructuresCalculationConfigurationExporter(calculations, filePath);
        }
    }
}