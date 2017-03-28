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
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Ringtoets.HeightStructures.IO.Test
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationExporterTest
        : CustomSchemaCalculationConfigurationExporterDesignGuidelinesTestFixture<
            HeightStructuresCalculationConfigurationExporter,
            HeightStructuresCalculationConfigurationWriter, 
            StructuresCalculation<HeightStructuresInput>, 
            HeightStructureCalculationConfiguration>
    {
        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            StructuresCalculation<HeightStructuresInput> calculation = CreateFullCalculation();
            StructuresCalculation<HeightStructuresInput> calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Name = "Berekening 2"
            };

            CalculationGroup calculationGroup2 = new CalculationGroup("Nested", false)
            {
                Children =
                {
                    calculation2
                }
            };

            CalculationGroup calculationGroup = new CalculationGroup("Testmap", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO,
                                                                    Path.Combine(nameof(HeightStructuresCalculationConfigurationWriter),
                                                                                 "folderWithSubfolderAndCalculation.xml"));

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        private static StructuresCalculation<HeightStructuresInput> CreateFullCalculation()
        {
            return new TestHeightStructuresCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1"),
                    Structure = new TestHeightStructure("kunstwerk1"),
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    FailureProbabilityStructureWithErosion = 1e-6,
                    StructureNormalOrientation = (RoundedDouble) 67.1,
                    UseBreakWater = true,
                    UseForeshore = true,
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = (RoundedDouble) 1.234
                    },
                    StormDuration = new VariationCoefficientLogNormalDistribution()
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    ModelFactorSuperCriticalFlow = new NormalDistribution()
                    {
                        Mean = (RoundedDouble) 1.10
                    },
                    FlowWidthAtBottomProtection = new LogNormalDistribution()
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures = new NormalDistribution()
                    {
                        Mean = (RoundedDouble) 13.2,
                        StandardDeviation = (RoundedDouble) 0.3
                    },
                    StorageStructureArea = new VariationCoefficientLogNormalDistribution()
                    {
                        Mean = (RoundedDouble) 15000,
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    AllowedLevelIncreaseStorage = new LogNormalDistribution()
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    LevelCrestStructure = new NormalDistribution()
                    {
                        Mean = (RoundedDouble) 4.3,
                        StandardDeviation = (RoundedDouble) 0.2
                    },
                    CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution()
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    }
                }
            };
        }

        protected override StructuresCalculation<HeightStructuresInput> CreateCalculation()
        {
            return new TestHeightStructuresCalculation();
        }

        protected override HeightStructuresCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new HeightStructuresCalculationConfigurationExporter(calculations, filePath);
        }
    }
}