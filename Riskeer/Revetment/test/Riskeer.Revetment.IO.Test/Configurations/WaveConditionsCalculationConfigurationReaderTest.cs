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
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Revetment.IO.Configurations;

namespace Riskeer.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationReaderTest
    {
        private string validMainSchemaDefinition;

        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Revetment.IO,
                                                                               nameof(WaveConditionsCalculationConfigurationReader<WaveConditionsCalculationConfiguration>));

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocationOld.xml",
                                              "Element 'hrlocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocationNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocationNew");
                yield return new TestCaseData("invalidCalculationHydraulicBoundaryLocationOldAndNew.xml",
                                              "Element 'hblocatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationHydraulicBoundaryLocationOldAndNew");
                yield return new TestCaseData("invalidCalculationMultipleForeshoreProfile.xml",
                                              "Element 'voorlandprofiel' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleForeshoreProfile");
                yield return new TestCaseData("invalidCalculationMultipleOrientation.xml",
                                              "Element 'orientatie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleOrientation");
                yield return new TestCaseData("invalidCalculationMultipleLowerBoundaryRevetment.xml",
                                              "Element 'ondergrensbekleding' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleLowerBoundaryRevetment");
                yield return new TestCaseData("invalidCalculationMultipleUpperBoundaryRevetment.xml",
                                              "Element 'bovengrensbekleding' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleUpperBoundaryRevetment");
                yield return new TestCaseData("invalidCalculationMultipleLowerBoundaryWaterLevels.xml",
                                              "Element 'ondergrenswaterstanden' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleLowerBoundaryWaterLevels");
                yield return new TestCaseData("invalidCalculationMultipleUpperBoundaryWaterLevels.xml",
                                              "Element 'bovengrenswaterstanden' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleUpperBoundaryWaterLevels");
                yield return new TestCaseData("invalidCalculationMultipleStepSize.xml",
                                              "Element 'stapgrootte' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleStepSize");
                yield return new TestCaseData("invalidCalculationMultipleWaveReduction.xml",
                                              "Element 'golfreductie' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleWaveReduction");
                yield return new TestCaseData("invalidCalculationMultipleDamUsage.xml",
                                              "Element 'damgebruiken' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleDamUsage");
                yield return new TestCaseData("invalidCalculationMultipleDamType.xml",
                                              "Element 'damtype' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleDamType");
                yield return new TestCaseData("invalidCalculationMultipleDamHeight.xml",
                                              "Element 'damhoogte' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleDamHeight");
                yield return new TestCaseData("invalidCalculationMultipleForeshoreUsage.xml",
                                              "Element 'voorlandgebruiken' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleForeshoreUsage");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationOld.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationOld");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationNew.xml",
                                              "The 'hblocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocationNew");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyForeshoreProfile.xml",
                                              "The 'voorlandprofiel' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyForeshoreProfile");
                yield return new TestCaseData("invalidLowerBoundaryRevetmentEmpty.xml",
                                              "The 'ondergrensbekleding' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryRevetmentEmpty");
                yield return new TestCaseData("invalidLowerBoundaryRevetmentNoDouble.xml",
                                              "The 'ondergrensbekleding' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryRevetmentNoDouble");
                yield return new TestCaseData("invalidUpperBoundaryRevetmentEmpty.xml",
                                              "The 'bovengrensbekleding' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryRevetmentEmpty");
                yield return new TestCaseData("invalidUpperBoundaryRevetmentNoDouble.xml",
                                              "The 'bovengrensbekleding' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryRevetmentNoDouble");
                yield return new TestCaseData("invalidLowerBoundaryWaterLevelsEmpty.xml",
                                              "The 'ondergrenswaterstanden' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryWaterLevelsEmpty");
                yield return new TestCaseData("invalidLowerBoundaryWaterLevelsNoDouble.xml",
                                              "The 'ondergrenswaterstanden' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryWaterLevelsNoDouble");
                yield return new TestCaseData("invalidUpperBoundaryWaterLevelsEmpty.xml",
                                              "The 'bovengrenswaterstanden' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryWaterLevelsEmpty");
                yield return new TestCaseData("invalidUpperBoundaryWaterLevelsNoDouble.xml",
                                              "The 'bovengrenswaterstanden' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryWaterLevelsNoDouble");
                yield return new TestCaseData("invalidStepSizeEmpty.xml",
                                              "The 'stapgrootte' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStepSizeEmpty");
                yield return new TestCaseData("invalidStepSizeNoDouble.xml",
                                              "The 'stapgrootte' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStepSizeNoDouble");
                yield return new TestCaseData("invalidStepSizeUnknownValue.xml",
                                              "The 'stapgrootte' element is invalid - The value '1.3' is invalid according to its datatype 'Double' - The Enumeration constraint failed.")
                    .SetName("invalidStepSizeUnknownValue");
                yield return new TestCaseData("invalidOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationEmpty");
                yield return new TestCaseData("invalidOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationNoDouble");
                yield return new TestCaseData("invalidDamUsageEmpty.xml",
                                              "The 'damgebruiken' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidDamUsageEmpty");
                yield return new TestCaseData("invalidDamUsageNoBoolean.xml",
                                              "The 'damgebruiken' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidDamUsageNoBoolean");
                yield return new TestCaseData("invalidDamTypeEmpty.xml",
                                              "The 'damtype' element is invalid - The value '' is invalid according to its datatype 'String'")
                    .SetName("invalidDamTypeEmpty");
                yield return new TestCaseData("invalidDamTypeUnknownValue.xml",
                                              "The 'damtype' element is invalid - The value 'text' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidDamTypeUnknownValue");
                yield return new TestCaseData("invalidDamHeightEmpty.xml",
                                              "The 'damhoogte' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidDamHeightEmpty");
                yield return new TestCaseData("invalidDamHeightNoDouble.xml",
                                              "The 'damhoogte' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidDamHeightNoDouble");
                yield return new TestCaseData("invalidForeshoreUsageEmpty.xml",
                                              "The 'voorlandgebruiken' element is invalid - The value '' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidForeshoreUsageEmpty");
                yield return new TestCaseData("invalidForeshoreUsageNoBoolean.xml",
                                              "The 'voorlandgebruiken' element is invalid - The value 'string' is invalid according to its datatype 'Boolean'")
                    .SetName("invalidForeshoreUsageNoBoolean");
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validMainSchemaDefinition = File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema.xsd"));
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationReader<WaveConditionsCalculationConfiguration>>(reader);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadWaveConditionsInput()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (WaveConditionsCalculationConfiguration) readItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.IsNull(calculation.UpperBoundaryRevetment);
            Assert.IsNull(calculation.LowerBoundaryRevetment);
            Assert.IsNull(calculation.UpperBoundaryWaterLevels);
            Assert.IsNull(calculation.LowerBoundaryWaterLevels);
            Assert.IsNull(calculation.StepSize);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.Orientation);
            Assert.IsNull(calculation.WaveReduction);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingEmptyWaveReduction_ReturnExpectedReadWaveConditionsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingEmptyWaveReduction.xml");
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (WaveConditionsCalculationConfiguration) readItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.WaveReduction.UseBreakWater);
            Assert.IsNull(calculation.WaveReduction.BreakWaterType);
            Assert.IsNull(calculation.WaveReduction.BreakWaterHeight);
            Assert.IsNull(calculation.WaveReduction.UseForeshoreProfile);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingNaNs_ReturnExpectedReadWaveConditionsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContainingNaNs.xml");
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (WaveConditionsCalculationConfiguration) readItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNaN(calculation.UpperBoundaryRevetment);
            Assert.IsNaN(calculation.LowerBoundaryRevetment);
            Assert.IsNaN(calculation.UpperBoundaryWaterLevels);
            Assert.IsNaN(calculation.LowerBoundaryWaterLevels);
            Assert.IsNaN(calculation.Orientation);
            Assert.IsNaN(calculation.WaveReduction.BreakWaterHeight);
        }

        [Test]
        public void Read_ValidConfigurationWithCalculationContainingInfinities_ReturnExpectedReadWaveConditionsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationCalculationContaininInfinities.xml");
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (WaveConditionsCalculationConfiguration) readItems.Single();

            Assert.IsNotNull(calculation);

            Assert.NotNull(calculation.UpperBoundaryRevetment);
            Assert.NotNull(calculation.LowerBoundaryRevetment);
            Assert.NotNull(calculation.UpperBoundaryWaterLevels);
            Assert.NotNull(calculation.LowerBoundaryWaterLevels);
            Assert.NotNull(calculation.Orientation);
            Assert.NotNull(calculation.WaveReduction.BreakWaterHeight);

            Assert.IsTrue(double.IsPositiveInfinity(calculation.UpperBoundaryRevetment.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.LowerBoundaryRevetment.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.UpperBoundaryWaterLevels.Value));
            Assert.IsTrue(double.IsNegativeInfinity(calculation.LowerBoundaryWaterLevels.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.Orientation.Value));
            Assert.IsTrue(double.IsPositiveInfinity(calculation.WaveReduction.BreakWaterHeight.Value));
        }

        [Test]
        [TestCase("validConfigurationFullCalculationOld.xml")]
        [TestCase("validConfigurationFullCalculation_differentOrder_old.xml")]
        [TestCase("validConfigurationFullCalculationNew.xml")]
        [TestCase("validConfigurationFullCalculation_differentOrder_new.xml")]
        public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadWaveConditionsCalculation(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (WaveConditionsCalculationConfiguration) readItems.Single();

            Assert.IsNotNull(calculation);
            Assert.AreEqual("Locatie", calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(1.1, calculation.UpperBoundaryRevetment);
            Assert.AreEqual(2.2, calculation.LowerBoundaryRevetment);
            Assert.AreEqual(3.3, calculation.UpperBoundaryWaterLevels);
            Assert.AreEqual(4.4, calculation.LowerBoundaryWaterLevels);
            Assert.AreEqual(ConfigurationWaveConditionsInputStepSize.Half, calculation.StepSize);
            Assert.AreEqual("Voorlandprofiel", calculation.ForeshoreProfileId);
            Assert.AreEqual(5.5, calculation.Orientation);
            Assert.IsTrue(calculation.WaveReduction.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Caisson, calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(6.6, calculation.WaveReduction.BreakWaterHeight);
            Assert.IsFalse(calculation.WaveReduction.UseForeshoreProfile);
        }

        [Test]
        public void Read_ValidConfigurationWithPartialCalculation_ReturnExpectedReadWaveConditionsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationPartialCalculation.xml");
            var reader = new TestWaveConditionsCalculationConfigurationReader(filePath, validMainSchemaDefinition);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (WaveConditionsCalculationConfiguration) readItems.Single();

            Assert.IsNotNull(calculation);
            Assert.IsNull(calculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(1.1, calculation.UpperBoundaryRevetment);
            Assert.AreEqual(2.2, calculation.LowerBoundaryRevetment);
            Assert.IsNull(calculation.UpperBoundaryWaterLevels);
            Assert.IsNull(calculation.LowerBoundaryWaterLevels);
            Assert.AreEqual(ConfigurationWaveConditionsInputStepSize.Half, calculation.StepSize);
            Assert.IsNull(calculation.ForeshoreProfileId);
            Assert.IsNull(calculation.Orientation);
            Assert.IsTrue(calculation.WaveReduction.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Caisson, calculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(3.3, calculation.WaveReduction.BreakWaterHeight);
            Assert.IsNull(calculation.WaveReduction.UseForeshoreProfile);
        }

        private class TestWaveConditionsCalculationConfigurationReader 
            : WaveConditionsCalculationConfigurationReader<WaveConditionsCalculationConfiguration>
        {
            public TestWaveConditionsCalculationConfigurationReader(string xmlFilePath, string configurationSchema)
                : base(xmlFilePath, configurationSchema) {}

            protected override WaveConditionsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
            {
                var configuration = new WaveConditionsCalculationConfiguration("test");
                ParseCalculationElementData(calculationElement, configuration);
                return configuration;
            }
        }
    }
}