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

            var calculation = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.SurfaceLineName);
            Assert.IsNull(calculation.StochasticSoilModelName);
            Assert.IsNull(calculation.StochasticSoilProfileName);
            Assert.IsNull(calculation.Scenario);
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

            var calculation = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.IsNaN(calculation.AssessmentLevel);
            Assert.IsNaN(calculation.Scenario.Contribution);
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

            var calculation = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];

            Assert.IsNotNull(calculation.AssessmentLevel);
            Assert.IsNotNull(calculation.Scenario.Contribution);

            Assert.IsTrue(double.IsNegativeInfinity(calculation.AssessmentLevel.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.Scenario.Contribution.Value));
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

            var calculation = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.AreEqual("HRlocatie", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("Profielschematisatie", calculation.SurfaceLineName);
            Assert.AreEqual("Ondergrondmodel", calculation.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfileName);
            Assert.AreEqual(8.8, calculation.Scenario.Contribution);
            Assert.IsFalse(calculation.Scenario.IsRelevant);
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

            var calculation = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.AreEqual(1.1, calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("Profielschematisatie", calculation.SurfaceLineName);
            Assert.AreEqual("Ondergrondmodel", calculation.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfileName);
            Assert.AreEqual(8.8, calculation.Scenario.Contribution);
            Assert.IsFalse(calculation.Scenario.IsRelevant);
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

            var calculation = (MacroStabilityInwardsCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.AreEqual(1.1, calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.SurfaceLineName);
            Assert.IsNull(calculation.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfileName);
        }
    }
}