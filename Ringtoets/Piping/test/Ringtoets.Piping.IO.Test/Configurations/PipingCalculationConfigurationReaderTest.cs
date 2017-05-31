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
using Ringtoets.Piping.IO.Configurations;

namespace Ringtoets.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
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
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.SurfaceLine);
            Assert.IsNull(calculation.EntryPointL);
            Assert.IsNull(calculation.ExitPointL);
            Assert.IsNull(calculation.StochasticSoilModel);
            Assert.IsNull(calculation.StochasticSoilProfile);
            Assert.IsNull(calculation.PhreaticLevelExitMean);
            Assert.IsNull(calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(calculation.DampingFactorExitMean);
            Assert.IsNull(calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingEmptyStochasts_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingEmptyStochasts.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.IsNull(calculation.PhreaticLevelExitMean);
            Assert.IsNull(calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(calculation.DampingFactorExitMean);
            Assert.IsNull(calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingNaNs.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.IsNaN(calculation.AssessmentLevel);
            Assert.IsNaN(calculation.EntryPointL);
            Assert.IsNaN(calculation.ExitPointL);
            Assert.IsNaN(calculation.PhreaticLevelExitMean);
            Assert.IsNaN(calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNaN(calculation.DampingFactorExitMean);
            Assert.IsNaN(calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingInfinities.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];

            Assert.IsNotNull(calculation.AssessmentLevel);
            Assert.IsNotNull(calculation.EntryPointL);
            Assert.IsNotNull(calculation.ExitPointL);
            Assert.IsNotNull(calculation.PhreaticLevelExitMean);
            Assert.IsNotNull(calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNotNull(calculation.DampingFactorExitMean);
            Assert.IsNotNull(calculation.DampingFactorExitStandardDeviation);

            Assert.IsTrue(double.IsNegativeInfinity(calculation.AssessmentLevel.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.EntryPointL.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.ExitPointL.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.PhreaticLevelExitMean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.PhreaticLevelExitStandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.DampingFactorExitMean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.DampingFactorExitStandardDeviation.Value));
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnPipingCalculation(HydraulicBoundaryLocation)")]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation_differentOrder.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnPipingCalculation(HydraulicBoundaryLocation_differentOrder)")]
        public void Read_ValidConfigurationWithFullCalculationContainingHydraulicBoundaryLocation_ReturnExpectedReadPipingCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.AreEqual("HRlocatie", calculation.HydraulicBoundaryLocation);
            Assert.AreEqual("Profielschematisatie", calculation.SurfaceLine);
            Assert.AreEqual(2.2, calculation.EntryPointL);
            Assert.AreEqual(3.3, calculation.ExitPointL);
            Assert.AreEqual("Ondergrondmodel", calculation.StochasticSoilModel);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfile);
            Assert.AreEqual(4.4, calculation.PhreaticLevelExitMean);
            Assert.AreEqual(5.5, calculation.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(6.6, calculation.DampingFactorExitMean);
            Assert.AreEqual(7.7, calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnPipingCalculation(AssessmentLevel)")]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel_differentOrder.xml",
            TestName = "Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnPipingCalculation(AssessmentLevel_differentOrder)")]
        public void Read_ValidConfigurationWithFullCalculationContainingAssessmentLevel_ReturnExpectedReadPipingCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.AreEqual(1.1, calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocation);
            Assert.AreEqual("Profielschematisatie", calculation.SurfaceLine);
            Assert.AreEqual(2.2, calculation.EntryPointL);
            Assert.AreEqual(3.3, calculation.ExitPointL);
            Assert.AreEqual("Ondergrondmodel", calculation.StochasticSoilModel);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfile);
            Assert.AreEqual(4.4, calculation.PhreaticLevelExitMean);
            Assert.AreEqual(5.5, calculation.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(6.6, calculation.DampingFactorExitMean);
            Assert.AreEqual(7.7, calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadPipingCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.AreEqual(1.1, calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.SurfaceLine);
            Assert.IsNull(calculation.EntryPointL);
            Assert.AreEqual(2.2, calculation.ExitPointL);
            Assert.IsNull(calculation.StochasticSoilModel);
            Assert.AreEqual("Ondergrondschematisatie", calculation.StochasticSoilProfile);
            Assert.AreEqual(3.3, calculation.PhreaticLevelExitMean);
            Assert.AreEqual(4.4, calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(calculation.DampingFactorExitMean);
            Assert.IsNull(calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastMean_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationStochastsNoMean.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.IsNull(calculation.PhreaticLevelExitMean);
            Assert.AreEqual(0.1, calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(calculation.DampingFactorExitMean);
            Assert.AreEqual(7.7, calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastStandardDeviation_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationStochastsNoStandardDeviation.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.AreEqual(0.0, calculation.PhreaticLevelExitMean);
            Assert.IsNull(calculation.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(6.6, calculation.DampingFactorExitMean);
            Assert.IsNull(calculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyStochast_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyStochasts.xml");
            var reader = new PipingCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (PipingCalculationConfiguration) readConfigurationItems[0];
            Assert.IsNull(calculation.PhreaticLevelExitMean);
            Assert.IsNull(calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(calculation.DampingFactorExitMean);
            Assert.IsNull(calculation.DampingFactorExitStandardDeviation);
        }
    }
}