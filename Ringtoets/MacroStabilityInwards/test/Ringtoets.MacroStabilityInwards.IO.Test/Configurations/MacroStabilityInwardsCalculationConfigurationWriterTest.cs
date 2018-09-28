// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            MacroStabilityInwardsCalculationConfigurationWriter,
            MacroStabilityInwardsCalculationConfiguration>
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                                                                          nameof(MacroStabilityInwardsCalculationConfigurationWriter));

        [Test]
        public void Write_CalculationGroupsAndCalculation_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Write_CalculationGroupsAndCalculation_ValidFile));

            var calculationGroup = new CalculationGroupConfiguration("PK001_0001", new IConfigurationItem[]
            {
                CreateFullCalculationConfiguration(),
                new CalculationGroupConfiguration("PK001_0002", new[]
                {
                    CreateSparseCalculationConfiguration()
                })
            });

            var writer = new MacroStabilityInwardsCalculationConfigurationWriter(filePath);

            try
            {
                // Call
                writer.Write(new[]
                {
                    calculationGroup
                });

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string expectedXmlFilePath = Path.Combine(testDataPath, "folderWithSubfolderAndCalculation.xml");
                string expectedXml = File.ReadAllText(expectedXmlFilePath);
                string actualXml = File.ReadAllText(filePath);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static MacroStabilityInwardsCalculationConfiguration CreateFullCalculationConfiguration()
        {
            return new MacroStabilityInwardsCalculationConfiguration("PK001_0001 W1-6_0_1D1")
            {
                AssessmentLevel = 10,
                HydraulicBoundaryLocationName = "PUNT_KAT_18",
                SurfaceLineName = "PK001_0001",
                StochasticSoilModelName = "PK001_0001_Macrostabiliteit",
                StochasticSoilProfileName = "W1-6_0_1D1",
                DikeSoilScenario = ConfigurationDikeSoilScenario.ClayDikeOnSand,
                SlipPlaneMinimumDepth = 0.4,
                SlipPlaneMinimumLength = 0.5,
                MaximumSliceWidth = 0.6,
                WaterLevelRiverAverage = 20,
                DrainageConstructionPresent = true,
                XCoordinateDrainageConstruction = 10.5,
                ZCoordinateDrainageConstruction = 10.6,
                MinimumLevelPhreaticLineAtDikeTopPolder = 11.1,
                MinimumLevelPhreaticLineAtDikeTopRiver = 11.2,
                PiezometricHeadPhreaticLine2Inwards = 60.1,
                PiezometricHeadPhreaticLine2Outwards = 60.2,
                LeakageLengthInwardsPhreaticLine3 = 70.1,
                LeakageLengthOutwardsPhreaticLine3 = 70.2,
                LeakageLengthInwardsPhreaticLine4 = 80.1,
                LeakageLengthOutwardsPhreaticLine4 = 80.2,
                LocationInputDaily = new MacroStabilityInwardsLocationInputConfiguration
                {
                    WaterLevelPolder = 99.9,
                    PhreaticLineOffsetBelowDikeToeAtPolder = 99.8,
                    PhreaticLineOffsetBelowDikeTopAtPolder = 99.7,
                    PhreaticLineOffsetBelowDikeTopAtRiver = 99.6,
                    PhreaticLineOffsetBelowShoulderBaseInside = 99.5,
                    UseDefaultOffsets = false
                },
                LocationInputExtreme = new MacroStabilityInwardsLocationInputExtremeConfiguration
                {
                    PenetrationLength = 42,
                    WaterLevelPolder = 199.9,
                    PhreaticLineOffsetBelowDikeToeAtPolder = 199.8,
                    PhreaticLineOffsetBelowDikeTopAtPolder = 199.7,
                    PhreaticLineOffsetBelowDikeTopAtRiver = 199.6,
                    PhreaticLineOffsetBelowShoulderBaseInside = 199.5,
                    UseDefaultOffsets = true
                },
                AdjustPhreaticLine3And4ForUplift = true,
                CreateZones = true,
                ZoningBoundariesDeterminationType = ConfigurationZoningBoundariesDeterminationType.Manual,
                ZoneBoundaryLeft = 20.0,
                ZoneBoundaryRight = 60.7,
                MoveGrid = true,
                GridDeterminationType = ConfigurationGridDeterminationType.Automatic,
                TangentLineDeterminationType = ConfigurationTangentLineDeterminationType.Specified,
                TangentLineZTop = 0.7,
                TangentLineZBottom = 0.8,
                TangentLineNumber = 9,
                LeftGrid = new MacroStabilityInwardsGridConfiguration
                {
                    XLeft = 1.0,
                    XRight = 1.2,
                    ZTop = 1.3,
                    ZBottom = 1.4,
                    NumberOfVerticalPoints = 0,
                    NumberOfHorizontalPoints = 1
                },
                RightGrid = new MacroStabilityInwardsGridConfiguration
                {
                    XLeft = 10.0,
                    XRight = 10.2,
                    ZTop = 10.3,
                    ZBottom = 10.4,
                    NumberOfVerticalPoints = 5,
                    NumberOfHorizontalPoints = 10
                },
                Scenario = new ScenarioConfiguration
                {
                    IsRelevant = true,
                    Contribution = 0.3
                }
            };
        }

        private static MacroStabilityInwardsCalculationConfiguration CreateSparseCalculationConfiguration()
        {
            return new MacroStabilityInwardsCalculationConfiguration("Sparse");
        }

        protected override MacroStabilityInwardsCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationWriter(filePath);
        }
    }
}