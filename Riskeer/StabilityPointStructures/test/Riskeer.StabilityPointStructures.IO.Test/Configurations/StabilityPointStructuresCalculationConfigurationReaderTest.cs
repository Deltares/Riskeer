// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityPointStructures.IO,
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

                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationEmptyOld.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationHydraulicBoundaryLocationEmptyOld");
                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationEmptyNew.xml",
                                              "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationHydraulicBoundaryLocationEmptyNew");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocationOld.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocationNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocationNew");
                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationOldAndNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationHydraulicBoundaryLocationOldAndNew");

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
            TestDelegate call = () => new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        [Test]
        [TestCase("validFullConfigurationOld")]
        [TestCase("validFullConfigurationNew")]
        [TestCase("validFullConfiguration_differentOrder_old")]
        [TestCase("validFullConfiguration_differentOrder_new")]
        public void Read_ValidFullConfigurations_ExpectedValues(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual(0.2, calculation.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(0.01, calculation.AllowedLevelIncreaseStorage.StandardDeviation);

            Assert.AreEqual(80.5, calculation.AreaFlowApertures.Mean);
            Assert.AreEqual(1, calculation.AreaFlowApertures.StandardDeviation);

            Assert.AreEqual(1.2, calculation.BankWidth.Mean);
            Assert.AreEqual(0.1, calculation.BankWidth.StandardDeviation);

            Assert.AreEqual(2, calculation.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(0.1, calculation.CriticalOvertoppingDischarge.VariationCoefficient);

            Assert.AreEqual(2, calculation.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.AreEqual(0.1, calculation.ConstructiveStrengthLinearLoadModel.VariationCoefficient);

            Assert.AreEqual(2, calculation.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.AreEqual(0.1, calculation.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient);

            Assert.AreEqual(0.1, calculation.DrainCoefficient.Mean);
            Assert.AreEqual(0.12, calculation.DrainCoefficient.StandardDeviation);

            Assert.AreEqual(0.1, calculation.EvaluationLevel);

            Assert.AreEqual(0.01, calculation.FactorStormDurationOpenStructure);

            Assert.AreEqual(1.2, calculation.FailureCollisionEnergy.Mean);
            Assert.AreEqual(0.1, calculation.FailureCollisionEnergy.VariationCoefficient);

            Assert.AreEqual(0.001, calculation.FailureProbabilityRepairClosure);

            Assert.AreEqual(1.1, calculation.FlowVelocityStructureClosable.Mean);
            Assert.AreEqual(0.12, calculation.FlowVelocityStructureClosable.VariationCoefficient);

            Assert.AreEqual(0.0001, calculation.FailureProbabilityStructureWithErosion);

            Assert.AreEqual("profiel1", calculation.ForeshoreProfileId);

            Assert.AreEqual(15.2, calculation.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(0.1, calculation.FlowWidthAtBottomProtection.StandardDeviation);

            Assert.AreEqual("Locatie1", calculation.HydraulicBoundaryLocationName);

            Assert.AreEqual(ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert, calculation.InflowModelType);

            Assert.AreEqual(0.5, calculation.InsideWaterLevel.Mean);
            Assert.AreEqual(0.1, calculation.InsideWaterLevel.StandardDeviation);

            Assert.AreEqual(0.7, calculation.InsideWaterLevelFailureConstruction.Mean);
            Assert.AreEqual(0.1, calculation.InsideWaterLevelFailureConstruction.StandardDeviation);

            Assert.AreEqual(4.3, calculation.LevelCrestStructure.Mean);
            Assert.AreEqual(0.1, calculation.LevelCrestStructure.StandardDeviation);

            Assert.AreEqual(1, calculation.LevellingCount);

            Assert.AreEqual(ConfigurationStabilityPointStructuresLoadSchematizationType.Linear, calculation.LoadSchematizationType);

            Assert.AreEqual(1e-5, calculation.ProbabilityCollisionSecondaryStructure);

            Assert.AreEqual(1.2, calculation.ShipVelocity.Mean);
            Assert.AreEqual(0.1, calculation.ShipVelocity.VariationCoefficient);

            Assert.AreEqual(16000, calculation.ShipMass.Mean);
            Assert.AreEqual(0.1, calculation.ShipMass.VariationCoefficient);

            Assert.AreEqual(1.2, calculation.StabilityLinearLoadModel.Mean);
            Assert.AreEqual(0.1, calculation.StabilityLinearLoadModel.VariationCoefficient);

            Assert.AreEqual(1.2, calculation.StabilityQuadraticLoadModel.Mean);
            Assert.AreEqual(0.1, calculation.StabilityQuadraticLoadModel.VariationCoefficient);

            Assert.AreEqual(15000, calculation.StorageStructureArea.Mean);
            Assert.AreEqual(0.01, calculation.StorageStructureArea.VariationCoefficient);

            Assert.AreEqual(6.0, calculation.StormDuration.Mean);
            Assert.AreEqual(0.12, calculation.StormDuration.VariationCoefficient);

            Assert.AreEqual(9.81, calculation.VolumicWeightWater);

            Assert.AreEqual(7, calculation.StructureNormalOrientation);

            Assert.AreEqual("kunstwerk1", calculation.StructureId);

            Assert.AreEqual(1.2, calculation.ThresholdHeightOpenWeir.Mean);
            Assert.AreEqual(0.1, calculation.ThresholdHeightOpenWeir.StandardDeviation);

            Assert.AreEqual(2, calculation.VerticalDistance);

            Assert.AreEqual(15.2, calculation.WidthFlowApertures.Mean);
            Assert.AreEqual(0.1, calculation.WidthFlowApertures.StandardDeviation);

            Assert.AreEqual(ConfigurationBreakWaterType.Dam, calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.23, calculation.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(calculation.WaveReduction.UseBreakWater);
            Assert.IsFalse(calculation.WaveReduction.UseForeshoreProfile);
        }

        [Test]
        [TestCase("validPartialConfigurationOld")]
        [TestCase("validPartialConfigurationNew")]
        public void Read_ValidPartialConfigurations_ExpectedValues(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual(0.01, calculation.FactorStormDurationOpenStructure);
            Assert.AreEqual("profiel1", calculation.ForeshoreProfileId);
            Assert.AreEqual("Locatie1", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(9.81, calculation.VolumicWeightWater);
            Assert.AreEqual("kunstwerk1", calculation.StructureId);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.23, calculation.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(calculation.WaveReduction.UseBreakWater);

            Assert.IsNull(calculation.InsideWaterLevel.Mean);
            Assert.AreEqual(0.1, calculation.InsideWaterLevel.StandardDeviation);

            Assert.AreEqual(1.2, calculation.StabilityLinearLoadModel.Mean);
            Assert.AreEqual(0.1, calculation.StabilityLinearLoadModel.VariationCoefficient);

            Assert.IsNull(calculation.StorageStructureArea.Mean);
            Assert.AreEqual(0.01, calculation.StorageStructureArea.VariationCoefficient);

            Assert.AreEqual(6.0, calculation.StormDuration.Mean);
            Assert.IsNull(calculation.StormDuration.VariationCoefficient);

            Assert.IsNull(calculation.AllowedLevelIncreaseStorage);
            Assert.IsNull(calculation.AreaFlowApertures);
            Assert.IsNull(calculation.BankWidth);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge);
            Assert.IsNull(calculation.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(calculation.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(calculation.DrainCoefficient);
            Assert.IsNull(calculation.EvaluationLevel);
            Assert.IsNull(calculation.FailureCollisionEnergy);
            Assert.IsNull(calculation.FailureProbabilityRepairClosure);
            Assert.IsNull(calculation.FlowVelocityStructureClosable);
            Assert.IsNull(calculation.FailureProbabilityStructureWithErosion);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection);
            Assert.IsNull(calculation.InflowModelType);
            Assert.IsNull(calculation.InsideWaterLevelFailureConstruction);
            Assert.IsNull(calculation.LevelCrestStructure);
            Assert.IsNull(calculation.LevellingCount);
            Assert.IsNull(calculation.LoadSchematizationType);
            Assert.IsNull(calculation.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(calculation.ShipVelocity);
            Assert.IsNull(calculation.ShipMass);
            Assert.IsNull(calculation.StabilityQuadraticLoadModel);
            Assert.IsNull(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.ThresholdHeightOpenWeir);
            Assert.IsNull(calculation.VerticalDistance);
            Assert.IsNull(calculation.WidthFlowApertures);
            Assert.IsNull(calculation.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(calculation.ShouldIllustrationPointsBeCalculated);
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
            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.AllowedLevelIncreaseStorage);
            Assert.IsNull(calculation.AreaFlowApertures);
            Assert.IsNull(calculation.BankWidth);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge);
            Assert.IsNull(calculation.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(calculation.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(calculation.DrainCoefficient);
            Assert.IsNull(calculation.FailureCollisionEnergy);
            Assert.IsNull(calculation.FlowVelocityStructureClosable);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection);
            Assert.IsNull(calculation.InsideWaterLevel);
            Assert.IsNull(calculation.InsideWaterLevelFailureConstruction);
            Assert.IsNull(calculation.LevelCrestStructure);
            Assert.IsNull(calculation.ShipVelocity);
            Assert.IsNull(calculation.ShipMass);
            Assert.IsNull(calculation.StabilityLinearLoadModel);
            Assert.IsNull(calculation.StabilityQuadraticLoadModel);
            Assert.IsNull(calculation.StorageStructureArea);
            Assert.IsNull(calculation.StormDuration);
            Assert.IsNull(calculation.ThresholdHeightOpenWeir);
            Assert.IsNull(calculation.WidthFlowApertures);

            Assert.IsNull(calculation.EvaluationLevel);
            Assert.IsNull(calculation.FactorStormDurationOpenStructure);
            Assert.IsNull(calculation.FailureProbabilityRepairClosure);
            Assert.IsNull(calculation.FailureProbabilityStructureWithErosion);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.InflowModelType);
            Assert.IsNull(calculation.LevellingCount);
            Assert.IsNull(calculation.LoadSchematizationType);
            Assert.IsNull(calculation.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(calculation.VolumicWeightWater);
            Assert.IsNull(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.StructureId);
            Assert.IsNull(calculation.VerticalDistance);

            Assert.IsNull(calculation.WaveReduction);
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
            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNull(calculation.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNull(calculation.AreaFlowApertures.Mean);
            Assert.IsNull(calculation.AreaFlowApertures.StandardDeviation);
            Assert.IsNull(calculation.BankWidth.Mean);
            Assert.IsNull(calculation.BankWidth.StandardDeviation);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge.Mean);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNull(calculation.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.IsNull(calculation.ConstructiveStrengthLinearLoadModel.VariationCoefficient);
            Assert.IsNull(calculation.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.IsNull(calculation.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient);
            Assert.IsNull(calculation.DrainCoefficient.Mean);
            Assert.IsNull(calculation.DrainCoefficient.StandardDeviation);
            Assert.IsNull(calculation.FailureCollisionEnergy.Mean);
            Assert.IsNull(calculation.FailureCollisionEnergy.VariationCoefficient);
            Assert.IsNull(calculation.FlowVelocityStructureClosable.Mean);
            Assert.IsNull(calculation.FlowVelocityStructureClosable.VariationCoefficient);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection.Mean);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNull(calculation.InsideWaterLevel.Mean);
            Assert.IsNull(calculation.InsideWaterLevel.StandardDeviation);
            Assert.IsNull(calculation.InsideWaterLevelFailureConstruction.Mean);
            Assert.IsNull(calculation.InsideWaterLevelFailureConstruction.StandardDeviation);
            Assert.IsNull(calculation.LevelCrestStructure.Mean);
            Assert.IsNull(calculation.LevelCrestStructure.StandardDeviation);
            Assert.IsNull(calculation.ShipVelocity.Mean);
            Assert.IsNull(calculation.ShipVelocity.VariationCoefficient);
            Assert.IsNull(calculation.ShipMass.Mean);
            Assert.IsNull(calculation.ShipMass.VariationCoefficient);
            Assert.IsNull(calculation.StabilityLinearLoadModel.Mean);
            Assert.IsNull(calculation.StabilityLinearLoadModel.VariationCoefficient);
            Assert.IsNull(calculation.StabilityQuadraticLoadModel.Mean);
            Assert.IsNull(calculation.StabilityQuadraticLoadModel.VariationCoefficient);
            Assert.IsNull(calculation.StorageStructureArea.Mean);
            Assert.IsNull(calculation.StorageStructureArea.VariationCoefficient);
            Assert.IsNull(calculation.StormDuration.Mean);
            Assert.IsNull(calculation.StormDuration.VariationCoefficient);
            Assert.IsNull(calculation.ThresholdHeightOpenWeir.Mean);
            Assert.IsNull(calculation.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNull(calculation.WidthFlowApertures.Mean);
            Assert.IsNull(calculation.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(calculation.EvaluationLevel);
            Assert.IsNull(calculation.FactorStormDurationOpenStructure);
            Assert.IsNull(calculation.FailureProbabilityRepairClosure);
            Assert.IsNull(calculation.FailureProbabilityStructureWithErosion);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.InflowModelType);
            Assert.IsNull(calculation.LevellingCount);
            Assert.IsNull(calculation.LoadSchematizationType);
            Assert.IsNull(calculation.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(calculation.VolumicWeightWater);
            Assert.IsNull(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.StructureId);
            Assert.IsNull(calculation.VerticalDistance);

            Assert.IsNull(calculation.WaveReduction);
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
            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.That(double.IsPositiveInfinity(calculation.AllowedLevelIncreaseStorage.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.AllowedLevelIncreaseStorage.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.AreaFlowApertures.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.AreaFlowApertures.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.BankWidth.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.BankWidth.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.CriticalOvertoppingDischarge.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.CriticalOvertoppingDischarge.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.ConstructiveStrengthLinearLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.ConstructiveStrengthLinearLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.ConstructiveStrengthQuadraticLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.DrainCoefficient.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.DrainCoefficient.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(calculation.EvaluationLevel.Value));

            Assert.That(double.IsNegativeInfinity(calculation.FactorStormDurationOpenStructure.Value));

            Assert.That(double.IsPositiveInfinity(calculation.FailureCollisionEnergy.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.FailureCollisionEnergy.VariationCoefficient.Value));

            Assert.That(double.IsNegativeInfinity(calculation.FailureProbabilityRepairClosure.Value));

            Assert.That(double.IsPositiveInfinity(calculation.FlowVelocityStructureClosable.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.FlowVelocityStructureClosable.VariationCoefficient.Value));

            Assert.That(double.IsNegativeInfinity(calculation.FailureProbabilityStructureWithErosion.Value));

            Assert.That(double.IsPositiveInfinity(calculation.FlowWidthAtBottomProtection.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.FlowWidthAtBottomProtection.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.InsideWaterLevel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.InsideWaterLevel.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.InsideWaterLevelFailureConstruction.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.InsideWaterLevelFailureConstruction.StandardDeviation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.LevelCrestStructure.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.LevelCrestStructure.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(calculation.ProbabilityCollisionSecondaryStructure.Value));

            Assert.That(double.IsPositiveInfinity(calculation.ShipVelocity.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.ShipVelocity.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.ShipMass.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.ShipMass.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.StabilityLinearLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.StabilityLinearLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.StabilityQuadraticLoadModel.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.StabilityQuadraticLoadModel.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.StorageStructureArea.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.StorageStructureArea.VariationCoefficient.Value));

            Assert.That(double.IsPositiveInfinity(calculation.StormDuration.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.StormDuration.VariationCoefficient.Value));

            Assert.That(double.IsNegativeInfinity(calculation.VolumicWeightWater.Value));

            Assert.That(double.IsNegativeInfinity(calculation.StructureNormalOrientation.Value));

            Assert.That(double.IsPositiveInfinity(calculation.ThresholdHeightOpenWeir.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.ThresholdHeightOpenWeir.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(calculation.VerticalDistance.Value));

            Assert.That(double.IsPositiveInfinity(calculation.WidthFlowApertures.Mean.Value));
            Assert.That(double.IsNegativeInfinity(calculation.WidthFlowApertures.StandardDeviation.Value));

            Assert.That(double.IsNegativeInfinity(calculation.WaveReduction.BreakWaterHeight.Value));
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
            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNaN(calculation.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(calculation.AllowedLevelIncreaseStorage.StandardDeviation);

            Assert.IsNaN(calculation.AreaFlowApertures.Mean);
            Assert.IsNaN(calculation.AreaFlowApertures.StandardDeviation);

            Assert.IsNaN(calculation.BankWidth.Mean);
            Assert.IsNaN(calculation.BankWidth.StandardDeviation);

            Assert.IsNaN(calculation.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(calculation.CriticalOvertoppingDischarge.VariationCoefficient);

            Assert.IsNaN(calculation.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.IsNaN(calculation.ConstructiveStrengthLinearLoadModel.VariationCoefficient);

            Assert.IsNaN(calculation.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.IsNaN(calculation.ConstructiveStrengthQuadraticLoadModel.VariationCoefficient);

            Assert.IsNaN(calculation.DrainCoefficient.Mean);
            Assert.IsNaN(calculation.DrainCoefficient.StandardDeviation);

            Assert.IsNaN(calculation.EvaluationLevel);

            Assert.IsNaN(calculation.FactorStormDurationOpenStructure);

            Assert.IsNaN(calculation.FailureCollisionEnergy.Mean);
            Assert.IsNaN(calculation.FailureCollisionEnergy.VariationCoefficient);

            Assert.IsNaN(calculation.FailureProbabilityRepairClosure);

            Assert.IsNaN(calculation.FlowVelocityStructureClosable.Mean);
            Assert.IsNaN(calculation.FlowVelocityStructureClosable.VariationCoefficient);

            Assert.IsNaN(calculation.FailureProbabilityStructureWithErosion);

            Assert.IsNaN(calculation.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(calculation.FlowWidthAtBottomProtection.StandardDeviation);

            Assert.IsNaN(calculation.InsideWaterLevel.Mean);
            Assert.IsNaN(calculation.InsideWaterLevel.StandardDeviation);

            Assert.IsNaN(calculation.InsideWaterLevelFailureConstruction.Mean);
            Assert.IsNaN(calculation.InsideWaterLevelFailureConstruction.StandardDeviation);

            Assert.IsNaN(calculation.LevelCrestStructure.Mean);
            Assert.IsNaN(calculation.LevelCrestStructure.StandardDeviation);

            Assert.IsNaN(calculation.ProbabilityCollisionSecondaryStructure);

            Assert.IsNaN(calculation.ShipVelocity.Mean);
            Assert.IsNaN(calculation.ShipVelocity.VariationCoefficient);

            Assert.IsNaN(calculation.ShipMass.Mean);
            Assert.IsNaN(calculation.ShipMass.VariationCoefficient);

            Assert.IsNaN(calculation.StabilityLinearLoadModel.Mean);
            Assert.IsNaN(calculation.StabilityLinearLoadModel.VariationCoefficient);

            Assert.IsNaN(calculation.StabilityQuadraticLoadModel.Mean);
            Assert.IsNaN(calculation.StabilityQuadraticLoadModel.VariationCoefficient);

            Assert.IsNaN(calculation.StorageStructureArea.Mean);
            Assert.IsNaN(calculation.StorageStructureArea.VariationCoefficient);

            Assert.IsNaN(calculation.StormDuration.Mean);
            Assert.IsNaN(calculation.StormDuration.VariationCoefficient);

            Assert.IsNaN(calculation.VolumicWeightWater);

            Assert.IsNaN(calculation.StructureNormalOrientation);

            Assert.IsNaN(calculation.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(calculation.ThresholdHeightOpenWeir.StandardDeviation);

            Assert.IsNaN(calculation.VerticalDistance);

            Assert.IsNaN(calculation.WidthFlowApertures.Mean);
            Assert.IsNaN(calculation.WidthFlowApertures.StandardDeviation);

            Assert.IsNaN(calculation.WaveReduction.BreakWaterHeight);
        }
    }
}