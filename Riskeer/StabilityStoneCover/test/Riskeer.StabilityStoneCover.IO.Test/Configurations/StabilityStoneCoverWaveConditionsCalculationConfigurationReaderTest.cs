﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Revetment.IO.Configurations;
using Riskeer.StabilityStoneCover.IO.Configurations;

namespace Riskeer.StabilityStoneCover.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.StabilityStoneCover.IO,
                                                                               nameof(StabilityStoneCoverWaveConditionsCalculationConfigurationReader));

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidCalculationMultipleCategoryType.xml",
                                              "Element 'categoriegrens' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleCategoryType");
                yield return new TestCaseData("invalidCategoryTypeUnknownValue.xml",
                                              "The 'categoriegrens' element is invalid - The value 'F' is invalid according to its datatype 'categoriegrensType' - The Enumeration constraint failed.")
                    .SetName("invalidCategoryTypeUnknownValue");

                yield return new TestCaseData("invalidCalculationMultipleRevetmentType.xml",
                                              "Element 'typebekleding' cannot appear more than once if content model type is \"all\".")
                    .SetName("invalidCalculationMultipleRevetmentType");
                yield return new TestCaseData("invalidRevetmentTypeUnknownValue.xml",
                                              "The 'typebekleding' element is invalid - The value 'Steen' is invalid according to its datatype 'String' - The Enumeration constraint failed.")
                    .SetName("invalidRevetmentTypeUnknownValue");
                yield return new TestCaseData("invalidRevetmentType.xml",
                                              "The element 'berekening' has invalid child element 'typebelasting'.")
                    .SetName("invalidRevetmentType");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new StabilityStoneCoverWaveConditionsCalculationConfigurationReader(filePath);

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationConfigurationReader<StabilityStoneCoverWaveConditionsCalculationConfiguration>>(reader);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsCalculationConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            StringAssert.Contains(expectedParsingMessage, exception.InnerException?.Message);
        }

        [Test]
        public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadWaveConditionsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationFullCalculation.xml");
            var reader = new StabilityStoneCoverWaveConditionsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var calculation = (StabilityStoneCoverWaveConditionsCalculationConfiguration)readItems.Single();

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
            Assert.AreEqual(ConfigurationAssessmentSectionCategoryType.SignalingNorm, calculation.CategoryType);
            Assert.AreEqual(ConfigurationStabilityStoneCoverCalculationType.Columns, calculation.CalculationType);
        }
    }
}