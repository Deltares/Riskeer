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
using Ringtoets.HeightStructures.IO.Configurations;

namespace Ringtoets.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO,
                                                                               nameof(HeightStructuresCalculationConfigurationReader));

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
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationHydraulicBoundaryLocationEmpty");
                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationOldAndNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationHydraulicBoundaryLocationOldAndNew");

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

                yield return new TestCaseData("invalidCalculationOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationEmpty");
                yield return new TestCaseData("invalidCalculationOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'nul' is invalid according to its datatype 'Double'")
                    .SetName("invalidCalculationOrientationNoDouble");

                yield return new TestCaseData("invalidCalculationStructureEmpty.xml",
                                              "The 'kunstwerk' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidCalculationStructureEmpty");

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
                yield return new TestCaseData("invalidMultipleLevelCrestStructureStochast.xml",
                                              "There is a duplicate key sequence 'kerendehoogte' for the 'uniqueStochastNameConstraint' key or unique identity constraint.")
                    .SetName("invalidMultipleLevelCrestStructureStochast");
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
            }
        }

        [Test]
        public void Constructor_WithFilePath_ReturnsNewInstance()
        {
            // Setup
            string existingPath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new HeightStructuresCalculationConfigurationReader(existingPath);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<HeightStructuresCalculationConfiguration>>(reader);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new HeightStructuresCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
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
            var reader = new HeightStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (HeightStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.StructureId);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.FailureProbabilityStructureWithErosion);

            Assert.IsNull(calculation.LevelCrestStructure);
            Assert.IsNull(calculation.AllowedLevelIncreaseStorage);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection);
            Assert.IsNull(calculation.ModelFactorSuperCriticalFlow);
            Assert.IsNull(calculation.StorageStructureArea);
            Assert.IsNull(calculation.StormDuration);
            Assert.IsNull(calculation.WidthFlowApertures);
            Assert.IsNull(calculation.ShouldIllustrationPointsBeCalculated);

            Assert.IsNull(calculation.WaveReduction);
        }

        [Test]
        [TestCase("validFullConfiguration")]
        [TestCase("validFullConfiguration_differentOrder")]
        public void Read_ValidFullConfigurations_ExpectedValues(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"{fileName}.xml");
            var reader = new HeightStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (HeightStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.AreEqual(67.1, calculation.StructureNormalOrientation);
            Assert.AreEqual("kunstwerk1", calculation.StructureId);
            Assert.AreEqual("Locatie1", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("profiel1", calculation.ForeshoreProfileId);
            Assert.AreEqual(1e-6, calculation.FailureProbabilityStructureWithErosion);

            Assert.AreEqual(4.3, calculation.LevelCrestStructure.Mean);
            Assert.AreEqual(0.1, calculation.LevelCrestStructure.StandardDeviation);

            Assert.AreEqual(0.2, calculation.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(0.01, calculation.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.AreEqual(2, calculation.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(0.1, calculation.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.AreEqual(15.2, calculation.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(0.1, calculation.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.AreEqual(1.10, calculation.ModelFactorSuperCriticalFlow.Mean);
            Assert.AreEqual(0.12, calculation.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.AreEqual(15000, calculation.StorageStructureArea.Mean);
            Assert.AreEqual(0.01, calculation.StorageStructureArea.VariationCoefficient);
            Assert.AreEqual(6.0, calculation.StormDuration.Mean);
            Assert.AreEqual(0.12, calculation.StormDuration.VariationCoefficient);
            Assert.AreEqual(15.2, calculation.WidthFlowApertures.Mean);
            Assert.AreEqual(0.1, calculation.WidthFlowApertures.StandardDeviation);

            Assert.AreEqual(ConfigurationBreakWaterType.Dam, calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(1.234, calculation.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(calculation.WaveReduction.UseBreakWater);
            Assert.IsTrue(calculation.WaveReduction.UseForeshoreProfile);
            Assert.IsTrue(calculation.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void Read_ValidFullConfigurationsContainingInfinity_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validFullConfigurationContainingInfinity.xml");
            var reader = new HeightStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (HeightStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsTrue(double.IsNegativeInfinity(calculation.StructureNormalOrientation.Value));
            Assert.IsNull(calculation.StructureId);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsTrue(double.IsNegativeInfinity(calculation.FailureProbabilityStructureWithErosion.Value));

            Assert.IsTrue(double.IsNegativeInfinity(calculation.LevelCrestStructure.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.LevelCrestStructure.StandardDeviation.Value));

            Assert.IsTrue(double.IsPositiveInfinity(calculation.AllowedLevelIncreaseStorage.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.AllowedLevelIncreaseStorage.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.CriticalOvertoppingDischarge.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.CriticalOvertoppingDischarge.VariationCoefficient.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.FlowWidthAtBottomProtection.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.FlowWidthAtBottomProtection.StandardDeviation.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.ModelFactorSuperCriticalFlow.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.ModelFactorSuperCriticalFlow.StandardDeviation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.StorageStructureArea.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.StorageStructureArea.VariationCoefficient.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.StormDuration.Mean.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.StormDuration.VariationCoefficient.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.WidthFlowApertures.Mean.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.WidthFlowApertures.StandardDeviation.Value));

            Assert.IsNull(calculation.WaveReduction.BreakWaterType);
            Assert.IsTrue(double.IsNegativeInfinity(calculation.WaveReduction.BreakWaterHeight.Value));
            Assert.IsNull(calculation.WaveReduction.UseBreakWater);
            Assert.IsNull(calculation.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(calculation.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void Read_ValidFullConfigurationsContainingNaN_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validFullConfigurationContainingNaN.xml");
            var reader = new HeightStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (HeightStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNaN(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.StructureId);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNaN(calculation.FailureProbabilityStructureWithErosion);

            Assert.IsNaN(calculation.LevelCrestStructure.Mean);
            Assert.IsNaN(calculation.LevelCrestStructure.StandardDeviation);

            Assert.IsNaN(calculation.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(calculation.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(calculation.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(calculation.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNaN(calculation.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(calculation.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(calculation.ModelFactorSuperCriticalFlow.Mean);
            Assert.IsNaN(calculation.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.IsNaN(calculation.StorageStructureArea.Mean);
            Assert.IsNaN(calculation.StorageStructureArea.VariationCoefficient);
            Assert.IsNaN(calculation.StormDuration.Mean);
            Assert.IsNaN(calculation.StormDuration.VariationCoefficient);
            Assert.IsNaN(calculation.WidthFlowApertures.Mean);
            Assert.IsNaN(calculation.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(calculation.WaveReduction.BreakWaterType);
            Assert.IsNaN(calculation.WaveReduction.BreakWaterHeight);
            Assert.IsNull(calculation.WaveReduction.UseBreakWater);
            Assert.IsNull(calculation.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(calculation.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void Read_ValidPartialConfigurations_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validPartialConfiguration.xml");
            var reader = new HeightStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (HeightStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.StructureId);
            Assert.AreEqual("Locatie1", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual("profiel1", calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.FailureProbabilityStructureWithErosion);

            Assert.IsNull(calculation.LevelCrestStructure);

            Assert.IsNull(calculation.AllowedLevelIncreaseStorage);
            Assert.AreEqual(2, calculation.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(0.1, calculation.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection);
            Assert.IsNull(calculation.ModelFactorSuperCriticalFlow);
            Assert.AreEqual(15000, calculation.StorageStructureArea.Mean);
            Assert.IsNull(calculation.StorageStructureArea.VariationCoefficient);
            Assert.AreEqual(6.0, calculation.StormDuration.Mean);
            Assert.IsNull(calculation.StormDuration.VariationCoefficient);
            Assert.IsNull(calculation.WidthFlowApertures.Mean);
            Assert.AreEqual(0.1, calculation.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(calculation.WaveReduction.BreakWaterType);
            Assert.IsNull(calculation.WaveReduction.BreakWaterHeight);
            Assert.IsTrue(calculation.WaveReduction.UseBreakWater);
            Assert.IsTrue(calculation.WaveReduction.UseForeshoreProfile);
            Assert.IsNull(calculation.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void Read_ValidConfigurationsEmptyStochastElements_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyStochastElements.xml");
            var reader = new HeightStructuresCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var calculation = (HeightStructuresCalculationConfiguration) readConfigurationItems.Single();

            Assert.IsNull(calculation.StructureNormalOrientation);
            Assert.IsNull(calculation.StructureId);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.FailureProbabilityStructureWithErosion);

            Assert.IsNull(calculation.LevelCrestStructure.Mean);
            Assert.IsNull(calculation.LevelCrestStructure.StandardDeviation);

            Assert.IsNull(calculation.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNull(calculation.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge.Mean);
            Assert.IsNull(calculation.CriticalOvertoppingDischarge.VariationCoefficient);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection.Mean);
            Assert.IsNull(calculation.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNull(calculation.ModelFactorSuperCriticalFlow.Mean);
            Assert.IsNull(calculation.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.IsNull(calculation.StorageStructureArea.Mean);
            Assert.IsNull(calculation.StorageStructureArea.VariationCoefficient);
            Assert.IsNull(calculation.StormDuration.Mean);
            Assert.IsNull(calculation.StormDuration.VariationCoefficient);
            Assert.IsNull(calculation.WidthFlowApertures.Mean);
            Assert.IsNull(calculation.WidthFlowApertures.StandardDeviation);

            Assert.IsNull(calculation.WaveReduction);
            Assert.IsNull(calculation.ShouldIllustrationPointsBeCalculated);
        }
    }
}