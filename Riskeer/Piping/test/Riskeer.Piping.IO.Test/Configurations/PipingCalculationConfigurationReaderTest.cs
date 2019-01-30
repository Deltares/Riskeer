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
using System.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Piping.IO,
                                                                               nameof(PipingCalculationConfigurationReader));

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
                yield return new TestCaseData("invalidWaterLevelEmpty.xml",
                                              "The 'waterstand' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidWaterLevelEmpty");
                yield return new TestCaseData("invalidWaterLevelNoDouble.xml",
                                              "The 'waterstand' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidWaterLevelNoDouble");
                yield return new TestCaseData("invalidEntryPointEmpty.xml",
                                              "The 'intredepunt' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidEntryPointEmpty");
                yield return new TestCaseData("invalidEntryPointNoDouble.xml",
                                              "The 'intredepunt' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidEntryPointNoDouble");
                yield return new TestCaseData("invalidExitPointEmpty.xml",
                                              "The 'uittredepunt' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidExitPointEmpty");
                yield return new TestCaseData("invalidExitPointNoDouble.xml",
                                              "The 'uittredepunt' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidExitPointNoDouble");
                yield return new TestCaseData("invalidStochastNoName.xml",
                                              "The required attribute 'naam' is missing.")
                    .SetName("invalidStochastNoName");
                yield return new TestCaseData("invalidStochastUnknownName.xml",
                                              "The 'naam' attribute is invalid - The value 'Test' is invalid according to its datatype 'nameType' - The Enumeration constraint failed.")
                    .SetName("invalidStochastUnknownName");
                yield return new TestCaseData("invalidStochastMultipleMean.xml",
                                              "Element 'verwachtingswaarde' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidStochastMultipleMean");
                yield return new TestCaseData("invalidStochastMultipleStandardDeviation.xml",
                                              "Element 'standaardafwijking' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidStochastMultipleStandardDeviation");
                yield return new TestCaseData("invalidStochastMeanEmpty.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastMeanEmpty");
                yield return new TestCaseData("invalidStochastMeanNoDouble.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastMeanNoDouble");
                yield return new TestCaseData("invalidStochastStandardDeviationEmpty.xml",
                                              "The 'standaardafwijking' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationEmpty");
                yield return new TestCaseData("invalidStochastStandardDeviationNoDouble.xml",
                                              "The 'standaardafwijking' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationNoDouble");
                yield return new TestCaseData("invalidMultiplePhreaticLevelExitStochast.xml",
                                              "There is a duplicate key sequence 'polderpeil' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultiplePhreaticLevelExitStochast");
                yield return new TestCaseData("invalidMultipleDampingFactorExitStochast.xml",
                                              "There is a duplicate key sequence 'dempingsfactor' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleDampingFactorExitStochast");
                yield return new TestCaseData("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocationOld.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocationNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothAssessmentLevelAndHydraulicBoundaryLocationNew");
                yield return new TestCaseData("invalidContainingBothWaterLevelAndHydraulicBoundaryLocationOld.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothWaterLevelAndHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidContainingBothWaterLevelAndHydraulicBoundaryLocationNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothWaterLevelAndHydraulicBoundaryLocationNew");
                yield return new TestCaseData("invalidCalculationMultipleAssessmentLevel.xml",
                                              "Element 'toetspeil' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleAssessmentLevel");
                yield return new TestCaseData("invalidCalculationMultipleWaterLevel.xml",
                                              "Element 'waterstand' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleWaterLevel");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocationOld.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocationNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocationNew");
                yield return new TestCaseData("invalidCalculationMultipleSurfaceLine.xml",
                                              "Element 'profielschematisatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleSurfaceLine");
                yield return new TestCaseData("invalidCalculationMultipleEntryPoint.xml",
                                              "Element 'intredepunt' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleEntryPoint");
                yield return new TestCaseData("invalidCalculationMultipleExitPoint.xml",
                                              "Element 'uittredepunt' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleExitPoint");
                yield return new TestCaseData("invalidCalculationMultipleStochasticSoilModel.xml",
                                              "Element 'ondergrondmodel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStochasticSoilModel");
                yield return new TestCaseData("invalidCalculationMultipleStochasticSoilProfile.xml",
                                              "Element 'ondergrondschematisatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStochasticSoilProfile");
                yield return new TestCaseData("invalidCalculationMultipleStochasts.xml",
                                              "Element 'stochasten' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStochasts");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationOld.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationNew.xml",
                                              "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationNew");
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
                yield return new TestCaseData("invalidContainingBothAssessmentLevelAndWaterLevel.xml",
                                              "Element 'toetspeil' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothAssessmentLevelAndWaterLevel");
                yield return new TestCaseData("invalidContainingBothHydraulicBoundaryLocationOldAndNew.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidContainingBothHydraulicBoundaryLocationOldAndNew");
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new PipingCalculationConfigurationReader(filePath);

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
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<PipingCalculationConfiguration>>(reader);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.SurfaceLineName);
            Assert.IsNull(calculation.EntryPointL);
            Assert.IsNull(calculation.ExitPointL);
            Assert.IsNull(calculation.StochasticSoilModelName);
            Assert.IsNull(calculation.StochasticSoilProfileName);
            Assert.IsNull(calculation.PhreaticLevelExit);
            Assert.IsNull(calculation.DampingFactorExit);
            Assert.IsNull(calculation.Scenario);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingEmptyStochasts_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingEmptyStochasts.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.PhreaticLevelExit);
            Assert.IsNull(calculation.DampingFactorExit);
        }

        [Test]
        [TestCase("validConfigurationCalculationContainingAssessmentLevelAndNaNs")]
        [TestCase("validConfigurationCalculationContainingWaterLevelAndNaNs")]
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadPipingCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNaN(calculation.AssessmentLevel);
            Assert.IsNaN(calculation.EntryPointL);
            Assert.IsNaN(calculation.ExitPointL);
            Assert.IsNaN(calculation.PhreaticLevelExit.Mean);
            Assert.IsNaN(calculation.PhreaticLevelExit.StandardDeviation);
            Assert.IsNaN(calculation.DampingFactorExit.Mean);
            Assert.IsNaN(calculation.DampingFactorExit.StandardDeviation);
            Assert.IsNaN(calculation.Scenario.Contribution);
        }

        [Test]
        [TestCase("validConfigurationCalculationContainingAssessmentLevelAndInfinities")]
        [TestCase("validConfigurationCalculationContainingWaterLevelAndInfinities")]
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadPipingCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation.AssessmentLevel);
            Assert.IsNotNull(calculation.EntryPointL);
            Assert.IsNotNull(calculation.ExitPointL);
            Assert.IsNotNull(calculation.PhreaticLevelExit.Mean);
            Assert.IsNotNull(calculation.PhreaticLevelExit.StandardDeviation);
            Assert.IsNotNull(calculation.DampingFactorExit.Mean);
            Assert.IsNotNull(calculation.DampingFactorExit.StandardDeviation);
            Assert.IsNotNull(calculation.Scenario.Contribution);

            Assert.IsTrue(double.IsNegativeInfinity(calculation.AssessmentLevel.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.EntryPointL.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.ExitPointL.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.PhreaticLevelExit.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.PhreaticLevelExit.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.DampingFactorExit.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.DampingFactorExit.StandardDeviation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.Scenario.Contribution.Value));
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocationOld")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocationNew")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder_old")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder_new")]
        public void Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnExpectedReadPipingCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.AreEqual("Locatie", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("Profielschematisatie", calculation.SurfaceLineName);
            Assert.AreEqual(2.2, calculation.EntryPointL);
            Assert.AreEqual(3.3, calculation.ExitPointL);
            Assert.AreEqual("Ondergrondmodel", calculation.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfileName);
            Assert.AreEqual(4.4, calculation.PhreaticLevelExit.Mean);
            Assert.AreEqual(5.5, calculation.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(6.6, calculation.DampingFactorExit.Mean);
            Assert.AreEqual(7.7, calculation.DampingFactorExit.StandardDeviation);
            Assert.AreEqual(8.8, calculation.Scenario.Contribution);
            Assert.IsFalse(calculation.Scenario.IsRelevant);
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel")]
        [TestCase("validConfigurationFullCalculationContainingWaterLevel")]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel_differentOrder")]
        [TestCase("validConfigurationFullCalculationContainingWaterLevel_differentOrder")]
        public void Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnExpectedReadPipingCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", calculation.Name);
            Assert.AreEqual(1.1, calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("Profielschematisatie", calculation.SurfaceLineName);
            Assert.AreEqual(2.2, calculation.EntryPointL);
            Assert.AreEqual(3.3, calculation.ExitPointL);
            Assert.AreEqual("Ondergrondmodel", calculation.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfileName);
            Assert.AreEqual(4.4, calculation.PhreaticLevelExit.Mean);
            Assert.AreEqual(5.5, calculation.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(6.6, calculation.DampingFactorExit.Mean);
            Assert.AreEqual(7.7, calculation.DampingFactorExit.StandardDeviation);
            Assert.AreEqual(8.8, calculation.Scenario.Contribution);
            Assert.IsFalse(calculation.Scenario.IsRelevant);
        }

        [Test]
        public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.SurfaceLineName);
            Assert.IsNull(calculation.EntryPointL);
            Assert.AreEqual(2.2, calculation.ExitPointL);
            Assert.IsNull(calculation.StochasticSoilModelName);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfileName);
            Assert.AreEqual(3.3, calculation.PhreaticLevelExit.Mean);
            Assert.AreEqual(4.4, calculation.PhreaticLevelExit.StandardDeviation);
            Assert.IsNull(calculation.DampingFactorExit);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastMean_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationStochastsNoMean.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.PhreaticLevelExit.Mean);
            Assert.AreEqual(0.1, calculation.PhreaticLevelExit.StandardDeviation);
            Assert.IsNull(calculation.DampingFactorExit.Mean);
            Assert.AreEqual(7.7, calculation.DampingFactorExit.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastStandardDeviation_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationStochastsNoStandardDeviation.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual(0.0, calculation.PhreaticLevelExit.Mean);
            Assert.IsNull(calculation.PhreaticLevelExit.StandardDeviation);
            Assert.AreEqual(6.6, calculation.DampingFactorExit.Mean);
            Assert.IsNull(calculation.DampingFactorExit.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyStochast_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyStochasts.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.PhreaticLevelExit.Mean);
            Assert.IsNull(calculation.PhreaticLevelExit.StandardDeviation);
            Assert.IsNull(calculation.DampingFactorExit.Mean);
            Assert.IsNull(calculation.DampingFactorExit.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyScenario_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyScenario.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (PipingCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.Scenario.Contribution);
            Assert.IsNull(calculation.Scenario.IsRelevant);
        }
    }
}