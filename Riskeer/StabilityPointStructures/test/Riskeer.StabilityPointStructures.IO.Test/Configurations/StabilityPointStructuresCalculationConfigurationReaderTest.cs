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
using Riskeer.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.StabilityPointStructures.IO,
                                                                               nameof(StabilityPointStructuresCalculationConfigurationReader));

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidMultipleAllowedLevelIncreaseStorageStochast.xml",
                                              "There is a duplicate key sequence 'peilverhogingkomberging' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleAllowedLevelIncreaseStorageStochast");

                yield return new TestCaseData("invalidMultipleAreaFlowAperturesStochast.xml",
                                              "There is a duplicate key sequence 'doorstroomoppervlak' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleAreaFlowAperturesStochast");

                yield return new TestCaseData("invalidMultipleBankWidthStochast.xml",
                                              "There is a duplicate key sequence 'bermbreedte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleBankWidthStochast");

                yield return new TestCaseData("invalidMultipleCriticalOvertoppingDischargeStochast.xml",
                                              "There is a duplicate key sequence 'kritiekinstromenddebiet' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleCriticalOvertoppingDischargeStochast");

                yield return new TestCaseData("invalidMultipleConstructiveStrengthLinearLoadModelStochast.xml",
                                              "There is a duplicate key sequence 'lineairebelastingschematiseringsterkte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleConstructiveStrengthLinearLoadModelStochast");

                yield return new TestCaseData("invalidMultipleConstructiveStrengthQuadraticLoadModelStochast.xml",
                                              "There is a duplicate key sequence 'kwadratischebelastingschematiseringsterkte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleConstructiveStrengthQuadraticLoadModelStochast");

                yield return new TestCaseData("invalidMultipleDrainCoefficientStochast.xml",
                                              "There is a duplicate key sequence 'afvoercoefficient' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleDrainCoefficientStochast");

                yield return new TestCaseData("invalidCalculationEvaluationLevelEmpty.xml",
                                              "The 'analysehoogte' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationEvaluationLevelEmpty");
                yield return new TestCaseData("invalidCalculationEvaluationLevelNoDouble.xml",
                                              "The 'analysehoogte' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationEvaluationLevelNoDouble");
                yield return new TestCaseData("invalidCalculationEvaluationLevelDuplicate.xml",
                                              "Element 'analysehoogte' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationEvaluationLevelDuplicate");

                yield return new TestCaseData("invalidCalculationFactorStormDurationOpenStructureEmpty.xml",
                                              "The 'factorstormduur' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFactorStormDurationOpenStructureEmpty");
                yield return new TestCaseData("invalidCalculationFactorStormDurationOpenStructureNoDouble.xml",
                                              "The 'factorstormduur' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFactorStormDurationOpenStructureNoDouble");
                yield return new TestCaseData("invalidCalculationFactorStormDurationOpenStructureDuplicate.xml",
                                              "Element 'factorstormduur' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationFactorStormDurationOpenStructureDuplicate");

                yield return new TestCaseData("invalidCalculationFailureProbabilityRepairClosureEmpty.xml",
                                              "The 'faalkansherstel' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityRepairClosureEmpty");
                yield return new TestCaseData("invalidCalculationFailureProbabilityRepairClosureNoDouble.xml",
                                              "The 'faalkansherstel' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityRepairClosureNoDouble");
                yield return new TestCaseData("invalidCalculationFailureProbabilityRepairClosureDuplicate.xml",
                                              "Element 'faalkansherstel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationFailureProbabilityRepairClosureDuplicate");

                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionEmpty.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionEmpty");
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionNoDouble.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionNoDouble");
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionDuplicate.xml",
                                              "Element 'faalkansgegevenerosiebodem' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionDuplicate");

                yield return new TestCaseData("invalidMultipleFlowVelocityStructureClosableStochast.xml",
                                              "There is a duplicate key sequence 'kritiekestroomsnelheid' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleFlowVelocityStructureClosableStochast");

                yield return new TestCaseData("invalidMultipleFlowWidthAtBottomProtectionStochast.xml",
                                              "There is a duplicate key sequence 'breedtebodembescherming' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleFlowWidthAtBottomProtectionStochast");

                yield return new TestCaseData("invalidCalculationForeshoreProfileEmpty.xml",
                                              "The 'voorlandprofiel' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationForeshoreProfileEmpty");
                yield return new TestCaseData("invalidCalculationMultipleForeshoreProfile.xml",
                                              "Element 'voorlandprofiel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleForeshoreProfile");

                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationEmpty.xml",
                                              "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationHydraulicBoundaryLocationEmpty");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocation.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocation");

                yield return new TestCaseData("invalidCalculationInflowModelTypeEmpty.xml",
                                              "The 'instroommodel' element is invalid - The value '' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidCalculationInflowModelTypeEmpty");
                yield return new TestCaseData("invalidCalculationInflowModelTypeUnsupportedString.xml",
                                              "The 'instroommodel' element is invalid - The value 'invalid' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidCalculationInflowModelTypeUnsupportedString");
                yield return new TestCaseData("invalidCalculationMultipleInflowModelTypes.xml",
                                              "Element 'instroommodel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleInflowModelTypes");

                yield return new TestCaseData("invalidMultipleInsideWaterLevelStochast.xml",
                                              "There is a duplicate key sequence 'binnenwaterstand' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleInsideWaterLevelStochast");

                yield return new TestCaseData("invalidMultipleInsideWaterLevelFailureConstructionStochast.xml",
                                              "There is a duplicate key sequence 'binnenwaterstandbijfalen' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleInsideWaterLevelFailureConstructionStochast");

                yield return new TestCaseData("invalidMultipleLevelCrestStructureStochast.xml",
                                              "There is a duplicate key sequence 'kerendehoogte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleLevelCrestStructureStochast");

                yield return new TestCaseData("invalidCalculationLevellingCountEmpty.xml",
                                              "The 'nrnivelleringen' element is invalid - The value '' is invalid according to its datatype 'Integer'")
                    .SetName("invalidCalculationLevellingCountEmpty");
                yield return new TestCaseData("invalidCalculationLevellingCountNoInteger.xml",
                                              "The 'nrnivelleringen' element is invalid - The value 'nul' is invalid according to its datatype 'Integer'")
                    .SetName("invalidCalculationLevellingCountNoDouble");
                yield return new TestCaseData("invalidCalculationLevellingCountDuplicate.xml",
                                              "Element 'nrnivelleringen' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationLevellingCountDuplicate");

                yield return new TestCaseData("invalidCalculationLoadSchematizationTypeEmpty.xml",
                                              "The 'belastingschematisering' element is invalid - The value '' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidCalculationLoadSchematizationTypeEmpty");
                yield return new TestCaseData("invalidCalculationLoadSchematizationTypeUnsupportedString.xml",
                                              "The 'belastingschematisering' element is invalid - The value 'invalid' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidCalculationLoadSchematizationTypeUnsupportedString");

                yield return new TestCaseData("invalidCalculationProbabilityCollisionSecondaryStructureEmpty.xml",
                                              "The 'kansaanvaringtweedekeermiddel' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationProbabilityCollisionSecondaryStructureEmpty");
                yield return new TestCaseData("invalidCalculationProbabilityCollisionSecondaryStructureNoDouble.xml",
                                              "The 'kansaanvaringtweedekeermiddel' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationProbabilityCollisionSecondaryStructureNoDouble");
                yield return new TestCaseData("invalidCalculationProbabilityCollisionSecondaryStructureDuplicate.xml",
                                              "Element 'kansaanvaringtweedekeermiddel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationProbabilityCollisionSecondaryStructureDuplicate");

                yield return new TestCaseData("invalidMultipleModelFactorSuperCriticalFlowStochast.xml",
                                              "There is a duplicate key sequence 'modelfactoroverloopdebiet' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleModelFactorSuperCriticalFlowStochast");

                yield return new TestCaseData("invalidMultipleShipMassStochast.xml",
                                              "There is a duplicate key sequence 'massaschip' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleShipMassStochast");

                yield return new TestCaseData("invalidMultipleShipVelocityStochast.xml",
                                              "There is a duplicate key sequence 'aanvaarsnelheid' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleShipVelocityStochast");

                yield return new TestCaseData("invalidMultipleStabilityLinearLoadModelStochast.xml",
                                              "There is a duplicate key sequence 'lineairebelastingschematiseringstabiliteit' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleStabilityLinearLoadModelStochast");

                yield return new TestCaseData("invalidMultipleStabilityQuadraticLoadModelStochast.xml",
                                              "There is a duplicate key sequence 'kwadratischebelastingschematiseringstabiliteit' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleStabilityQuadraticLoadModelStochast");

                yield return new TestCaseData("invalidCalculationStructureEmpty.xml",
                                              "The 'kunstwerk' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationStructureEmpty");
                yield return new TestCaseData("invalidCalculationMultipleStructure.xml",
                                              "Element 'kunstwerk' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStructure");

                yield return new TestCaseData("invalidMultipleStorageStructureAreaStochast.xml",
                                              "There is a duplicate key sequence 'kombergendoppervlak' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleStorageStructureAreaStochast");

                yield return new TestCaseData("invalidMultipleStormDurationStochast.xml",
                                              "There is a duplicate key sequence 'stormduur' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleStormDurationStochast");

                yield return new TestCaseData("invalidCalculationOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationStructureNormalOrientationEmpty");
                yield return new TestCaseData("invalidCalculationOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationStructureNormalOrientationNoDouble");
                yield return new TestCaseData("invalidCalculationMultipleOrientation.xml",
                                              "Element 'orientatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStructureNormalOrientation");

                yield return new TestCaseData("invalidMultipleThresholdHeightOpenWeirStochast.xml",
                                              "There is a duplicate key sequence 'drempelhoogte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleThresholdHeightOpenWeirStochast");

                yield return new TestCaseData("invalidCalculationVerticalDistanceEmpty.xml",
                                              "The 'afstandonderkantwandteendijk' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationVerticalDistanceEmpty");
                yield return new TestCaseData("invalidCalculationVerticalDistanceNoDouble.xml",
                                              "The 'afstandonderkantwandteendijk' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationVerticalDistanceNoDouble");
                yield return new TestCaseData("invalidCalculationVerticalDistanceDuplicate.xml",
                                              "Element 'afstandonderkantwandteendijk' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationVerticalDistanceDuplicate");

                yield return new TestCaseData("invalidCalculationVolumicWeightWaterEmpty.xml",
                                              "The 'volumiekgewichtwater' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationVolumicWeightWaterEmpty");
                yield return new TestCaseData("invalidCalculationVolumicWeightWaterNoDouble.xml",
                                              "The 'volumiekgewichtwater' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationVolumicWeightWaterNoDouble");
                yield return new TestCaseData("invalidCalculationVolumicWeightWaterDuplicate.xml",
                                              "Element 'volumiekgewichtwater' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationVolumicWeightWaterDuplicate");

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

                yield return new TestCaseData("invalidMultipleWidthFlowAperturesStochast.xml",
                                              "There is a duplicate key sequence 'breedtedoorstroomopening' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleWidthFlowAperturesStochast");

                yield return new TestCaseData("invalidStochastNoName.xml",
                                              "The required attribute 'naam' is missing.")
                    .SetName("invalidStochastNoName");

                yield return new TestCaseData("invalidStochastUnknownName.xml",
                                              "The 'naam' attribute is invalid - The value 'Test' is invalid according to its datatype 'nameType' - The Enumeration constraint failed.")
                    .SetName("invalidStochastUnknownName");

                yield return new TestCaseData("invalidStochastMultipleMean.xml",
                                              "Element 'verwachtingswaarde' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidStochastMultipleMean");
                yield return new TestCaseData("invalidStochastMeanNoDouble.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastMeanNoDouble");
                yield return new TestCaseData("invalidStochastMeanEmpty.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastMeanEmpty");

                yield return new TestCaseData("invalidStochastStandardDeviationEmpty.xml",
                                              "The 'standaardafwijking' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationEmpty");
                yield return new TestCaseData("invalidStochastStandardDeviationNoDouble.xml",
                                              "The 'standaardafwijking' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationNoDouble");
                yield return new TestCaseData("invalidStochastMultipleStandardDeviation.xml",
                                              "Element 'standaardafwijking' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidStochastMultipleStandardDeviation");

                yield return new TestCaseData("invalidStochastVariationCoefficientEmpty.xml",
                                              "The 'variatiecoefficient' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientEmpty");
                yield return new TestCaseData("invalidStochastVariationCoefficientNoDouble.xml",
                                              "The 'variatiecoefficient' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientNoDouble");
                yield return new TestCaseData("invalidStochastMultipleVariationCoefficient.xml",
                                              "Element 'variatiecoefficient' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidStochastMultipleVariationCoefficient");
                yield return new TestCaseData("invalidShouldIllustrationPointsBeCalculatedEmpty.xml",
                                              "The 'illustratiepunteninlezen' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldIllustrationPointsBeCalculatedEmpty");
                yield return new TestCaseData("invalidShouldIllustrationPointsBeCalculatedNoBoolean.xml",
                                              "The 'illustratiepunteninlezen' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidShouldIllustrationPointsBeCalculatedNoBoolean");
            }
        }

        [Test]
        public void Constructor_WithFilePath_ReturnsNewInstance()
        {
            // Setup
            string existingPath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new StabilityPointStructuresCalculationConfigurationReader(existingPath);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<StabilityPointStructuresCalculationConfiguration>>(reader);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            void Call() => new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        [Test]
        [TestCase("validFullConfiguration")]
        [TestCase("validFullConfiguration_differentOrder")]
        public void Read_ValidFullConfigurations_ExpectedValues(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            AssertConfiguration(configuration);
        }

        [Test]
        public void Read_ValidPreviousVersionConfigurationWithFullCalculation_ReturnExpectedReadCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "version0ValidConfigurationFullCalculation.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration)readConfigurationItems.Single();

            AssertConfiguration(configuration);
        }

        [Test]
        public void Read_ValidPartialConfigurations_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validPartialConfiguration.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual(0.01, configuration.FactorStormDurationOpenStructure);
            Assert.AreEqual("profiel1", configuration.ForeshoreProfileId);
            Assert.AreEqual("Locatie1", configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(9.81, configuration.VolumicWeightWater);
            Assert.AreEqual("kunstwerk1", configuration.StructureId);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.23, configuration.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);

            Assert.IsNull(configuration.InsideWaterLevel.Mean);
            Assert.AreEqual(0.1, configuration.InsideWaterLevel.StandardDeviation);

            Assert.AreEqual(1.2, configuration.StabilityLinearLoadModel.Mean);
            Assert.AreEqual(0.1, configuration.StabilityLinearLoadModel.VariationCoefficient);

            Assert.IsNull(configuration.StorageStructureArea.Mean);
            Assert.AreEqual(0.01, configuration.StorageStructureArea.VariationCoefficient);

            Assert.AreEqual(6.0, configuration.StormDuration.Mean);
            Assert.IsNull(configuration.StormDuration.VariationCoefficient);

            Assert.IsNull(configuration.AllowedLevelIncreaseStorage);
            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.BankWidth);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(configuration.DrainCoefficient);
            Assert.IsNull(configuration.EvaluationLevel);
            Assert.IsNull(configuration.FailureCollisionEnergy);
            Assert.IsNull(configuration.FailureProbabilityRepairClosure);
            Assert.IsNull(configuration.FlowVelocityStructureClosable);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.InsideWaterLevelFailureConstruction);
            Assert.IsNull(configuration.LevelCrestStructure);
            Assert.IsNull(configuration.LevellingCount);
            Assert.IsNull(configuration.LoadSchematizationType);
            Assert.IsNull(configuration.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(configuration.ShipVelocity);
            Assert.IsNull(configuration.ShipMass);
            Assert.IsNull(configuration.StabilityQuadraticLoadModel);
            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir);
            Assert.IsNull(configuration.VerticalDistance);
            Assert.IsNull(configuration.WidthFlowApertures);
            Assert.IsNull(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(configuration.ShouldIllustrationPointsBeCalculated);
            Assert.IsTrue(configuration.Scenario.IsRelevant);
            Assert.AreEqual(8.8, configuration.Scenario.Contribution);
        }

        [Test]
        [TestCase("validConfigurationEmptyStochasts", TestName = "Read_Empty({0:80})")]
        [TestCase("validConfigurationEmptyCalculation", TestName = "Read_Empty({0:80})")]
        public void Read_ValidConfigurationWithEmptyCalculationOrWithEmptyStochasts_NoValuesSet(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(configuration.AllowedLevelIncreaseStorage);
            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.BankWidth);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(configuration.DrainCoefficient);
            Assert.IsNull(configuration.FailureCollisionEnergy);
            Assert.IsNull(configuration.FlowVelocityStructureClosable);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection);
            Assert.IsNull(configuration.InsideWaterLevel);
            Assert.IsNull(configuration.InsideWaterLevelFailureConstruction);
            Assert.IsNull(configuration.LevelCrestStructure);
            Assert.IsNull(configuration.ShipVelocity);
            Assert.IsNull(configuration.ShipMass);
            Assert.IsNull(configuration.StabilityLinearLoadModel);
            Assert.IsNull(configuration.StabilityQuadraticLoadModel);
            Assert.IsNull(configuration.StorageStructureArea);
            Assert.IsNull(configuration.StormDuration);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir);
            Assert.IsNull(configuration.WidthFlowApertures);

            Assert.IsNull(configuration.EvaluationLevel);
            Assert.IsNull(configuration.FactorStormDurationOpenStructure);
            Assert.IsNull(configuration.FailureProbabilityRepairClosure);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.LevellingCount);
            Assert.IsNull(configuration.LoadSchematizationType);
            Assert.IsNull(configuration.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(configuration.VolumicWeightWater);
            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.VerticalDistance);
            Assert.IsNull(configuration.WaveReduction);
            Assert.IsNull(configuration.Scenario);
        }

        [Test]
        public void Read_ValidConfigurationsEmptyStochastElements_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyStochastElements.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(configuration.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNull(configuration.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNull(configuration.AreaFlowApertures.Mean);
            Assert.IsNull(configuration.AreaFlowApertures.StandardDeviation);
            Assert.IsNull(configuration.BankWidth.Mean);
            Assert.IsNull(configuration.BankWidth.StandardDeviation);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge.Mean);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel.VariationCoefficient);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient);
            Assert.IsNull(configuration.DrainCoefficient.Mean);
            Assert.IsNull(configuration.DrainCoefficient.StandardDeviation);
            Assert.IsNull(configuration.FailureCollisionEnergy.Mean);
            Assert.IsNull(configuration.FailureCollisionEnergy.VariationCoefficient);
            Assert.IsNull(configuration.FlowVelocityStructureClosable.Mean);
            Assert.IsNull(configuration.FlowVelocityStructureClosable.VariationCoefficient);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection.Mean);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNull(configuration.InsideWaterLevel.Mean);
            Assert.IsNull(configuration.InsideWaterLevel.StandardDeviation);
            Assert.IsNull(configuration.InsideWaterLevelFailureConstruction.Mean);
            Assert.IsNull(configuration.InsideWaterLevelFailureConstruction.StandardDeviation);
            Assert.IsNull(configuration.LevelCrestStructure.Mean);
            Assert.IsNull(configuration.LevelCrestStructure.StandardDeviation);
            Assert.IsNull(configuration.ShipVelocity.Mean);
            Assert.IsNull(configuration.ShipVelocity.VariationCoefficient);
            Assert.IsNull(configuration.ShipMass.Mean);
            Assert.IsNull(configuration.ShipMass.VariationCoefficient);
            Assert.IsNull(configuration.StabilityLinearLoadModel.Mean);
            Assert.IsNull(configuration.StabilityLinearLoadModel.VariationCoefficient);
            Assert.IsNull(configuration.StabilityQuadraticLoadModel.Mean);
            Assert.IsNull(configuration.StabilityQuadraticLoadModel.VariationCoefficient);
            Assert.IsNull(configuration.StorageStructureArea.Mean);
            Assert.IsNull(configuration.StorageStructureArea.VariationCoefficient);
            Assert.IsNull(configuration.StormDuration.Mean);
            Assert.IsNull(configuration.StormDuration.VariationCoefficient);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir.Mean);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNull(configuration.WidthFlowApertures.Mean);
            Assert.IsNull(configuration.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(configuration.EvaluationLevel);
            Assert.IsNull(configuration.FactorStormDurationOpenStructure);
            Assert.IsNull(configuration.FailureProbabilityRepairClosure);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.LevellingCount);
            Assert.IsNull(configuration.LoadSchematizationType);
            Assert.IsNull(configuration.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(configuration.VolumicWeightWater);
            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.VerticalDistance);
            Assert.IsNull(configuration.WaveReduction);
            Assert.IsNull(configuration.Scenario);
        }

        [Test]
        public void Read_ValidFullConfigurationsContainingInfinity_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validFullConfigurationContainingInfinity.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.That(double.IsPositiveInfinity(configuration.AllowedLevelIncreaseStorage.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.AllowedLevelIncreaseStorage.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.AreaFlowApertures.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.AreaFlowApertures.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.BankWidth.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.BankWidth.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.CriticalOvertoppingDischarge.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.CriticalOvertoppingDischarge.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.ConstructiveStrengthLinearLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.ConstructiveStrengthLinearLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.ConstructiveStrengthQuadraticLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.DrainCoefficient.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.DrainCoefficient.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(configuration.EvaluationLevel.Value));

            Assert.That(double.IsNegativeInfinity(configuration.FactorStormDurationOpenStructure.Value));

            Assert.That(double.IsPositiveInfinity(configuration.FailureCollisionEnergy.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.FailureCollisionEnergy.VariationCoefficient.Value));

            Assert.That(double.IsNegativeInfinity(configuration.FailureProbabilityRepairClosure.Value));

            Assert.That(double.IsPositiveInfinity(configuration.FlowVelocityStructureClosable.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.FlowVelocityStructureClosable.VariationCoefficient.Value));

            Assert.That(double.IsNegativeInfinity(configuration.FailureProbabilityStructureWithErosion.Value));

            Assert.That(double.IsPositiveInfinity(configuration.FlowWidthAtBottomProtection.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.FlowWidthAtBottomProtection.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.InsideWaterLevel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.InsideWaterLevel.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.InsideWaterLevelFailureConstruction.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.InsideWaterLevelFailureConstruction.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.LevelCrestStructure.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.LevelCrestStructure.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(configuration.ProbabilityCollisionSecondaryStructure.Value));

            Assert.That(double.IsPositiveInfinity(configuration.ShipVelocity.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.ShipVelocity.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.ShipMass.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.ShipMass.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.StabilityLinearLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.StabilityLinearLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.StabilityQuadraticLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.StabilityQuadraticLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.StorageStructureArea.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.StorageStructureArea.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(configuration.StormDuration.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.StormDuration.VariationCoefficient.Value));

            Assert.That(double.IsNegativeInfinity(configuration.VolumicWeightWater.Value));

            Assert.That(double.IsNegativeInfinity(configuration.StructureNormalOrientation.Value));

            Assert.That(double.IsPositiveInfinity(configuration.ThresholdHeightOpenWeir.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.ThresholdHeightOpenWeir.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(configuration.VerticalDistance.Value));

            Assert.That(double.IsPositiveInfinity(configuration.WidthFlowApertures.Mean.Value));
            Assert.That(double.IsNegativeInfinity(configuration.WidthFlowApertures.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(configuration.WaveReduction.BreakWaterHeight.Value));

            Assert.That(double.IsPositiveInfinity(configuration.Scenario.Contribution.Value));
        }

        [Test]
        public void Read_ValidFullConfigurationsContainingNaN_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validFullConfigurationContainingNaN.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNaN(configuration.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(configuration.AllowedLevelIncreaseStorage.StandardDeviation);

            Assert.IsNaN(configuration.AreaFlowApertures.Mean);
            Assert.IsNaN(configuration.AreaFlowApertures.StandardDeviation);

            Assert.IsNaN(configuration.BankWidth.Mean);
            Assert.IsNaN(configuration.BankWidth.StandardDeviation);

            Assert.IsNaN(configuration.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(configuration.CriticalOvertoppingDischarge.VariationCoefficient);

            Assert.IsNaN(configuration.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.IsNaN(configuration.ConstructiveStrengthLinearLoadModel.VariationCoefficient);

            Assert.IsNaN(configuration.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.IsNaN(configuration.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient);

            Assert.IsNaN(configuration.DrainCoefficient.Mean);
            Assert.IsNaN(configuration.DrainCoefficient.StandardDeviation);

            Assert.IsNaN(configuration.EvaluationLevel);

            Assert.IsNaN(configuration.FactorStormDurationOpenStructure);

            Assert.IsNaN(configuration.FailureCollisionEnergy.Mean);
            Assert.IsNaN(configuration.FailureCollisionEnergy.VariationCoefficient);

            Assert.IsNaN(configuration.FailureProbabilityRepairClosure);

            Assert.IsNaN(configuration.FlowVelocityStructureClosable.Mean);
            Assert.IsNaN(configuration.FlowVelocityStructureClosable.VariationCoefficient);

            Assert.IsNaN(configuration.FailureProbabilityStructureWithErosion);

            Assert.IsNaN(configuration.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(configuration.FlowWidthAtBottomProtection.StandardDeviation);

            Assert.IsNaN(configuration.InsideWaterLevel.Mean);
            Assert.IsNaN(configuration.InsideWaterLevel.StandardDeviation);

            Assert.IsNaN(configuration.InsideWaterLevelFailureConstruction.Mean);
            Assert.IsNaN(configuration.InsideWaterLevelFailureConstruction.StandardDeviation);

            Assert.IsNaN(configuration.LevelCrestStructure.Mean);
            Assert.IsNaN(configuration.LevelCrestStructure.StandardDeviation);

            Assert.IsNaN(configuration.ProbabilityCollisionSecondaryStructure);

            Assert.IsNaN(configuration.ShipVelocity.Mean);
            Assert.IsNaN(configuration.ShipVelocity.VariationCoefficient);

            Assert.IsNaN(configuration.ShipMass.Mean);
            Assert.IsNaN(configuration.ShipMass.VariationCoefficient);

            Assert.IsNaN(configuration.StabilityLinearLoadModel.Mean);
            Assert.IsNaN(configuration.StabilityLinearLoadModel.VariationCoefficient);

            Assert.IsNaN(configuration.StabilityQuadraticLoadModel.Mean);
            Assert.IsNaN(configuration.StabilityQuadraticLoadModel.VariationCoefficient);

            Assert.IsNaN(configuration.StorageStructureArea.Mean);
            Assert.IsNaN(configuration.StorageStructureArea.VariationCoefficient);

            Assert.IsNaN(configuration.StormDuration.Mean);
            Assert.IsNaN(configuration.StormDuration.VariationCoefficient);

            Assert.IsNaN(configuration.VolumicWeightWater);

            Assert.IsNaN(configuration.StructureNormalOrientation);

            Assert.IsNaN(configuration.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(configuration.ThresholdHeightOpenWeir.StandardDeviation);

            Assert.IsNaN(configuration.VerticalDistance);

            Assert.IsNaN(configuration.WidthFlowApertures.Mean);
            Assert.IsNaN(configuration.WidthFlowApertures.StandardDeviation);

            Assert.IsNaN(configuration.WaveReduction.BreakWaterHeight);

            Assert.IsNaN(configuration.Scenario.Contribution);
        }

        private static void AssertConfiguration(StabilityPointStructuresCalculationConfiguration configuration)
        {
            Assert.AreEqual(0.2, configuration.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(0.01, configuration.AllowedLevelIncreaseStorage.StandardDeviation);

            Assert.AreEqual(80.5, configuration.AreaFlowApertures.Mean);
            Assert.AreEqual(1, configuration.AreaFlowApertures.StandardDeviation);

            Assert.AreEqual(1.2, configuration.BankWidth.Mean);
            Assert.AreEqual(0.1, configuration.BankWidth.StandardDeviation);

            Assert.AreEqual(2, configuration.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(0.1, configuration.CriticalOvertoppingDischarge.VariationCoefficient);

            Assert.AreEqual(2, configuration.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.AreEqual(0.1, configuration.ConstructiveStrengthLinearLoadModel.VariationCoefficient);

            Assert.AreEqual(2, configuration.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.AreEqual(0.1, configuration.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient);

            Assert.AreEqual(0.1, configuration.DrainCoefficient.Mean);
            Assert.AreEqual(0.12, configuration.DrainCoefficient.StandardDeviation);

            Assert.AreEqual(0.1, configuration.EvaluationLevel);

            Assert.AreEqual(0.01, configuration.FactorStormDurationOpenStructure);

            Assert.AreEqual(1.2, configuration.FailureCollisionEnergy.Mean);
            Assert.AreEqual(0.1, configuration.FailureCollisionEnergy.VariationCoefficient);

            Assert.AreEqual(0.001, configuration.FailureProbabilityRepairClosure);

            Assert.AreEqual(1.1, configuration.FlowVelocityStructureClosable.Mean);
            Assert.AreEqual(0.12, configuration.FlowVelocityStructureClosable.VariationCoefficient);

            Assert.AreEqual(0.0001, configuration.FailureProbabilityStructureWithErosion);

            Assert.AreEqual("profiel1", configuration.ForeshoreProfileId);

            Assert.AreEqual(15.2, configuration.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(0.1, configuration.FlowWidthAtBottomProtection.StandardDeviation);

            Assert.AreEqual("Locatie1", configuration.HydraulicBoundaryLocationName);

            Assert.AreEqual(ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert, configuration.InflowModelType);

            Assert.AreEqual(0.5, configuration.InsideWaterLevel.Mean);
            Assert.AreEqual(0.1, configuration.InsideWaterLevel.StandardDeviation);

            Assert.AreEqual(0.7, configuration.InsideWaterLevelFailureConstruction.Mean);
            Assert.AreEqual(0.1, configuration.InsideWaterLevelFailureConstruction.StandardDeviation);

            Assert.AreEqual(4.3, configuration.LevelCrestStructure.Mean);
            Assert.AreEqual(0.1, configuration.LevelCrestStructure.StandardDeviation);

            Assert.AreEqual(1, configuration.LevellingCount);

            Assert.AreEqual(ConfigurationStabilityPointStructuresLoadSchematizationType.Linear, configuration.LoadSchematizationType);

            Assert.AreEqual(1e-5, configuration.ProbabilityCollisionSecondaryStructure);

            Assert.AreEqual(1.2, configuration.ShipVelocity.Mean);
            Assert.AreEqual(0.1, configuration.ShipVelocity.VariationCoefficient);

            Assert.AreEqual(16000, configuration.ShipMass.Mean);
            Assert.AreEqual(0.1, configuration.ShipMass.VariationCoefficient);

            Assert.AreEqual(1.2, configuration.StabilityLinearLoadModel.Mean);
            Assert.AreEqual(0.1, configuration.StabilityLinearLoadModel.VariationCoefficient);

            Assert.AreEqual(1.2, configuration.StabilityQuadraticLoadModel.Mean);
            Assert.AreEqual(0.1, configuration.StabilityQuadraticLoadModel.VariationCoefficient);

            Assert.AreEqual(15000, configuration.StorageStructureArea.Mean);
            Assert.AreEqual(0.01, configuration.StorageStructureArea.VariationCoefficient);

            Assert.AreEqual(6.0, configuration.StormDuration.Mean);
            Assert.AreEqual(0.12, configuration.StormDuration.VariationCoefficient);

            Assert.AreEqual(9.81, configuration.VolumicWeightWater);

            Assert.AreEqual(7, configuration.StructureNormalOrientation);

            Assert.AreEqual("kunstwerk1", configuration.StructureId);

            Assert.AreEqual(1.2, configuration.ThresholdHeightOpenWeir.Mean);
            Assert.AreEqual(0.1, configuration.ThresholdHeightOpenWeir.StandardDeviation);

            Assert.AreEqual(2, configuration.VerticalDistance);

            Assert.AreEqual(15.2, configuration.WidthFlowApertures.Mean);
            Assert.AreEqual(0.1, configuration.WidthFlowApertures.StandardDeviation);

            Assert.AreEqual(ConfigurationBreakWaterType.Dam, configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.23, configuration.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);
            Assert.IsFalse(configuration.WaveReduction.UseForeshoreProfile);

            Assert.IsTrue(configuration.Scenario.IsRelevant);
            Assert.AreEqual(8.8, configuration.Scenario.Contribution);
        }
    }
}