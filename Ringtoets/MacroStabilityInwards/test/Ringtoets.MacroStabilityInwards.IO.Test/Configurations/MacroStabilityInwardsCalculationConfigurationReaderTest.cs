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

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidAssessmentLevelEmpty.xml",
                                              "The 'toetspeil' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidAssessmentLevelEmpty");
                yield return new TestCaseData("invalidAssessmentLevelNoDouble.xml",
                                              "The 'toetspeil' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidAssessmentLevelNoDouble");
                yield return new TestCaseData("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocation.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocation");
                yield return new TestCaseData("invalidCalculationMultipleAssessmentLevel.xml",
                                              "Element 'toetspeil' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleAssessmentLevel");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocation.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocation");
                yield return new TestCaseData("invalidCalculationMultipleSurfaceLine.xml",
                                              "Element 'profielschematisatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleSurfaceLine");
                yield return new TestCaseData("invalidCalculationMultipleStochasticSoilModel.xml",
                                              "Element 'ondergrondmodel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStochasticSoilModel");
                yield return new TestCaseData("invalidCalculationMultipleStochasticSoilProfile.xml",
                                              "Element 'ondergrondschematisatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStochasticSoilProfile");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocation.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocation");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptySurfaceLine.xml",
                                              "The 'profielschematisatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptySurfaceLine");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptySoilModel.xml",
                                              "The 'ondergrondmodel' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptySoilModel");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptySoilProfile.xml",
                                              "The 'ondergrondschematisatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptySoilProfile");
                yield return new TestCaseData("invalidCalculationMultipleScenario.xml",
                                              "Element 'scenario' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleScenario");
                yield return new TestCaseData("invalidScenarioMultipleContribution.xml",
                                              "Element 'bijdrage' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidScenarioMultipleContribution");
                yield return new TestCaseData("invalidScenarioContributionEmpty.xml",
                                              "The 'bijdrage' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidScenarioContributionEmpty");
                yield return new TestCaseData("invalidScenarioContributionNoDouble.xml",
                                              "The 'bijdrage' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidScenarioContributionNoDouble");
                yield return new TestCaseData("invalidScenarioMultipleRelevant.xml",
                                              "Element 'gebruik' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidScenarioMultipleRelevant");
                yield return new TestCaseData("invalidScenarioRelevantEmpty.xml",
                                              "The 'gebruik' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidScenarioRelevantEmpty");
                yield return new TestCaseData("invalidScenarioRelevantNoBoolean.xml",
                                              "The 'gebruik' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidScenarioRelevantNoBoolean");
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

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
    }
}