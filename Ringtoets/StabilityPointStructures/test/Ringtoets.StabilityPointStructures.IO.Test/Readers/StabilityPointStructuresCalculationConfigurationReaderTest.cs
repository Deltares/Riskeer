// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.IO.Readers;
using Ringtoets.StabilityPointStructures.IO.Readers;

namespace Ringtoets.StabilityPointStructures.IO.Test.Readers
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
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionEmpty.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionEmpty");
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionNoDouble.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionNoDouble");
                yield return new TestCaseData("invalidCalculationFailureProbabilityStructureWithErosionWrongCulture.xml",
                                              "The 'faalkansgegevenerosiebodem' element is invalid - The value '0,5' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationFailureProbabilityStructureWithErosionWrongCulture");

                yield return new TestCaseData("invalidCalculationForeshoreProfileEmpty.xml",
                                              "The 'voorlandprofiel' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationForeshoreProfileEmpty");

                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationEmpty.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationHydraulicBoundaryLocationEmpty");

                yield return new TestCaseData("invalidCalculationMultipleFailureProbabilityStructureWithErosion.xml",
                                              "Element 'faalkansgegevenerosiebodem' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleFailureProbabilityStructureWithErosion");
                yield return new TestCaseData("invalidCalculationMultipleForeshoreProfile.xml",
                                              "Element 'voorlandprofiel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleForeshoreProfile");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocation.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
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
                yield return new TestCaseData("invalidCalculationMultipleInflowModelTypes.xml",
                                              "Element 'instroommodel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleInflowModelTypes");

                yield return new TestCaseData("invalidCalculationOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationEmpty");
                yield return new TestCaseData("invalidCalculationOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationNoDouble");
                yield return new TestCaseData("invalidCalculationOrientationWrongCulture.xml",
                                              "The 'orientatie' element is invalid - The value '0,5' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationWrongCulture");

                yield return new TestCaseData("invalidCalculationStructureEmpty.xml",
                                              "The 'kunstwerk' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationStructureEmpty");

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
                yield return new TestCaseData("invalidStochastMeanWrongCulture.xml",
                                              "The 'verwachtingswaarde' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastMeanWrongCulture");
                yield return new TestCaseData("invalidStochastStandardDeviationEmpty.xml",
                                              "The 'standaardafwijking' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationEmpty");
                yield return new TestCaseData("invalidStochastStandardDeviationNoDouble.xml",
                                              "The 'standaardafwijking' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationNoDouble");
                yield return new TestCaseData("invalidStochastStandardDeviationWrongCulture.xml",
                                              "The 'standaardafwijking' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastStandardDeviationWrongCulture");
                yield return new TestCaseData("invalidStochastVariationCoefficientEmpty.xml",
                                              "The 'variatiecoefficient' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientEmpty");
                yield return new TestCaseData("invalidStochastVariationCoefficientNoDouble.xml",
                                              "The 'variatiecoefficient' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientNoDouble");
                yield return new TestCaseData("invalidStochastVariationCoefficientWrongCulture.xml",
                                              "The 'variatiecoefficient' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidStochastVariationCoefficientWrongCulture");

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
                yield return new TestCaseData("invalidBreakWaterHeightWrongCulture.xml",
                                              "The 'damhoogte' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidBreakWaterHeightWrongCulture");
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
        [TestCase("validFullConfiguration")]
        public void Read_ValidFullConfigurations_ExpectedValues(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new StabilityPointStructuresCalculationConfigurationReader(filePath);

            // Call
            IList<IConfigurationItem> readConfigurationItems = reader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var calculation = (StabilityPointStructuresCalculationConfiguration) readConfigurationItems[0];

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

            Assert.AreEqual("profiel1", calculation.ForeshoreProfileName);

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

            Assert.AreEqual(0.10, calculation.ModelFactorSuperCriticalFlow.Mean);
            Assert.AreEqual(0.12, calculation.ModelFactorSuperCriticalFlow.StandardDeviation);

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

            Assert.AreEqual("kunstwerk1", calculation.StructureName);

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
    }
}