// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.MacroStabilityInwards.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                                                                               nameof(MacroStabilityInwardsCalculationConfigurationReader));

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Precondition
            Assert.IsTrue(File.Exists(filePath), $"File '{fileName}' does not exist");

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        [Test]
        public void Constructor_ValidConfiguration_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<MacroStabilityInwardsCalculationConfiguration>>(reader);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadMacroStabilityInwardsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", configuration.Name);
            Assert.IsNull(configuration.AssessmentLevel);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.SurfaceLineName);
            Assert.IsNull(configuration.StochasticSoilModelName);
            Assert.IsNull(configuration.StochasticSoilProfileName);
            Assert.IsNull(configuration.Scenario);
            Assert.IsNull(configuration.DikeSoilScenario);
            Assert.IsNull(configuration.WaterLevelRiverAverage);
            Assert.IsNull(configuration.DrainageConstructionPresent);
            Assert.IsNull(configuration.XCoordinateDrainageConstruction);
            Assert.IsNull(configuration.ZCoordinateDrainageConstruction);
            Assert.IsNull(configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNull(configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNull(configuration.AdjustPhreaticLine3And4ForUplift);
            Assert.IsNull(configuration.PiezometricHeadPhreaticLine2Inwards);
            Assert.IsNull(configuration.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNull(configuration.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNull(configuration.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNull(configuration.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNull(configuration.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNull(configuration.LocationInputDaily);
            Assert.IsNull(configuration.LocationInputExtreme);
            Assert.IsNull(configuration.SlipPlaneMinimumDepth);
            Assert.IsNull(configuration.SlipPlaneMinimumLength);
            Assert.IsNull(configuration.MaximumSliceWidth);
            Assert.IsNull(configuration.CreateZones);
            Assert.IsNull(configuration.ZoningBoundariesDeterminationType);
            Assert.IsNull(configuration.ZoneBoundaryLeft);
            Assert.IsNull(configuration.ZoneBoundaryRight);
            Assert.IsNull(configuration.GridDeterminationType);
            Assert.IsNull(configuration.MoveGrid);
            Assert.IsNull(configuration.TangentLineDeterminationType);
            Assert.IsNull(configuration.TangentLineZTop);
            Assert.IsNull(configuration.TangentLineZBottom);
            Assert.IsNull(configuration.TangentLineNumber);
            Assert.IsNull(configuration.LeftGrid);
            Assert.IsNull(configuration.RightGrid);
        }

        [Test]
        [TestCase("validConfigurationCalculationContainingAssessmentLevelAndNaNs")]
        [TestCase("validConfigurationCalculationContainingWaterLevelAndNaNs")]
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadMacroStabilityInwardsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNaN(configuration.AssessmentLevel);
            Assert.IsNaN(configuration.Scenario.Contribution);
            Assert.IsNaN(configuration.WaterLevelRiverAverage);
            Assert.IsNaN(configuration.XCoordinateDrainageConstruction);
            Assert.IsNaN(configuration.ZCoordinateDrainageConstruction);
            Assert.IsNaN(configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNaN(configuration.PiezometricHeadPhreaticLine2Inwards);
            Assert.IsNaN(configuration.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNaN(configuration.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNaN(configuration.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNaN(configuration.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNaN(configuration.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(configuration.LocationInputDaily.WaterLevelPolder);
            Assert.IsNaN(configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.IsNaN(configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(configuration.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(configuration.LocationInputExtreme.PenetrationLength);
            Assert.IsNaN(configuration.LocationInputExtreme.WaterLevelPolder);
            Assert.IsNaN(configuration.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(configuration.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.IsNaN(configuration.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(configuration.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(configuration.SlipPlaneMinimumDepth);
            Assert.IsNaN(configuration.SlipPlaneMinimumLength);
            Assert.IsNaN(configuration.MaximumSliceWidth);
            Assert.IsNaN(configuration.TangentLineZTop);
            Assert.IsNaN(configuration.TangentLineZBottom);
            Assert.IsNaN(configuration.LeftGrid.XLeft);
            Assert.IsNaN(configuration.LeftGrid.XRight);
            Assert.IsNaN(configuration.LeftGrid.ZTop);
            Assert.IsNaN(configuration.LeftGrid.ZBottom);
            Assert.IsNaN(configuration.RightGrid.XLeft);
            Assert.IsNaN(configuration.RightGrid.XRight);
            Assert.IsNaN(configuration.RightGrid.ZTop);
            Assert.IsNaN(configuration.RightGrid.ZBottom);
        }

        [Test]
        [TestCase("validConfigurationCalculationContainingAssessmentLevelAndInfinities")]
        [TestCase("validConfigurationCalculationContainingWaterLevelAndInfinities")]
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadMacroStabilityInwardsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration.AssessmentLevel);
            Assert.IsNotNull(configuration.Scenario.Contribution);

            Assert.IsTrue(double.IsNegativeInfinity(configuration.AssessmentLevel.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.Scenario.Contribution.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.WaterLevelRiverAverage.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.XCoordinateDrainageConstruction.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.ZCoordinateDrainageConstruction.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.MinimumLevelPhreaticLineAtDikeTopRiver.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.MinimumLevelPhreaticLineAtDikeTopPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PiezometricHeadPhreaticLine2Inwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PiezometricHeadPhreaticLine2Outwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeakageLengthInwardsPhreaticLine3.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeakageLengthOutwardsPhreaticLine3.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeakageLengthInwardsPhreaticLine4.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeakageLengthOutwardsPhreaticLine4.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputDaily.WaterLevelPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputExtreme.PenetrationLength.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputExtreme.WaterLevelPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.SlipPlaneMinimumDepth.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.SlipPlaneMinimumLength.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.MaximumSliceWidth.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.TangentLineZTop.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.TangentLineZBottom.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeftGrid.XLeft.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeftGrid.XRight.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeftGrid.ZTop.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LeftGrid.ZBottom.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.RightGrid.XLeft.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.RightGrid.XRight.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.RightGrid.ZTop.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.RightGrid.ZBottom.Value));
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocationOld")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocationNew")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder_old")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder_new")]
        public void Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnExpectedReadMacroStabilityInwardsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", configuration.Name);
            Assert.IsNull(configuration.AssessmentLevel);
            Assert.AreEqual("Locatie", configuration.HydraulicBoundaryLocationName);

            Assert.AreEqual("Profielschematisatie", configuration.SurfaceLineName);
            Assert.AreEqual("Ondergrondmodel", configuration.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", configuration.StochasticSoilProfileName);

            Assert.AreEqual(ConfigurationDikeSoilScenario.SandDikeOnClay, configuration.DikeSoilScenario);
            Assert.AreEqual(10.5, configuration.WaterLevelRiverAverage);

            Assert.IsTrue(configuration.DrainageConstructionPresent);
            Assert.AreEqual(10.6, configuration.XCoordinateDrainageConstruction);
            Assert.AreEqual(10.7, configuration.ZCoordinateDrainageConstruction);

            Assert.AreEqual(10.9, configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(10.8, configuration.MinimumLevelPhreaticLineAtDikeTopPolder);

            Assert.IsTrue(configuration.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(20.1, configuration.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(20.2, configuration.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(10.1, configuration.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(10.2, configuration.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(10.3, configuration.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(10.4, configuration.LeakageLengthOutwardsPhreaticLine4);

            MacroStabilityInwardsLocationInputConfiguration dailyConfiguration = configuration.LocationInputDaily;
            Assert.IsNotNull(dailyConfiguration);
            Assert.AreEqual(2.2, dailyConfiguration.WaterLevelPolder);
            Assert.IsTrue(dailyConfiguration.UseDefaultOffsets);
            Assert.AreEqual(2.21, dailyConfiguration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2.24, dailyConfiguration.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2.22, dailyConfiguration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2.23, dailyConfiguration.PhreaticLineOffsetBelowShoulderBaseInside);

            MacroStabilityInwardsLocationInputExtremeConfiguration extremeConfiguration = configuration.LocationInputExtreme;
            Assert.IsNotNull(extremeConfiguration);
            Assert.AreEqual(15.2, extremeConfiguration.WaterLevelPolder);
            Assert.AreEqual(16.2, extremeConfiguration.PenetrationLength);
            Assert.IsFalse(extremeConfiguration.UseDefaultOffsets);
            Assert.AreEqual(15.21, extremeConfiguration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(15.24, extremeConfiguration.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(15.22, extremeConfiguration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(15.23, extremeConfiguration.PhreaticLineOffsetBelowShoulderBaseInside);

            Assert.AreEqual(0.4, configuration.SlipPlaneMinimumDepth);
            Assert.AreEqual(0.5, configuration.SlipPlaneMinimumLength);
            Assert.AreEqual(0.6, configuration.MaximumSliceWidth);

            Assert.IsTrue(configuration.CreateZones);
            Assert.AreEqual(ConfigurationZoningBoundariesDeterminationType.Manual, configuration.ZoningBoundariesDeterminationType);
            Assert.AreEqual(10.0, configuration.ZoneBoundaryLeft);
            Assert.AreEqual(43.5, configuration.ZoneBoundaryRight);

            Assert.IsTrue(configuration.MoveGrid);
            Assert.AreEqual(ConfigurationGridDeterminationType.Automatic, configuration.GridDeterminationType);

            Assert.AreEqual(ConfigurationTangentLineDeterminationType.LayerSeparated, configuration.TangentLineDeterminationType);
            Assert.AreEqual(10, configuration.TangentLineZTop);
            Assert.AreEqual(1, configuration.TangentLineZBottom);
            Assert.AreEqual(5, configuration.TangentLineNumber);

            MacroStabilityInwardsGridConfiguration leftGridConfiguration = configuration.LeftGrid;
            Assert.IsNotNull(leftGridConfiguration);
            Assert.IsNaN(leftGridConfiguration.XLeft);
            Assert.IsNaN(leftGridConfiguration.XRight);
            Assert.IsNaN(leftGridConfiguration.ZTop);
            Assert.IsNaN(leftGridConfiguration.ZBottom);
            Assert.AreEqual(6, leftGridConfiguration.NumberOfVerticalPoints);
            Assert.AreEqual(5, leftGridConfiguration.NumberOfHorizontalPoints);

            MacroStabilityInwardsGridConfiguration rightGridConfiguration = configuration.RightGrid;
            Assert.IsNotNull(rightGridConfiguration);
            Assert.AreEqual(1, rightGridConfiguration.XLeft);
            Assert.AreEqual(2, rightGridConfiguration.XRight);
            Assert.AreEqual(4, rightGridConfiguration.ZTop);
            Assert.AreEqual(3, rightGridConfiguration.ZBottom);
            Assert.AreEqual(5, rightGridConfiguration.NumberOfVerticalPoints);
            Assert.AreEqual(6, rightGridConfiguration.NumberOfHorizontalPoints);

            ScenarioConfiguration scenarioConfiguration = configuration.Scenario;
            Assert.IsNotNull(scenarioConfiguration);
            Assert.AreEqual(8.8, scenarioConfiguration.Contribution);
            Assert.IsFalse(scenarioConfiguration.IsRelevant);
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel")]
        [TestCase("validConfigurationFullCalculationContainingWaterLevel")]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel_differentOrder")]
        [TestCase("validConfigurationFullCalculationContainingWaterLevel_differentOrder")]
        public void Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnExpectedReadMacroStabilityInwardsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", configuration.Name);
            Assert.AreEqual(1.1, configuration.AssessmentLevel);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual("Profielschematisatie", configuration.SurfaceLineName);
            Assert.AreEqual("Ondergrondmodel", configuration.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", configuration.StochasticSoilProfileName);

            Assert.AreEqual(ConfigurationDikeSoilScenario.SandDikeOnClay, configuration.DikeSoilScenario);
            Assert.AreEqual(10.5, configuration.WaterLevelRiverAverage);

            Assert.IsTrue(configuration.DrainageConstructionPresent);
            Assert.AreEqual(10.6, configuration.XCoordinateDrainageConstruction);
            Assert.AreEqual(10.7, configuration.ZCoordinateDrainageConstruction);

            Assert.AreEqual(10.9, configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(10.8, configuration.MinimumLevelPhreaticLineAtDikeTopPolder);

            Assert.IsTrue(configuration.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(20.1, configuration.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(20.2, configuration.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(10.1, configuration.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(10.2, configuration.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(10.3, configuration.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(10.4, configuration.LeakageLengthOutwardsPhreaticLine4);

            MacroStabilityInwardsLocationInputConfiguration dailyConfiguration = configuration.LocationInputDaily;
            Assert.IsNotNull(dailyConfiguration);
            Assert.AreEqual(2.2, dailyConfiguration.WaterLevelPolder);
            Assert.IsTrue(dailyConfiguration.UseDefaultOffsets);
            Assert.AreEqual(2.21, dailyConfiguration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2.24, dailyConfiguration.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2.22, dailyConfiguration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2.23, dailyConfiguration.PhreaticLineOffsetBelowShoulderBaseInside);

            MacroStabilityInwardsLocationInputExtremeConfiguration extremeConfiguration = configuration.LocationInputExtreme;
            Assert.IsNotNull(extremeConfiguration);
            Assert.AreEqual(15.2, extremeConfiguration.WaterLevelPolder);
            Assert.AreEqual(16.2, extremeConfiguration.PenetrationLength);
            Assert.IsFalse(extremeConfiguration.UseDefaultOffsets);
            Assert.AreEqual(15.21, extremeConfiguration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(15.24, extremeConfiguration.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(15.22, extremeConfiguration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(15.23, extremeConfiguration.PhreaticLineOffsetBelowShoulderBaseInside);

            Assert.AreEqual(0.4, configuration.SlipPlaneMinimumDepth);
            Assert.AreEqual(0.5, configuration.SlipPlaneMinimumLength);
            Assert.AreEqual(0.6, configuration.MaximumSliceWidth);

            Assert.IsTrue(configuration.CreateZones);
            Assert.AreEqual(ConfigurationZoningBoundariesDeterminationType.Manual, configuration.ZoningBoundariesDeterminationType);
            Assert.AreEqual(10.0, configuration.ZoneBoundaryLeft);
            Assert.AreEqual(43.5, configuration.ZoneBoundaryRight);

            Assert.IsTrue(configuration.MoveGrid);
            Assert.AreEqual(ConfigurationGridDeterminationType.Automatic, configuration.GridDeterminationType);

            Assert.AreEqual(ConfigurationTangentLineDeterminationType.LayerSeparated, configuration.TangentLineDeterminationType);
            Assert.AreEqual(10, configuration.TangentLineZTop);
            Assert.AreEqual(1, configuration.TangentLineZBottom);
            Assert.AreEqual(5, configuration.TangentLineNumber);

            MacroStabilityInwardsGridConfiguration leftGridConfiguration = configuration.LeftGrid;
            Assert.IsNotNull(leftGridConfiguration);
            Assert.IsNaN(leftGridConfiguration.XLeft);
            Assert.IsNaN(leftGridConfiguration.XRight);
            Assert.IsNaN(leftGridConfiguration.ZTop);
            Assert.IsNaN(leftGridConfiguration.ZBottom);
            Assert.AreEqual(6, leftGridConfiguration.NumberOfVerticalPoints);
            Assert.AreEqual(5, leftGridConfiguration.NumberOfHorizontalPoints);

            MacroStabilityInwardsGridConfiguration rightGridConfiguration = configuration.RightGrid;
            Assert.IsNotNull(rightGridConfiguration);
            Assert.AreEqual(1, rightGridConfiguration.XLeft);
            Assert.AreEqual(2, rightGridConfiguration.XRight);
            Assert.AreEqual(4, rightGridConfiguration.ZTop);
            Assert.AreEqual(3, rightGridConfiguration.ZBottom);
            Assert.AreEqual(5, rightGridConfiguration.NumberOfVerticalPoints);
            Assert.AreEqual(6, rightGridConfiguration.NumberOfHorizontalPoints);

            ScenarioConfiguration scenarioConfiguration = configuration.Scenario;
            Assert.IsNotNull(scenarioConfiguration);
            Assert.AreEqual(8.8, scenarioConfiguration.Contribution);
            Assert.IsFalse(scenarioConfiguration.IsRelevant);
        }

        [Test]
        public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadMacroStabilityInwardsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", configuration.Name);
            Assert.IsNull(configuration.AssessmentLevel);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.SurfaceLineName);
            Assert.IsNull(configuration.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", configuration.StochasticSoilProfileName);

            Assert.IsNull(configuration.Scenario);
            Assert.AreEqual(10.5, configuration.WaterLevelRiverAverage);
            Assert.AreEqual(10.6, configuration.XCoordinateDrainageConstruction);
            Assert.IsNull(configuration.ZCoordinateDrainageConstruction);
            Assert.AreEqual(10.9, configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNull(configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNull(configuration.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(20.1, configuration.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(20.2, configuration.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(10.1, configuration.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(10.2, configuration.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNull(configuration.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNull(configuration.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(configuration.LocationInputDaily.WaterLevelPolder);
            Assert.AreEqual(2.21, configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2.24, configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2.22, configuration.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2.23, configuration.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.LocationInputExtreme);
            Assert.IsNull(configuration.MaximumSliceWidth);
            Assert.IsNull(configuration.SlipPlaneMinimumLength);
            Assert.AreEqual(0.4, configuration.SlipPlaneMinimumDepth);
            Assert.AreEqual(10, configuration.TangentLineZTop);
            Assert.AreEqual(1, configuration.TangentLineZBottom);
            Assert.AreEqual(5, configuration.TangentLineNumber);
            Assert.IsNaN(configuration.LeftGrid.XLeft);
            Assert.IsNaN(configuration.LeftGrid.XRight);
            Assert.IsNaN(configuration.LeftGrid.ZTop);
            Assert.IsNaN(configuration.LeftGrid.ZBottom);
            Assert.IsNull(configuration.RightGrid);
        }

        private static IEnumerable<TestCaseData> InvalidConfigurations()
        {
            const string testName = "{0:100}";

            foreach (TestCaseData testCaseData in InvalidDuplicateElements())
            {
                yield return testCaseData.SetName(testName);
            }

            foreach (TestCaseData testCaseData in InvalidTypeTestCases())
            {
                yield return testCaseData.SetName(testName);
            }
        }

        private static IEnumerable<TestCaseData> InvalidTypeTestCases()
        {
            const string message = "The '{0}' element is invalid - The value '{1}' is invalid according to its datatype";

            const string dataTypeDouble = "Double";
            foreach (NameAdapter adapter in GetNameAdaptersOfDoubleProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(message, adapter.ElementName, ""));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeDouble}.xml",
                                              string.Format(message, adapter.ElementName, "string"));
            }

            const string dataTypeBoolean = "Boolean";
            foreach (NameAdapter adapter in GetNameAdaptersOfBoolProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(message, adapter.ElementName, ""));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeBoolean}.xml",
                                              string.Format(message, adapter.ElementName, "string"));
            }

            const string dataTypeInteger = "Integer";
            foreach (NameAdapter adapter in GetNameAdaptersOfIntegerProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(message, adapter.ElementName, ""));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeInteger}.xml",
                                              string.Format(message, adapter.ElementName, "string"));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeInteger}ButDouble.xml",
                                              string.Format(message, adapter.ElementName, "3.1415"));
            }

            const string stringMessage = "The '{0}' element is invalid - The value '' is invalid according to its datatype";
            foreach (NameAdapter adapter in GetNameAdaptersOfStringProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(stringMessage, adapter.ElementName));
            }

            const string dataTypeEnumeration = "Enumeration";
            const string enumMessage = "The '{0}' element is invalid - The value '{1}' is invalid according to its datatype 'String' - The Enumeration constraint failed.";
            foreach (NameAdapter adapter in GetNameAdaptersOfEnumerationProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(enumMessage, adapter.ElementName, ""));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeEnumeration}.xml",
                                              string.Format(enumMessage, adapter.ElementName, "string"));
            }
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfIntegerProperties()
        {
            yield return new NameAdapter("TangentLineNumber", "aantal");
            yield return new NameAdapter("LeftGridGridNumberOfHorizontalPoints", "aantalpuntenhorizontaal");
            yield return new NameAdapter("LeftGridGridNumberOfVerticalPoints", "aantalpuntenverticaal");
            yield return new NameAdapter("RightGridGridNumberOfHorizontalPoints", "aantalpuntenhorizontaal");
            yield return new NameAdapter("RightGridGridNumberOfVerticalPoints", "aantalpuntenverticaal");
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfStringProperties()
        {
            yield return new NameAdapter("HydraulicBoundaryLocationOld", "hrlocatie");
            yield return new NameAdapter("HydraulicBoundaryLocationNew", "hblocatie");
            yield return new NameAdapter("SurfaceLine", "profielschematisatie");
            yield return new NameAdapter("StochasticSoilModel", "ondergrondmodel");
            yield return new NameAdapter("StochasticSoilProfile", "ondergrondschematisatie");
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfEnumerationProperties()
        {
            yield return new NameAdapter("DikeSoilScenario", "dijktype");
            yield return new NameAdapter("GridDeterminationType", "bepaling");
            yield return new NameAdapter("TangentLineDeterminationType", "bepalingtangentlijnen");
            yield return new NameAdapter("ZoningBoundariesDeterminationType", "methode");
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfBoolProperties()
        {
            yield return new NameAdapter("IsRelevant", "gebruik");
            yield return new NameAdapter("CreateZones", "bepaling");
            yield return new NameAdapter("DrainageConstructionPresent", "aanwezig");
            yield return new NameAdapter("AdjustPhreaticLine3And4ForUplift", "corrigeervooropbarsten");
            yield return new NameAdapter("LocationInputDailyUseDefaultOffsets", "gebruikdefaults");
            yield return new NameAdapter("LocationInputExtremeUseDefaultOffsets", "gebruikdefaults");
            yield return new NameAdapter("MoveGrid", "verplaatsgrid");
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfDoubleProperties()
        {
            yield return new NameAdapter("AssessmentLevel", "toetspeil");
            yield return new NameAdapter("WaterLevel", "waterstand");
            yield return new NameAdapter("ScenarioContribution", "bijdrage");
            yield return new NameAdapter("SlipPlaneMinimumDepth", "minimaleglijvlakdiepte");
            yield return new NameAdapter("SlipPlaneMinimumLength", "minimaleglijvlaklengte");
            yield return new NameAdapter("MaximumSliceWidth", "maximalelamelbreedte");
            yield return new NameAdapter("WaterLevelRiverAverage", "gemiddeldhoogwater");
            yield return new NameAdapter("XCoordinateDrainageConstruction", "x");
            yield return new NameAdapter("ZCoordinateDrainageConstruction", "z");

            yield return new NameAdapter("MinimumLevelPhreaticLineAtDikeTopRiver", "buitenkruin");
            yield return new NameAdapter("MinimumLevelPhreaticLineAtDikeTopPolder", "binnenkruin");
            yield return new NameAdapter("TangentLineZTop", "zboven");
            yield return new NameAdapter("TangentLineZBottom", "zonder");

            yield return new NameAdapter("PiezometricHeadPhreaticLine2Inwards", "binnenwaarts");
            yield return new NameAdapter("PiezometricHeadPhreaticLine2Outwards", "buitenwaarts");
            yield return new NameAdapter("LeakageLengthInwardsPhreaticLine3", "binnenwaarts");
            yield return new NameAdapter("LeakageLengthOutwardsPhreaticLine3", "buitenwaarts");
            yield return new NameAdapter("LeakageLengthInwardsPhreaticLine4", "binnenwaarts");
            yield return new NameAdapter("LeakageLengthOutwardsPhreaticLine4", "buitenwaarts");

            yield return new NameAdapter("LocationInputExtremePenetrationLength", "indringingslengte");
            yield return new NameAdapter("LocationInputExtremeWaterLevelPolder", "polderpeil");
            yield return new NameAdapter("LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver", "buitenkruin");
            yield return new NameAdapter("LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder", "binnenkruin");
            yield return new NameAdapter("LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside", "insteekbinnenberm");
            yield return new NameAdapter("LocationInputExtremePhreaticLineOffsetBelowDikeToeAtPolder", "teendijkbinnenwaarts");

            yield return new NameAdapter("LocationInputDailyWaterLevelPolder", "polderpeil");
            yield return new NameAdapter("LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver", "buitenkruin");
            yield return new NameAdapter("LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder", "binnenkruin");
            yield return new NameAdapter("LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside", "insteekbinnenberm");
            yield return new NameAdapter("LocationInputDailyPhreaticLineOffsetBelowDikeToeAtPolder", "teendijkbinnenwaarts");

            yield return new NameAdapter("LeftGridXLeft", "xlinks");
            yield return new NameAdapter("LeftGridXRight", "xrechts");
            yield return new NameAdapter("LeftGridZTop", "zboven");
            yield return new NameAdapter("LeftGridZBottom", "zonder");

            yield return new NameAdapter("RightGridXLeft", "xlinks");
            yield return new NameAdapter("RightGridXRight", "xrechts");
            yield return new NameAdapter("RightGridZTop", "zboven");
            yield return new NameAdapter("RightGridZBottom", "zonder");

            yield return new NameAdapter("ZoneBoundaryLeft", "zoneringsgrenslinks");
            yield return new NameAdapter("ZoneBoundaryRight", "zoneringsgrensrechts");
        }

        private static IEnumerable<string> GetTagElements()
        {
            yield return "waterspanningen";
            yield return "drainage";
            yield return "initielehoogtepl1";
            yield return "leklengtespl3";
            yield return "leklengtespl4";
            yield return "stijghoogtespl2";
            yield return "dagelijks";
            yield return "extreem";
            yield return "offsets";
            yield return "zonering";
            yield return "grids";
            yield return "tangentlijnen";
            yield return "linkergrid";
            yield return "rechtergrid";
            yield return "scenario";
        }

        private static IEnumerable<TestCaseData> InvalidDuplicateElements()
        {
            const string message = "Element '{0}' cannot appear more than once if content model type is \"all\".";

            foreach (NameAdapter adapter in GetNameAdaptersOfAllProperties())
            {
                yield return new TestCaseData($"invalidCalculationMultiple{adapter.PropertyName}.xml",
                                              string.Format(message, adapter.ElementName));
            }

            foreach (string tagElement in GetTagElements())
            {
                yield return new TestCaseData($"invalidCalculationMultiple{tagElement}Tag.xml",
                                              string.Format(message, tagElement));
            }

            yield return new TestCaseData("invalidContainingBothAssessmentLevelAndWaterLevel.xml",
                                          string.Format(message, "toetspeil"));
            yield return new TestCaseData("invalidContainingBothHydraulicBoundaryLocationOldAndNew.xml",
                                          string.Format(message, "hrlocatie"));
            yield return new TestCaseData("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocationOld.xml",
                                          string.Format(message, "hrlocatie"));
            yield return new TestCaseData("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocationNew.xml",
                                          string.Format(message, "hblocatie"));
            yield return new TestCaseData("invalidContainingBothWaterLevelAndHydraulicBoundaryLocationOld.xml",
                                          string.Format(message, "hrlocatie"));
            yield return new TestCaseData("invalidContainingBothWaterLevelAndHydraulicBoundaryLocationNew.xml",
                                          string.Format(message, "hblocatie"));
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfAllProperties()
        {
            return GetNameAdaptersOfDoubleProperties().Concat(GetNameAdaptersOfBoolProperties())
                                                      .Concat(GetNameAdaptersOfIntegerProperties())
                                                      .Concat(GetNameAdaptersOfEnumerationProperties())
                                                      .Concat(GetNameAdaptersOfStringProperties());
        }

        /// <summary>
        /// Adapter class between the name of the property in the data model and the name of the corresponding xml element.
        /// </summary>
        private class NameAdapter
        {
            /// <summary>
            /// Creates a new instance of <see cref="NameAdapter"/>.
            /// </summary>
            /// <param name="propertyName">The name of the property in the data model.</param>
            /// <param name="elementName">The name of the identifier of the xml element.</param>
            public NameAdapter(string propertyName, string elementName)
            {
                PropertyName = propertyName;
                ElementName = elementName;
            }

            /// <summary>
            /// Gets the name of the property in the data model.
            /// </summary>
            public string PropertyName { get; }

            /// <summary>
            /// Gets the name of the identifier of the xml element.
            /// </summary>
            public string ElementName { get; }
        }
    }
}