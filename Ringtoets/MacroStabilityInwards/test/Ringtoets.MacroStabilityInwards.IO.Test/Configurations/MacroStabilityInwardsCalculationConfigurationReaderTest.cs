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
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
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
            Assert.IsNull(configuration.PhreaticLine2);
            Assert.IsNull(configuration.PhreaticLine3);
            Assert.IsNull(configuration.PhreaticLine4);
            Assert.IsNull(configuration.LocationInputDaily);
            Assert.IsNull(configuration.LocationInputExtreme);
            Assert.IsNull(configuration.SlipPlaneMinimumDepth);
            Assert.IsNull(configuration.SlipPlaneMinimumLength);
            Assert.IsNull(configuration.MaximumSliceWidth);
            Assert.IsNull(configuration.CreateZones);
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
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadMacroStabilityInwardsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingNaNs.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.IsNaN(configuration.AssessmentLevel);
            Assert.IsNaN(configuration.Scenario.Contribution);
            Assert.IsNaN(configuration.WaterLevelRiverAverage);
            Assert.IsNaN(configuration.XCoordinateDrainageConstruction);
            Assert.IsNaN(configuration.ZCoordinateDrainageConstruction);
            Assert.IsNaN(configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNaN(configuration.PhreaticLine2.Inwards);
            Assert.IsNaN(configuration.PhreaticLine2.Outwards);
            Assert.IsNaN(configuration.PhreaticLine3.Inwards);
            Assert.IsNaN(configuration.PhreaticLine3.Outwards);
            Assert.IsNaN(configuration.PhreaticLine4.Inwards);
            Assert.IsNaN(configuration.PhreaticLine4.Outwards);
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
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadMacroStabilityInwardsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingInfinities.xml");
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];

            Assert.IsNotNull(configuration.AssessmentLevel);
            Assert.IsNotNull(configuration.Scenario.Contribution);

            Assert.IsTrue(double.IsNegativeInfinity(configuration.AssessmentLevel.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.Scenario.Contribution.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.WaterLevelRiverAverage.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.XCoordinateDrainageConstruction.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.ZCoordinateDrainageConstruction.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.MinimumLevelPhreaticLineAtDikeTopRiver.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.MinimumLevelPhreaticLineAtDikeTopPolder.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLine2.Inwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLine2.Outwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLine3.Inwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLine3.Outwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLine4.Inwards.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.PhreaticLine4.Outwards.Value));
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
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnCalculation(HydraulicBoundaryLocation)")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnCalculation(HydraulicBoundaryLocation_differentOrder)")]
        public void Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnExpectedReadMacroStabilityInwardsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", configuration.Name);
            Assert.IsNull(configuration.AssessmentLevel);
            Assert.AreEqual("HRlocatie", configuration.HydraulicBoundaryLocationName);

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

            MacroStabilityInwardsPhreaticLineConfiguration phreaticLine3Configuration = configuration.PhreaticLine3;
            Assert.IsNotNull(phreaticLine3Configuration);
            Assert.AreEqual(10.1, phreaticLine3Configuration.Inwards);
            Assert.AreEqual(10.2, phreaticLine3Configuration.Outwards);

            MacroStabilityInwardsPhreaticLineConfiguration phreaticLine4Configuration = configuration.PhreaticLine4;
            Assert.IsNotNull(phreaticLine4Configuration);
            Assert.AreEqual(10.3, phreaticLine4Configuration.Inwards);
            Assert.AreEqual(10.4, phreaticLine4Configuration.Outwards);

            MacroStabilityInwardsPhreaticLineConfiguration phreaticLine2Configuration = configuration.PhreaticLine2;
            Assert.IsNotNull(phreaticLine2Configuration);
            Assert.AreEqual(20.1, phreaticLine2Configuration.Inwards);
            Assert.AreEqual(20.2, phreaticLine2Configuration.Outwards);

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
            Assert.AreEqual(3, rightGridConfiguration.ZTop);
            Assert.AreEqual(4, rightGridConfiguration.ZBottom);
            Assert.AreEqual(5, rightGridConfiguration.NumberOfVerticalPoints);
            Assert.AreEqual(6, rightGridConfiguration.NumberOfHorizontalPoints);

            ScenarioConfiguration scenarioConfiguration = configuration.Scenario;
            Assert.IsNotNull(scenarioConfiguration);
            Assert.AreEqual(8.8, scenarioConfiguration.Contribution);
            Assert.IsFalse(scenarioConfiguration.IsRelevant);
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnCalculation(AssessmentLevel)")]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel_differentOrder.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnCalculation(AssessmentLevel_differentOrder)")]
        public void Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnExpectedReadMacroStabilityInwardsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new MacroStabilityInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
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

            MacroStabilityInwardsPhreaticLineConfiguration phreaticLine3Configuration = configuration.PhreaticLine3;
            Assert.IsNotNull(phreaticLine3Configuration);
            Assert.AreEqual(10.1, phreaticLine3Configuration.Inwards);
            Assert.AreEqual(10.2, phreaticLine3Configuration.Outwards);

            MacroStabilityInwardsPhreaticLineConfiguration phreaticLine4Configuration = configuration.PhreaticLine4;
            Assert.IsNotNull(phreaticLine4Configuration);
            Assert.AreEqual(10.3, phreaticLine4Configuration.Inwards);
            Assert.AreEqual(10.4, phreaticLine4Configuration.Outwards);

            MacroStabilityInwardsPhreaticLineConfiguration phreaticLine2Configuration = configuration.PhreaticLine2;
            Assert.IsNotNull(phreaticLine2Configuration);
            Assert.AreEqual(20.1, phreaticLine2Configuration.Inwards);
            Assert.AreEqual(20.2, phreaticLine2Configuration.Outwards);

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
            Assert.AreEqual(3, rightGridConfiguration.ZTop);
            Assert.AreEqual(4, rightGridConfiguration.ZBottom);
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
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var configuration = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", configuration.Name);
            Assert.AreEqual(1.1, configuration.AssessmentLevel);
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
            Assert.AreEqual(20.1, configuration.PhreaticLine2.Inwards);
            Assert.AreEqual(20.2, configuration.PhreaticLine2.Outwards);
            Assert.AreEqual(10.1, configuration.PhreaticLine3.Inwards);
            Assert.AreEqual(10.2, configuration.PhreaticLine3.Outwards);
            Assert.IsNull(configuration.PhreaticLine4);
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
        }

        private static IEnumerable<TestCaseData> InvalidTypeTestCases()
        {
            const string message = "The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}'";

            const string dataTypeDouble = "Double";
            foreach (NameAdapter adapter in GetNameAdaptersOfDoubleProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(message, adapter.ElementName, "", dataTypeDouble));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeDouble}.xml",
                                              string.Format(message, adapter.ElementName, "string", dataTypeDouble));
            }

            const string dataTypeBoolean = "Boolean";
            foreach (NameAdapter adapter in GetNameAdaptersOfBoolProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(message, adapter.ElementName, "", dataTypeBoolean));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeBoolean}.xml",
                                              string.Format(message, adapter.ElementName, "string", dataTypeBoolean));
            }

            const string dataTypeInteger = "Integer";
            foreach (NameAdapter adapter in GetNameAdaptersOfIntegerProperties())
            {
                yield return new TestCaseData($"invalid{adapter.PropertyName}Empty.xml",
                                              string.Format(message, adapter.ElementName, "", dataTypeInteger));
                yield return new TestCaseData($"invalid{adapter.PropertyName}No{dataTypeInteger}.xml",
                                              string.Format(message, adapter.ElementName, "string", dataTypeInteger));
            }

            const string stringMessage = "The '{0}' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.";
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
            yield return new NameAdapter("HydraulicBoundaryLocation", "hrlocatie");
            yield return new NameAdapter("SurfaceLine", "profielschematisatie");
            yield return new NameAdapter("StochasticSoilModel", "ondergrondmodel");
            yield return new NameAdapter("StochasticSoilProfile", "ondergrondschematisatie");
        }

        private static IEnumerable<NameAdapter> GetNameAdaptersOfEnumerationProperties()
        {
            yield return new NameAdapter("DikeSoilScenario", "dijktype");
            yield return new NameAdapter("GridDeterminationType", "bepaling");
            yield return new NameAdapter("TangentLineDeterminationType", "bepalingtangentlijnen");
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
        }

        private static IEnumerable<TestCaseData> InvalidDuplicateElements()
        {
            const string message = "Element '{0}' cannot appear more than once if content model type is \"all\".";

            foreach (NameAdapter adapter in GetNameAdaptersOfAllProperties())
            {
                yield return new TestCaseData($"invalidCalculationMultiple{adapter.PropertyName}.xml",
                                              string.Format(message, adapter.ElementName));
            }

            yield return new TestCaseData("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocation.xml",
                                          string.Format(message, "hrlocatie"));
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