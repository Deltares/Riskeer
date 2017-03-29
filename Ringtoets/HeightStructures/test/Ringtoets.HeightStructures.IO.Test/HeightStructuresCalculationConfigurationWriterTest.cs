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
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.HeightStructures.IO.Test
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationWriterTest
        : CustomSchemaCalculationConfigurationWriterDesignGuidelinesTestFixture<
            HeightStructuresCalculationConfigurationWriter,
            HeightStructuresCalculationConfiguration>
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.HeightStructures.IO,
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
                        new CalculationConfigurationGroup("Testmap", new IConfigurationItem[]
                        {
                            CreateFullCalculation(),
                            new CalculationConfigurationGroup("Nested", new IConfigurationItem[]
                            {
                                new HeightStructuresCalculationConfiguration("Berekening 2")
                            })
                        })
                    })
                    .SetName("Calculation configuration with none of its parameters set");
            }
        }

        protected override void AssertDefaultConstructedInstance(HeightStructuresCalculationConfigurationWriter writer)
        {
            Assert.IsInstanceOf<StructureCalculationConfigurationWriter<HeightStructuresCalculationConfiguration>>(writer);
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculation_ValidFile(string expectedFileName, IConfigurationItem[] configuration)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

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

        private static HeightStructuresCalculationConfiguration CreateFullCalculation()
        {
            return new HeightStructuresCalculationConfiguration("Berekening 1")
            {
                HydraulicBoundaryLocationName = "Locatie1",
                StructureName = "kunstwerk1",
                ForeshoreProfileName = "profiel1",
                FailureProbabilityStructureWithErosion = 1e-6,
                StructureNormalOrientation = 67.1,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = true,
                    BreakWaterType = ReadBreakWaterType.Dam,
                    BreakWaterHeight = 1.23,
                    UseForeshoreProfile = true
                },
                StormDuration = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = (RoundedDouble) 6.0
                },
                ModelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = (RoundedDouble) 1.10
                },
                FlowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = (RoundedDouble) 15.2,
                    StandardDeviation = (RoundedDouble) 0.1
                },
                WidthFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = (RoundedDouble) 13.2,
                    StandardDeviation = (RoundedDouble) 0.3
                },
                StorageStructureArea = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = (RoundedDouble) 15000,
                    VariationCoefficient = (RoundedDouble) 0.01
                },
                AllowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = (RoundedDouble) 0.2,
                    StandardDeviation = (RoundedDouble) 0.01
                },
                LevelCrestStructure = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = (RoundedDouble) 4.3,
                    StandardDeviation = (RoundedDouble) 0.2
                },
                CriticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = (RoundedDouble) 2,
                    VariationCoefficient = (RoundedDouble) 0.1
                }
            };
        }

        protected override HeightStructuresCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new HeightStructuresCalculationConfigurationWriter(filePath);
        }
    }
}