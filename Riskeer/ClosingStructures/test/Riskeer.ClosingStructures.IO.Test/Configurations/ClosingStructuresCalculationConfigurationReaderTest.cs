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
using Riskeer.ClosingStructures.IO.Configurations;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Import;

namespace Riskeer.ClosingStructures.IO.Test.Configurations
{
    [TestFixture]
    public class ClosingStructuresCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.ClosingStructures.IO,
                                                                               nameof(ClosingStructuresCalculationConfigurationReader));

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionEmpty.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionEmpty");
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionNoDouble.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionNoDouble");

                yield return new TestCaseData("invalidCalculationForeshoreProfileEmpty.xml",
                                              "The 'voorlandprofiel' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationForeshoreProfileEmpty");

                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationEmpty.xml",
                                              "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationHydraulicBoundaryLocationEmptyNew");

                yield return new TestCaseData("invalidCalculationMultipleFailureProbabilityStructureWithErosion.xml",
                                              "Element 'faalkansgegevenerosiebodem' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleFailureProbabilityStructureWithErosion");
                yield return new TestCaseData("invalidCalculationMultipleForeshoreProfile.xml",
                                              "Element 'voorlandprofiel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleForeshoreProfile");

                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocation.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocation");
                yield return new TestCaseData("invalidCalculationMultipleOrientation.xml",
                                              "Element 'orientatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleOrientation");
                yield return new TestCaseData("invalidCalculationMultipleStructure.xml",
                                              "Element 'kunstwerk' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStructure");

                yield return new TestCaseData("invalidCalculationMultipleFactorStormDurationOpenStructure.xml",
                                              "Element 'factorstormduur' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleFactorStormDurationOpenStructure");
                yield return new TestCaseData("invalidCalculationMultipleFailureProbabilityOpenStructure.xml",
                                              "Element 'kansmislukkensluiting' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleFailureProbabilityOpenStructure");
                yield return new TestCaseData("invalidCalculationMultipleFailureProbabilityReparation.xml",
                                              "Element 'faalkansherstel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleFailureProbabilityReparation");
                yield return new TestCaseData("invalidCalculationMultipleIdenticalApertures.xml",
                                              "Element 'nrdoorstroomopeningen' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleIdenticalApertures");
                yield return new TestCaseData("invalidCalculationMultipleInflowModelTypes.xml",
                                              "Element 'instroommodel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleInflowModelTypes");
                yield return new TestCaseData("invalidCalculationMultipleProbabilityOpenStructureBeforeFlooding.xml",
                                              "Element 'kansopopenstaan' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleProbabilityOpenStructureBeforeFlooding");

                yield return new TestCaseData("invalidCalculationOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationEmpty");
                yield return new TestCaseData("invalidCalculationOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationNoDouble");

                yield return new TestCaseData("invalidCalculationStructureEmpty.xml",
                                              "The 'kunstwerk' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationStructureEmpty");

                yield return new TestCaseData("invalidCalculationFactorStormDurationOpenStructureEmpty.xml",
                                              "The 'factorstormduur' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFactorStormDurationOpenStructureEmpty");
                yield return new TestCaseData("invalidCalculationFactorStormDurationOpenStructureNoDouble.xml",
                                              "The 'factorstormduur' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFactorStormDurationOpenStructureNoDouble");

                yield return new TestCaseData("invalidCalculationFailureProbabilityOpenStructureEmpty.xml",
                                              "The 'kansmislukkensluiting' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityOpenStructureEmpty");
                yield return new TestCaseData("invalidCalculationFailureProbabilityOpenStructureNoDouble.xml",
                                              "The 'kansmislukkensluiting' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityOpenStructureNoDouble");

                yield return new TestCaseData("invalidCalculationFailureProbabilityReparationEmpty.xml",
                                              "The 'faalkansherstel' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityReparationEmpty");
                yield return new TestCaseData("invalidCalculationFailureProbabilityReparationNoDouble.xml",
                                              "The 'faalkansherstel' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityReparationNoDouble");

                yield return new TestCaseData("invalidCalculationProbabilityOpenStructureBeforeFloodingEmpty.xml",
                                              "The 'kansopopenstaan' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationProbabilityOpenStructureBeforeFloodingEmpty");
                yield return new TestCaseData("invalidCalculationProbabilityOpenStructureBeforeFloodingNoDouble.xml",
                                              "The 'kansopopenstaan' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationProbabilityOpenStructureBeforeFloodingNoDouble");

                yield return new TestCaseData("invalidCalculationIdenticalAperturesEmpty.xml",
                                              "The 'nrdoorstroomopeningen' element is invalid - The value '' is invalid according to its datatype 'Integer'")
                    .SetName("invalidCalculationIdenticalAperturesEmpty");
                yield return new TestCaseData("invalidCalculationIdenticalAperturesNoInt.xml",
                                              "The 'nrdoorstroomopeningen' element is invalid - The value 'nul' is invalid according to its datatype 'Integer'")
                    .SetName("invalidCalculationIdenticalAperturesNoInt");

                yield return new TestCaseData("invalidCalculationInflowModelTypeEmpty.xml",
                                              "The 'instroommodel' element is invalid - The value '' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidCalculationInflowModelTypeEmpty");
                yield return new TestCaseData("invalidCalculationInflowModelTypeUnsupportedString.xml",
                                              "The 'instroommodel' element is invalid - The value 'invalid' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidCalculationInflowModelTypeUnsupportedString");

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
                yield return new TestCaseData("invalidStochastMultipleVariationCoefficient.xml",
                                              "Element 'variatiecoefficient' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidStochastMultipleVariationCoefficient");
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
                yield return new TestCaseData("invalidStochastVariationCoefficientEmpty.xml",
                                              "The 'variatiecoefficient' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientEmpty");
                yield return new TestCaseData("invalidStochastVariationCoefficientNoDouble.xml",
                                              "The 'variatiecoefficient' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientNoDouble");

                yield return new TestCaseData("invalidMultipleAllowedLevelIncreaseStochast.xml",
                                              "There is a duplicate key sequence 'peilverhogingkomberging' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleAllowedLevelIncreaseStochast");
                yield return new TestCaseData("invalidMultipleCriticalOvertoppingDischargeStochast.xml",
                                              "There is a duplicate key sequence 'kritiekinstromenddebiet' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleCriticalOvertoppingDischargeStochast");
                yield return new TestCaseData("invalidMultipleFlowWidthAtBottomProtectionStochast.xml",
                                              "There is a duplicate key sequence 'breedtebodembescherming' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleFlowWidthAtBottomProtectionStochast");
                yield return new TestCaseData("invalidMultipleModelFactorSuperCriticalFlowStochast.xml",
                                              "There is a duplicate key sequence 'modelfactoroverloopdebiet' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleModelFactorSuperCriticalFlowStochast");
                yield return new TestCaseData("invalidMultipleStorageStructureAreaStochast.xml",
                                              "There is a duplicate key sequence 'kombergendoppervlak' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleStorageStructureAreaStochast");
                yield return new TestCaseData("invalidMultipleStormDurationStochast.xml",
                                              "There is a duplicate key sequence 'stormduur' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleStormDurationStochast");
                yield return new TestCaseData("invalidMultipleWidthFlowAperturesStochast.xml",
                                              "There is a duplicate key sequence 'breedtedoorstroomopening' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleWidthFlowAperturesStochast");

                yield return new TestCaseData("invalidMultipleAreaFlowAperturesStochast.xml",
                                              "There is a duplicate key sequence 'doorstroomoppervlak' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleAreaFlowAperturesStochast");
                yield return new TestCaseData("invalidMultipleDrainCoefficientStochast.xml",
                                              "There is a duplicate key sequence 'afvoercoefficient' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleDrainCoefficientStochast");
                yield return new TestCaseData("invalidMultipleInsideWaterLevelStochast.xml",
                                              "There is a duplicate key sequence 'binnenwaterstand' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleInsideWaterLevelStochast");
                yield return new TestCaseData("invalidMultipleLevelCrestStructureNotClosingStochast.xml",
                                              "There is a duplicate key sequence 'kruinhoogte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleLevelCrestStructureNotClosingStochast");
                yield return new TestCaseData("invalidMultipleThresholdHeightOpenWeirStochast.xml",
                                              "There is a duplicate key sequence 'drempelhoogte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleThresholdHeightOpenWeirStochast");

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

                yield return new TestCaseData("invalidShouldIllustrationPointsBeCalculatedEmpty.xml",
                                              "The 'illustratiepunteninlezen' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldIllustrationPointsBeCalculatedEmpty");
                yield return new TestCaseData("invalidShouldIllustrationPointsBeCalculatedNoBoolean.xml",
                                              "The 'illustratiepunteninlezen' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldIllustrationPointsBeCalculatedNoBoolean");

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
        public void Constructor_WithFilePath_ReturnsNewInstance()
        {
            // Setup
            string existingPath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new ClosingStructuresCalculationConfigurationReader(existingPath);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<ClosingStructuresCalculationConfiguration>>(reader);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            void Call() => new ClosingStructuresCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        [Test]
        [TestCase("validConfigurationEmptyStochasts")]
        [TestCase("validConfigurationEmptyCalculation")]
        public void Read_ValidConfigurationWithEmptyCalculationOrWithEmptyStochasts_NoValuesSet(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);

            Assert.IsNull(configuration.LevelCrestStructureNotClosing);
            Assert.IsNull(configuration.AllowedLevelIncreaseStorage);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection);
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow);
            Assert.IsNull(configuration.StorageStructureArea);
            Assert.IsNull(configuration.StormDuration);
            Assert.IsNull(configuration.WidthFlowApertures);
            Assert.IsNull(configuration.WaveReduction);
            Assert.IsNull(configuration.Scenario);
        }

        [Test]
        [TestCase("validFullConfiguration")]
        [TestCase("validFullConfiguration_differentOrder")]
        public void Read_ValidFullConfigurations_ExpectedValues(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            AssertConfiguration(configuration);
        }

        [Test]
        public void Read_ValidPreviousVersionConfigurationWithFullCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "version0ValidConfigurationFullCalculation.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            AssertConfiguration(configuration);
        }

        [Test]
        public void Read_ValidFullConfigurationsContainingInfinity_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validFullConfigurationContainingInfinity.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsTrue(double.IsNegativeInfinity(configuration.StructureNormalOrientation.Value));
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsTrue(double.IsPositiveInfinity(configuration.FactorStormDurationOpenStructure.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.ProbabilityOpenStructureBeforeFlooding.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.FailureProbabilityOpenStructure.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.FailureProbabilityReparation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.FailureProbabilityStructureWithErosion.Value));

            Assert.IsTrue(double.IsNegativeInfinity(configuration.DrainCoefficient.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.DrainCoefficient.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.InsideWaterLevel.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.InsideWaterLevel.StandardDeviation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.AreaFlowApertures.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.AreaFlowApertures.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.ThresholdHeightOpenWeir.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.ThresholdHeightOpenWeir.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.LevelCrestStructureNotClosing.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.LevelCrestStructureNotClosing.StandardDeviation.Value));

            Assert.IsTrue(double.IsPositiveInfinity(configuration.AllowedLevelIncreaseStorage.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.AllowedLevelIncreaseStorage.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.CriticalOvertoppingDischarge.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.CriticalOvertoppingDischarge.VariationCoefficient.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.FlowWidthAtBottomProtection.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.FlowWidthAtBottomProtection.StandardDeviation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.ModelFactorSuperCriticalFlow.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.ModelFactorSuperCriticalFlow.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.StorageStructureArea.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.StorageStructureArea.VariationCoefficient.Value));
            Assert.IsTrue(double.IsNegativeInfinity(configuration.StormDuration.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.StormDuration.VariationCoefficient.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.WidthFlowApertures.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(configuration.WidthFlowApertures.StandardDeviation.Value));

            Assert.IsNull(configuration.WaveReduction.BreakWaterType);
            Assert.IsTrue(double.IsNegativeInfinity(configuration.WaveReduction.BreakWaterHeight.Value));
            Assert.IsNull(configuration.WaveReduction.UseBreakWater);
            Assert.IsNull(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsTrue(double.IsPositiveInfinity(configuration.Scenario.Contribution.Value));
        }

        [Test]
        public void Read_ValidFullConfigurationsContainingNaN_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validFullConfigurationContainingNaN.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNaN(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNaN(configuration.FactorStormDurationOpenStructure);
            Assert.IsNaN(configuration.ProbabilityOpenStructureBeforeFlooding);
            Assert.IsNaN(configuration.FailureProbabilityOpenStructure);
            Assert.IsNaN(configuration.FailureProbabilityReparation);
            Assert.IsNaN(configuration.FailureProbabilityStructureWithErosion);

            Assert.IsNaN(configuration.DrainCoefficient.Mean);
            Assert.IsNaN(configuration.DrainCoefficient.StandardDeviation);
            Assert.IsNaN(configuration.InsideWaterLevel.Mean);
            Assert.IsNaN(configuration.InsideWaterLevel.StandardDeviation);
            Assert.IsNaN(configuration.AreaFlowApertures.Mean);
            Assert.IsNaN(configuration.AreaFlowApertures.StandardDeviation);
            Assert.IsNaN(configuration.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(configuration.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNaN(configuration.LevelCrestStructureNotClosing.Mean);
            Assert.IsNaN(configuration.LevelCrestStructureNotClosing.StandardDeviation);

            Assert.IsNaN(configuration.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(configuration.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(configuration.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(configuration.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNaN(configuration.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(configuration.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(configuration.ModelFactorSuperCriticalFlow.Mean);
            Assert.IsNaN(configuration.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.IsNaN(configuration.StorageStructureArea.Mean);
            Assert.IsNaN(configuration.StorageStructureArea.VariationCoefficient);
            Assert.IsNaN(configuration.StormDuration.Mean);
            Assert.IsNaN(configuration.StormDuration.VariationCoefficient);
            Assert.IsNaN(configuration.WidthFlowApertures.Mean);
            Assert.IsNaN(configuration.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(configuration.WaveReduction.BreakWaterType);
            Assert.IsNaN(configuration.WaveReduction.BreakWaterHeight);
            Assert.IsNull(configuration.WaveReduction.UseBreakWater);
            Assert.IsNull(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsNaN(configuration.Scenario.Contribution);
        }

        [Test]
        public void Read_ValidPartialConfigurations_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validPartialConfiguration.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.StructureId);
            Assert.AreEqual("Locatie1", configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual("profiel1", configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);

            Assert.IsNull(configuration.FailureProbabilityOpenStructure);
            Assert.IsNull(configuration.FailureProbabilityReparation);
            Assert.IsNull(configuration.IdenticalApertures);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.FactorStormDurationOpenStructure);
            Assert.IsNull(configuration.ProbabilityOpenStructureBeforeFlooding);

            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.DrainCoefficient);
            Assert.IsNull(configuration.InsideWaterLevel);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir);
            Assert.IsNull(configuration.LevelCrestStructureNotClosing);

            Assert.IsNull(configuration.AllowedLevelIncreaseStorage);
            Assert.AreEqual(2, configuration.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(0.1, configuration.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection);
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow);
            Assert.AreEqual(15000, configuration.StorageStructureArea.Mean);
            Assert.IsNull(configuration.StorageStructureArea.VariationCoefficient);
            Assert.AreEqual(6.0, configuration.StormDuration.Mean);
            Assert.IsNull(configuration.StormDuration.VariationCoefficient);
            Assert.IsNull(configuration.WidthFlowApertures.Mean);
            Assert.AreEqual(0.1, configuration.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(configuration.WaveReduction.BreakWaterType);
            Assert.IsNull(configuration.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);
            Assert.IsTrue(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsTrue(configuration.Scenario.IsRelevant);
            Assert.AreEqual(8.8, configuration.Scenario.Contribution);
        }

        [Test]
        public void Read_ValidConfigurationsEmptyStochastElements_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyStochastElements.xml");
            var reader = new ClosingStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (ClosingStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);

            Assert.IsNull(configuration.FailureProbabilityOpenStructure);
            Assert.IsNull(configuration.FailureProbabilityReparation);
            Assert.IsNull(configuration.IdenticalApertures);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.FactorStormDurationOpenStructure);
            Assert.IsNull(configuration.ProbabilityOpenStructureBeforeFlooding);

            Assert.IsNull(configuration.AreaFlowApertures.Mean);
            Assert.IsNull(configuration.AreaFlowApertures.StandardDeviation);
            Assert.IsNull(configuration.DrainCoefficient.Mean);
            Assert.IsNull(configuration.DrainCoefficient.StandardDeviation);
            Assert.IsNull(configuration.InsideWaterLevel.Mean);
            Assert.IsNull(configuration.InsideWaterLevel.StandardDeviation);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir.Mean);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNull(configuration.LevelCrestStructureNotClosing.Mean);
            Assert.IsNull(configuration.LevelCrestStructureNotClosing.StandardDeviation);

            Assert.IsNull(configuration.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNull(configuration.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge.Mean);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection.Mean);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow.Mean);
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.IsNull(configuration.StorageStructureArea.Mean);
            Assert.IsNull(configuration.StorageStructureArea.VariationCoefficient);
            Assert.IsNull(configuration.StormDuration.Mean);
            Assert.IsNull(configuration.StormDuration.VariationCoefficient);
            Assert.IsNull(configuration.WidthFlowApertures.Mean);
            Assert.IsNull(configuration.WidthFlowApertures.StandardDeviation);
            Assert.IsNull(configuration.WaveReduction);
            Assert.IsNull(configuration.Scenario);
        }

        private static void AssertConfiguration(ClosingStructuresCalculationConfiguration configuration)
        {
            Assert.AreEqual(67.1, configuration.StructureNormalOrientation);
            Assert.AreEqual("kunstwerk1", configuration.StructureId);
            Assert.AreEqual("Locatie1", configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual("profiel1", configuration.ForeshoreProfileId);
            Assert.AreEqual(0.002, configuration.FactorStormDurationOpenStructure);
            Assert.AreEqual(0.03, configuration.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(0.22, configuration.FailureProbabilityOpenStructure);
            Assert.AreEqual(0.0006, configuration.FailureProbabilityReparation);
            Assert.AreEqual(0.001, configuration.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(4, configuration.IdenticalApertures);
            Assert.AreEqual(ConfigurationClosingStructureInflowModelType.VerticalWall, configuration.InflowModelType);

            Assert.AreEqual(1.1, configuration.DrainCoefficient.Mean);
            Assert.AreEqual(0.1, configuration.DrainCoefficient.StandardDeviation);
            Assert.AreEqual(0.5, configuration.InsideWaterLevel.Mean);
            Assert.AreEqual(0.1, configuration.InsideWaterLevel.StandardDeviation);
            Assert.AreEqual(80.5, configuration.AreaFlowApertures.Mean);
            Assert.AreEqual(1, configuration.AreaFlowApertures.StandardDeviation);
            Assert.AreEqual(1.2, configuration.ThresholdHeightOpenWeir.Mean);
            Assert.AreEqual(0.1, configuration.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.AreEqual(4.3, configuration.LevelCrestStructureNotClosing.Mean);
            Assert.AreEqual(0.2, configuration.LevelCrestStructureNotClosing.StandardDeviation);

            Assert.AreEqual(0.2, configuration.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(0.01, configuration.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.AreEqual(2, configuration.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(0.1, configuration.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.AreEqual(15.2, configuration.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(0.1, configuration.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.AreEqual(1.10, configuration.ModelFactorSuperCriticalFlow.Mean);
            Assert.AreEqual(0.12, configuration.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.AreEqual(15000, configuration.StorageStructureArea.Mean);
            Assert.AreEqual(0.01, configuration.StorageStructureArea.VariationCoefficient);
            Assert.AreEqual(6.0, configuration.StormDuration.Mean);
            Assert.AreEqual(0.12, configuration.StormDuration.VariationCoefficient);
            Assert.AreEqual(15.2, configuration.WidthFlowApertures.Mean);
            Assert.AreEqual(0.1, configuration.WidthFlowApertures.StandardDeviation);

            Assert.AreEqual(ConfigurationBreakWaterType.Dam, configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.234, configuration.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);
            Assert.IsTrue(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsTrue(configuration.Scenario.IsRelevant);
            Assert.AreEqual(8.8, configuration.Scenario.Contribution);
        }
    }
}