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
using Ringtoets.GrassCoverErosionInwards.IO.Readers;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Readers
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                               nameof(GrassCoverErosionInwardsCalculationConfigurationReader));

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidHydraulicBoundaryLocationEmpty.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidHydraulicBoundaryLocationEmpty");
                yield return new TestCaseData("invalidMultipleHydraulicBoundaryLocations.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleHydraulicBoundaryLocations");

                yield return new TestCaseData("invalidDikeProfileEmpty.xml",
                                              "The 'dijkprofiel' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidDikeProfileEmpty");
                yield return new TestCaseData("invalidMultipleDikeProfiles.xml",
                                              "Element 'dijkprofiel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleDikeProfiles");

                yield return new TestCaseData("invalidOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationEmpty");
                yield return new TestCaseData("invalidOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationNoDouble");
                yield return new TestCaseData("invalidMultipleOrientation.xml",
                                              "Element 'orientatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleOrientation");

                yield return new TestCaseData("invalidDikeHeightEmpty.xml",
                                              "The 'dijkhoogte' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidDikeHeightEmpty");
                yield return new TestCaseData("invalidDikeHeightNoDouble.xml",
                                              "The 'dijkhoogte' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidDikeHeightNoDouble");
                yield return new TestCaseData("invalidMultipleDikeHeight.xml",
                                              "Element 'dijkhoogte' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleDikeHeight");

                yield return new TestCaseData("invalidDikeHeightCalculationTypeEmpty.xml",
                                              "The 'hbnberekenen' element is invalid - The value '' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidDikeHeightCalculationTypeEmpty");
                yield return new TestCaseData("invalidMultipleDikeHeightCalculationTypes.xml",
                                              "Element 'hbnberekenen' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleDikeHeightCalculationTypes");
                yield return new TestCaseData("invalidDikeHeightCalculationTypeUnsupportedString.xml",
                                              "The 'hbnberekenen' element is invalid - The value 'invalid' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidDikeHeightCalculationTypeUnsupportedString");

                yield return new TestCaseData("invalidOvertoppingRateCalculationTypeEmpty.xml",
                                              "The 'overslagdebietberekenen' element is invalid - The value '' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidOvertoppingRateCalculationTypeEmpty");
                yield return new TestCaseData("invalidMultipleOvertoppingRateCalculationTypes.xml",
                                              "Element 'overslagdebietberekenen' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleOvertoppingRateCalculationTypes");
                yield return new TestCaseData("invalidOvertoppingRateCalculationTypeUnsupportedString.xml",
                                              "The 'overslagdebietberekenen' element is invalid - The value 'invalid' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidOvertoppingRateCalculationTypeUnsupportedString");

                yield return new TestCaseData("invalidUseBreakWaterEmpty.xml",
                                              "The 'damgebruiken' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidUseBreakWaterEmpty");
                yield return new TestCaseData("invalidUseBreakWaterNoBoolean.xml",
                                              "The 'damgebruiken' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidUseBreakWaterNoBoolean");
                yield return new TestCaseData("invalidMultipleUseBreakWaters.xml",
                                              "Element 'damgebruiken' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleUseBreakWaters");

                yield return new TestCaseData("invalidBreakWaterTypeEmpty.xml",
                                              "The 'damtype' element is invalid - The value '' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidBreakWaterTypeEmpty");
                yield return new TestCaseData("invalidMultipleBreakWaterTypes.xml",
                                              "Element 'damtype' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleBreakWaterTypes");
                yield return new TestCaseData("invalidBreakWaterTypeUnsupportedString.xml",
                                              "The 'damtype' element is invalid - The value 'invalid' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidBreakWaterTypeUnsupportedString");

                yield return new TestCaseData("invalidBreakWaterHeightEmpty.xml",
                                              "The 'damhoogte' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidBreakWaterHeightEmpty");
                yield return new TestCaseData("invalidBreakWaterHeightNoDouble.xml",
                                              "The 'damhoogte' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidBreakWaterHeightNoDouble");
                yield return new TestCaseData("invalidMultipleBreakWaterHeights.xml",
                                              "Element 'damhoogte' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleBreakWaterHeights");

                yield return new TestCaseData("invalidUseForeshoreEmpty.xml",
                                              "The 'voorlandgebruiken' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidUseForeshoreEmpty");
                yield return new TestCaseData("invalidUseForeshoreNoBoolean.xml",
                                              "The 'voorlandgebruiken' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidUseForeshoreNoBoolean");
                yield return new TestCaseData("invalidMultipleUseForeshore.xml",
                                              "Element 'voorlandgebruiken' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleUseForeshores");

                yield return new TestCaseData("invalidMultipleCriticalFlowRateStochast.xml",
                                              "There is a duplicate key sequence 'overslagdebiet' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleCriticalFlowRateStochast");

                yield return new TestCaseData("invalidCriticalFlowRateMeanEmpty.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCriticalFlowRateMeanEmpty");
                yield return new TestCaseData("invalidCriticalFlowRateMeanNoDouble.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidCriticalFlowRateMeanNoDouble");
                yield return new TestCaseData("invalidMultipleCriticalFlowRateMean.xml",
                                              "Element 'verwachtingswaarde' cannot appear more than once if content model type is \"all\"")
                    .SetName("invalidMultipleCriticalFlowRateMean");

                yield return new TestCaseData("invalidCriticalFlowRateStandardDeviationEmpty.xml",
                                              "The 'standaardafwijking' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCriticalFlowRateStandardDeviationEmpty");
                yield return new TestCaseData("invalidCriticalFlowRateStandardDeviationNoDouble.xml",
                                              "The 'standaardafwijking' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidCriticalFlowRateStandardDeviationNoDouble");
                yield return new TestCaseData("invalidMultipleCriticalFlowRateStandardDeviation.xml",
                                              "Element 'standaardafwijking' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleCriticalFlowRateStandardDeviation");

                yield return new TestCaseData("invalidStochastNoName.xml",
                                              "The required attribute 'naam' is missing.")
                    .SetName("invalidStochastNoName");
                yield return new TestCaseData("invalidStochastUnknownName.xml",
                                              "The 'naam' attribute is invalid - The value 'unsupported' is invalid according to its datatype 'nameType' - The Enumeration constraint failed.")
                    .SetName("invalidStochastUnknownName");
                yield return new TestCaseData("invalidMultipleStochasts.xml",
                                              "Element 'stochasten' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidMultipleStochasts");
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

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
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<ReadGrassCoverErosionInwardsCalculation>>(reader);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.DikeProfile);
            Assert.IsNull(calculation.Orientation);
            Assert.IsNull(calculation.DikeHeight);
            Assert.IsNull(calculation.DikeHeightCalculationType);
            Assert.IsNull(calculation.OvertoppingRateCalculationType);
            Assert.IsNull(calculation.UseBreakWater);
            Assert.IsNull(calculation.BreakWaterType);
            Assert.IsNull(calculation.BreakWaterHeight);
            Assert.IsNull(calculation.UseForeshore);
            Assert.IsNull(calculation.CriticalFlowRateMean);
            Assert.IsNull(calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingEmptyStochasts_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingEmptyStochasts.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.CriticalFlowRateMean);
            Assert.IsNull(calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingNaNs.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.IsNaN(calculation.Orientation);
            Assert.IsNaN(calculation.DikeHeight);
            Assert.IsNaN(calculation.BreakWaterHeight);
            Assert.IsNaN(calculation.CriticalFlowRateMean);
            Assert.IsNaN(calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingInfinities.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);

            Assert.IsNotNull(calculation.Orientation);
            Assert.IsNotNull(calculation.DikeHeight);
            Assert.IsNotNull(calculation.BreakWaterHeight);
            Assert.IsNotNull(calculation.CriticalFlowRateMean);
            Assert.IsNotNull(calculation.CriticalFlowRateStandardDeviation);

            Assert.IsTrue(double.IsPositiveInfinity(calculation.Orientation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.DikeHeight.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.BreakWaterHeight.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.CriticalFlowRateMean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.CriticalFlowRateStandardDeviation.Value));
        }

        [Test]
        [TestCase("validConfigurationFullCalculation.xml",
            TestName = "Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadCalculation(FullCalculation)")]
        [TestCase("validConfigurationFullCalculation_differentOrder.xml",
            TestName = "Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadCalculation(FullCalculation_differentOrder)")]
        public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.AreEqual("Berekening 1", calculation.Name);
            Assert.AreEqual("Some_hydraulic_boundary_location", calculation.HydraulicBoundaryLocation);
            Assert.AreEqual("some_dike_profile", calculation.DikeProfile);
            Assert.AreEqual(67.1, calculation.Orientation);
            Assert.AreEqual(3.45, calculation.DikeHeight);
            Assert.AreEqual(ReadHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm, calculation.DikeHeightCalculationType);
            Assert.AreEqual(ReadHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability, calculation.OvertoppingRateCalculationType);
            Assert.AreEqual(true, calculation.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, calculation.BreakWaterType);
            Assert.AreEqual(1.234, calculation.BreakWaterHeight);
            Assert.AreEqual(false, calculation.UseForeshore);
            Assert.AreEqual(0.1, calculation.CriticalFlowRateMean);
            Assert.AreEqual(0.2, calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.AreEqual("Partial calculation 2", calculation.Name);
            Assert.IsNull(calculation.HydraulicBoundaryLocation);
            Assert.AreEqual("name_of_dikeprofile", calculation.DikeProfile);
            Assert.IsNull(calculation.Orientation);
            Assert.AreEqual(-1.2, calculation.DikeHeight);
            Assert.IsNull(calculation.DikeHeightCalculationType);
            Assert.IsNull(calculation.OvertoppingRateCalculationType);
            Assert.AreEqual(false, calculation.UseBreakWater);
            Assert.IsNull(calculation.BreakWaterType);
            Assert.AreEqual(3.4, calculation.BreakWaterHeight);
            Assert.IsNull(calculation.UseForeshore);
            Assert.IsNull(calculation.CriticalFlowRateMean);
            Assert.IsNull(calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastMean_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCriticalFlowRateMissingMean.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.CriticalFlowRateMean);
            Assert.AreEqual(2.2, calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastStandardDeviation_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCriticalFlowRateMissingStandardDeviation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.AreEqual(1.1, calculation.CriticalFlowRateMean);
            Assert.IsNull(calculation.CriticalFlowRateStandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyStochast_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCriticalFlowRate.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = readConfigurationItems[0] as ReadGrassCoverErosionInwardsCalculation;
            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.CriticalFlowRateMean);
            Assert.IsNull(calculation.CriticalFlowRateStandardDeviation);
        }
    }
}