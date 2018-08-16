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
using Ringtoets.GrassCoverErosionInwards.IO.Configurations;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Configurations
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
                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationOldAndNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationHydraulicBoundaryLocationOldAndNew");

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

                yield return new TestCaseData("invalidShouldOvertoppingOutputIllustrationPointsBeCalculatedEmpty.xml",
                                              "The 'illustratiepunteninlezen' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldOvertoppingOutputIllustrationPointsBeCalculatedEmpty");
                yield return new TestCaseData("invalidShouldOvertoppingOutputIllustrationPointsBeCalculatedNoBoolean.xml",
                                              "The 'illustratiepunteninlezen' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldOvertoppingOutputIllustrationPointsBeCalculatedNoBoolean");

                yield return new TestCaseData("invalidShouldDikeHeightIllustrationPointsBeCalculatedEmpty.xml",
                                              "The 'hbnillustratiepunteninlezen' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldDikeHeightIllustrationPointsBeCalculatedEmpty");
                yield return new TestCaseData("invalidShouldDikeHeightIllustrationPointsBeCalculatedNoBoolean.xml",
                                              "The 'hbnillustratiepunteninlezen' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldDikeHeightIllustrationPointsBeCalculatedNoBoolean");

                yield return new TestCaseData("invalidShouldOvertoppingRateIllustrationPointsBeCalculatedEmpty.xml",
                                              "The 'overslagdebietillustratiepunteninlezen' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldOvertoppingRateIllustrationPointsBeCalculatedEmpty");
                yield return new TestCaseData("invalidShouldOvertoppingRateIllustrationPointsBeCalculatedNoBoolean.xml",
                                              "The 'overslagdebietillustratiepunteninlezen' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldOvertoppingRateIllustrationPointsBeCalculatedNoBoolean");
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
            Assert.IsInstanceOf<CalculationConfigurationReader<GrassCoverErosionInwardsCalculationConfiguration>>(reader);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.DikeProfileId);
            Assert.IsNull(calculation.Orientation);
            Assert.IsNull(calculation.DikeHeight);
            Assert.IsNull(calculation.DikeHeightCalculationType);
            Assert.IsNull(calculation.OvertoppingRateCalculationType);
            Assert.IsNull(calculation.WaveReduction);
            Assert.IsNull(calculation.CriticalFlowRate);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingEmptyStochasts_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingEmptyStochasts.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.CriticalFlowRate);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingNaNs.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNaN(calculation.Orientation);
            Assert.IsNaN(calculation.DikeHeight);
            Assert.IsNaN(calculation.WaveReduction.BreakWaterHeight);
            Assert.IsNaN(calculation.CriticalFlowRate.Mean);
            Assert.IsNaN(calculation.CriticalFlowRate.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingInfinities.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);

            Assert.IsNotNull(calculation.Orientation);
            Assert.IsNotNull(calculation.DikeHeight);
            Assert.IsNotNull(calculation.WaveReduction.BreakWaterHeight);
            Assert.IsNotNull(calculation.CriticalFlowRate.Mean);
            Assert.IsNotNull(calculation.CriticalFlowRate.StandardDeviation);

            Assert.IsTrue(double.IsPositiveInfinity(calculation.Orientation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.DikeHeight.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.WaveReduction.BreakWaterHeight.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.CriticalFlowRate.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.CriticalFlowRate.StandardDeviation.Value));
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
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.AreEqual("Berekening 1", calculation.Name);
            Assert.AreEqual("Some_hydraulic_boundary_location", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("some_dike_profile", calculation.DikeProfileId);
            Assert.AreEqual(67.1, calculation.Orientation);
            Assert.AreEqual(3.45, calculation.DikeHeight);
            Assert.AreEqual(ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm, calculation.DikeHeightCalculationType);
            Assert.AreEqual(ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability, calculation.OvertoppingRateCalculationType);
            Assert.AreEqual(true, calculation.WaveReduction.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.234, calculation.WaveReduction.BreakWaterHeight);
            Assert.AreEqual(false, calculation.WaveReduction.UseForeshoreProfile);
            Assert.AreEqual(0.1, calculation.CriticalFlowRate.Mean);
            Assert.AreEqual(0.2, calculation.CriticalFlowRate.StandardDeviation);
            Assert.IsTrue(calculation.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsTrue(calculation.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsFalse(calculation.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }

        [Test]
        public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.AreEqual("Partial calculation 2", calculation.Name);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("id_of_dikeprofile", calculation.DikeProfileId);
            Assert.IsNull(calculation.Orientation);
            Assert.AreEqual(-1.2, calculation.DikeHeight);
            Assert.IsNull(calculation.DikeHeightCalculationType);
            Assert.IsNull(calculation.OvertoppingRateCalculationType);
            Assert.AreEqual(false, calculation.WaveReduction.UseBreakWater);
            Assert.IsNull(calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(3.4, calculation.WaveReduction.BreakWaterHeight);
            Assert.IsNull(calculation.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(calculation.CriticalFlowRate);
            Assert.IsTrue(calculation.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsNull(calculation.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsNull(calculation.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastMean_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCriticalFlowRateMissingMean.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.CriticalFlowRate.Mean);
            Assert.AreEqual(2.2, calculation.CriticalFlowRate.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithMissingStochastStandardDeviation_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCriticalFlowRateMissingStandardDeviation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.AreEqual(1.1, calculation.CriticalFlowRate.Mean);
            Assert.IsNull(calculation.CriticalFlowRate.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyStochast_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCriticalFlowRate.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.CriticalFlowRate.Mean);
            Assert.IsNull(calculation.CriticalFlowRate.StandardDeviation);
        }
    }
}