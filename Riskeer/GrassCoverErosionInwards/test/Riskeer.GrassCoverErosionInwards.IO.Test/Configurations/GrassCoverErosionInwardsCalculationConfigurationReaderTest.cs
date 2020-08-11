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
using Riskeer.GrassCoverErosionInwards.IO.Configurations;

namespace Riskeer.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.GrassCoverErosionInwards.IO,
                                                                               nameof(GrassCoverErosionInwardsCalculationConfigurationReader));

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidHydraulicBoundaryLocationEmpty.xml",
                                              "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidHydraulicBoundaryLocationEmpty");
                yield return new TestCaseData("invalidMultipleHydraulicBoundaryLocations.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
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
            void Call() => new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.AreEqual("Calculation", configuration.Name);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.DikeProfileId);
            Assert.IsNull(configuration.Orientation);
            Assert.IsNull(configuration.DikeHeight);
            Assert.IsNull(configuration.DikeHeightCalculationType);
            Assert.IsNull(configuration.OvertoppingRateCalculationType);
            Assert.IsNull(configuration.WaveReduction);
            Assert.IsNull(configuration.CriticalFlowRate);
            Assert.IsNull(configuration.Scenario);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.IsNull(configuration.CriticalFlowRate);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.IsNaN(configuration.Orientation);
            Assert.IsNaN(configuration.DikeHeight);
            Assert.IsNaN(configuration.WaveReduction.BreakWaterHeight);
            Assert.IsNaN(configuration.CriticalFlowRate.Mean);
            Assert.IsNaN(configuration.CriticalFlowRate.StandardDeviation);
            Assert.IsNaN(configuration.Scenario.Contribution);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);

            Assert.IsNotNull(configuration.Orientation);
            Assert.IsNotNull(configuration.DikeHeight);
            Assert.IsNotNull(configuration.WaveReduction.BreakWaterHeight);
            Assert.IsNotNull(configuration.CriticalFlowRate.Mean);
            Assert.IsNotNull(configuration.CriticalFlowRate.StandardDeviation);
            Assert.IsNotNull(configuration.Scenario.Contribution);

            Assert.IsTrue(double.IsPositiveInfinity(configuration.Orientation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.DikeHeight.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.WaveReduction.BreakWaterHeight.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.CriticalFlowRate.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.CriticalFlowRate.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.Scenario.Contribution.Value));
        }

        [Test]
        [TestCase("validConfigurationFullCalculation.xml")]
        [TestCase("validConfigurationFullCalculation_differentOrder.xml")]
        public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            AssertConfiguration(configuration);
        }

        [Test]
        public void Read_ValidPreviousVersionConfigurationWithFullCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "version0ValidConfigurationFullCalculation.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            AssertConfiguration(configuration);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.AreEqual("Partial calculation 2", configuration.Name);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual("id_of_dikeprofile", configuration.DikeProfileId);
            Assert.IsNull(configuration.Orientation);
            Assert.AreEqual(-1.2, configuration.DikeHeight);
            Assert.IsNull(configuration.DikeHeightCalculationType);
            Assert.IsNull(configuration.OvertoppingRateCalculationType);
            Assert.AreEqual(false, configuration.WaveReduction.UseBreakWater);
            Assert.IsNull(configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(3.4, configuration.WaveReduction.BreakWaterHeight);
            Assert.IsNull(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(configuration.CriticalFlowRate);
            Assert.IsTrue(configuration.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsNull(configuration.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsNull(configuration.ShouldOvertoppingRateIllustrationPointsBeCalculated);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.IsNull(configuration.CriticalFlowRate.Mean);
            Assert.AreEqual(2.2, configuration.CriticalFlowRate.StandardDeviation);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.AreEqual(1.1, configuration.CriticalFlowRate.Mean);
            Assert.IsNull(configuration.CriticalFlowRate.StandardDeviation);
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
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNotNull(configuration);
            Assert.IsNull(configuration.CriticalFlowRate.Mean);
            Assert.IsNull(configuration.CriticalFlowRate.StandardDeviation);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyScenario_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyScenario.xml");
            var reader = new GrassCoverErosionInwardsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (GrassCoverErosionInwardsCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(configuration.Scenario.Contribution);
            Assert.IsNull(configuration.Scenario.IsRelevant);
        }

        private static void AssertConfiguration(GrassCoverErosionInwardsCalculationConfiguration configuration)
        {
            Assert.IsNotNull(configuration);
            Assert.AreEqual("Berekening 1", configuration.Name);
            Assert.AreEqual("Some_hydraulic_boundary_location", configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual("some_dike_profile", configuration.DikeProfileId);
            Assert.AreEqual(67.1, configuration.Orientation);
            Assert.AreEqual(3.45, configuration.DikeHeight);
            Assert.AreEqual(ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm, configuration.DikeHeightCalculationType);
            Assert.AreEqual(ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability, configuration.OvertoppingRateCalculationType);
            Assert.AreEqual(true, configuration.WaveReduction.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.234, configuration.WaveReduction.BreakWaterHeight);
            Assert.AreEqual(false, configuration.WaveReduction.UseForeshoreProfile);
            Assert.AreEqual(0.1, configuration.CriticalFlowRate.Mean);
            Assert.AreEqual(0.2, configuration.CriticalFlowRate.StandardDeviation);
            Assert.IsTrue(configuration.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsTrue(configuration.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsFalse(configuration.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            Assert.AreEqual(8.8, configuration.Scenario.Contribution);
            Assert.IsTrue(configuration.Scenario.IsRelevant);
        }
    }
}