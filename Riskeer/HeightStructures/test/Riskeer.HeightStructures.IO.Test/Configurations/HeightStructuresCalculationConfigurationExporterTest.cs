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
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.IO.Configurations;

namespace Riskeer.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            HeightStructuresCalculationConfigurationExporter,
            HeightStructuresCalculationConfigurationWriter,
            StructuresCalculation<HeightStructuresInput>,
            HeightStructuresCalculationConfiguration>
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
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Export_ValidData_ReturnTrueAndWritesFile(string fileName, ICalculationBase[] calculations)
        {
            // Setup
            string testDirectory = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.HeightStructures.IO,
                nameof(HeightStructuresCalculationConfigurationExporter));

            string expectedXmlFilePath = Path.Combine(testDirectory, $"{fileName}.xml");

            // Call and Assert
            WriteAndValidate(calculations, expectedXmlFilePath);
        }

        private static ICalculation CreateSparseCalculation()
        {
            return new StructuresCalculation<HeightStructuresInput>
            {
                Name = "sparse config"
            };
        }

        private static StructuresCalculation<HeightStructuresInput> CreateFullCalculation()
        {
            return new TestHeightStructuresCalculationScenario
            {
                Name = "full config",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1"),
                    Structure = new TestHeightStructure("kunstwerk1"),
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    FailureProbabilityStructureWithErosion = 1e-6,
                    StructureNormalOrientation = (RoundedDouble) 67.1,
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
                        CoefficientOfVariation = (RoundedDouble) 1.2222
                    },
                    ModelFactorSuperCriticalFlow = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 1.10,
                        StandardDeviation = (RoundedDouble) 1.2222
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
                    LevelCrestStructure = new NormalDistribution
                    {
                        Mean = (RoundedDouble) 4.3,
                        StandardDeviation = (RoundedDouble) 0.2
                    },
                    CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    }
                }
            };
        }

        private static StructuresCalculation<HeightStructuresInput> CreateCalculationWithForeshoreProfile()
        {
            return new StructuresCalculation<HeightStructuresInput>
            {
                Name = "with foreshore profile",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    UseForeshore = true
                }
            };
        }

        private static StructuresCalculation<HeightStructuresInput> CreateCalculationWithUseBreakWater()
        {
            return new StructuresCalculation<HeightStructuresInput>
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

        private static StructuresCalculation<HeightStructuresInput> CreateCalculationWithStructure()
        {
            return new StructuresCalculation<HeightStructuresInput>
            {
                Name = "with structure",
                InputParameters =
                {
                    Structure = new TestHeightStructure("kunstwerk1")
                }
            };
        }

        protected override StructuresCalculation<HeightStructuresInput> CreateCalculation()
        {
            return new TestHeightStructuresCalculationScenario();
        }

        protected override HeightStructuresCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new HeightStructuresCalculationConfigurationExporter(calculations, filePath);
        }
    }
}