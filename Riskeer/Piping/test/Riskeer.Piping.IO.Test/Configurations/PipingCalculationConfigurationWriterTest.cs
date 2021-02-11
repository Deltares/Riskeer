// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            PipingCalculationConfigurationWriter,
            PipingCalculationConfiguration>
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Piping.IO,
                                                                          nameof(PipingCalculationConfigurationWriter));

        [Test]
        public void Write_CalculationGroupsAndCalculation_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Write_CalculationGroupsAndCalculation_ValidFile));

            var calculationGroup = new CalculationGroupConfiguration("PK001_0001", new IConfigurationItem[]
            {
                CreateFullSemiProbabilisticCalculationConfiguration(),
                CreateFullProbabilisticCalculationConfiguration(),
                new CalculationGroupConfiguration("PK001_0002", new[]
                {
                    CreateSparseCalculationConfiguration("Sparse semi-probabilistisch",
                                                         PipingCalculationConfigurationType.SemiProbabilistic),
                    CreateSparseCalculationConfiguration("Sparse probabilistisch",
                                                         PipingCalculationConfigurationType.Probabilistic)
                })
            });

            var writer = new PipingCalculationConfigurationWriter(filePath);

            try
            {
                // Call
                writer.Write(new[]
                {
                    calculationGroup
                });

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string pathToExpectedFile = Path.Combine(testDataPath, "folderWithSubfolderAndCalculation.xml");
                string expectedXml = File.ReadAllText(pathToExpectedFile);
                string actualXml = File.ReadAllText(filePath);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static PipingCalculationConfiguration CreateFullSemiProbabilisticCalculationConfiguration()
        {
            return new PipingCalculationConfiguration("PK001_0001 W1-6_0_1D1 semi-probabilistisch", PipingCalculationConfigurationType.SemiProbabilistic)
            {
                AssessmentLevel = 10,
                HydraulicBoundaryLocationName = "PUNT_KAT_18",
                SurfaceLineName = "PK001_0001",
                StochasticSoilModelName = "PK001_0001_Piping",
                StochasticSoilProfileName = "W1-6_0_1D1",
                EntryPointL = 0.1,
                ExitPointL = 0.2,
                PhreaticLevelExit = new StochastConfiguration
                {
                    Mean = 0,
                    StandardDeviation = 0.1
                },
                DampingFactorExit = new StochastConfiguration
                {
                    Mean = 0.7,
                    StandardDeviation = 0.1
                },
                Scenario = new ScenarioConfiguration
                {
                    IsRelevant = true,
                    Contribution = 0.3
                },
                ShouldProfileSpecificIllustrationPointsBeCalculated = true,
                ShouldSectionSpecificIllustrationPointsBeCalculated = false
            };
        }

        private static PipingCalculationConfiguration CreateFullProbabilisticCalculationConfiguration()
        {
            return new PipingCalculationConfiguration("PK001_0001 W1-6_0_1D1 probabilistisch", PipingCalculationConfigurationType.Probabilistic)
            {
                AssessmentLevel = 15,
                HydraulicBoundaryLocationName = "PUNT_KAT_18",
                SurfaceLineName = "PK001_0001",
                StochasticSoilModelName = "PK001_0001_Piping",
                StochasticSoilProfileName = "W1-6_0_1D1",
                EntryPointL = 0.5,
                ExitPointL = 0.8,
                PhreaticLevelExit = new StochastConfiguration
                {
                    Mean = 0.4,
                    StandardDeviation = 0.2
                },
                DampingFactorExit = new StochastConfiguration
                {
                    Mean = 0.8,
                    StandardDeviation = 0.2
                },
                Scenario = new ScenarioConfiguration
                {
                    IsRelevant = true,
                    Contribution = 0.6
                },
                ShouldProfileSpecificIllustrationPointsBeCalculated = true,
                ShouldSectionSpecificIllustrationPointsBeCalculated = true
            };
        }

        private static PipingCalculationConfiguration CreateSparseCalculationConfiguration(string name, PipingCalculationConfigurationType calculationType)
        {
            return new PipingCalculationConfiguration(name, calculationType);
        }

        protected override PipingCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new PipingCalculationConfigurationWriter(filePath);
        }
    }
}