﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Xml.Schema;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Revetment.IO.Readers;

namespace Ringtoets.Revetment.IO.Test.Readers
{
    [TestFixture]
    public class WaveConditionsInputConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO,
                                                                               "WaveConditionsInputConfigurationReader");

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidCalculationMultipleHydraulicBoundaryLocation.xml",
                                              "The element 'berekening' has invalid child element 'hrlocatie'.")
                    .SetName("invalidCalculationMultipleHydraulicBoundaryLocation");
                yield return new TestCaseData("invalidCalculationMultipleForeshoreProfile.xml",
                                              "The element 'berekening' has invalid child element 'voorlandprofiel'.")
                    .SetName("invalidCalculationMultipleForeshoreProfile");
                yield return new TestCaseData("invalidCalculationMultipleOrientation.xml",
                                              "The element 'berekening' has invalid child element 'orientatie'.")
                    .SetName("invalidCalculationMultipleOrientation");
                yield return new TestCaseData("invalidCalculationMultipleLowerBoundaryRevetment.xml",
                                              "The element 'berekening' has invalid child element 'ondergrensbekleding'.")
                    .SetName("invalidCalculationMultipleLowerBoundaryRevetment");
                yield return new TestCaseData("invalidCalculationMultipleUpperBoundaryRevetment.xml",
                                              "The element 'berekening' has invalid child element 'bovengrensbekleding'.")
                    .SetName("invalidCalculationMultipleUpperBoundaryRevetment");
                yield return new TestCaseData("invalidCalculationMultipleLowerBoundaryWaterLevels.xml",
                                              "The element 'berekening' has invalid child element 'ondergrenswaterstanden'.")
                    .SetName("invalidCalculationMultipleLowerBoundaryWaterLevels");
                yield return new TestCaseData("invalidCalculationMultipleUpperBoundaryWaterLevels.xml",
                                              "The element 'berekening' has invalid child element 'bovengrenswaterstanden'.")
                    .SetName("invalidCalculationMultipleUpperBoundaryWaterLevels");
                yield return new TestCaseData("invalidCalculationMultipleStepSize.xml",
                                              "The element 'berekening' has invalid child element 'stapgrootte'.")
                    .SetName("invalidCalculationMultipleStepSize");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocation.xml",
                                              "The 'hrlocatie' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyHydraulicBoundaryLocation");
                yield return new TestCaseData("invalidConfigurationCalculationContainingEmptyForeshoreProfile.xml",
                                              "The 'voorlandprofiel' element is invalid - The value '' is invalid according to its datatype 'String' - The actual length is less than the MinLength value.")
                    .SetName("invalidConfigurationCalculationContainingEmptyForeshoreProfile");
                yield return new TestCaseData("invalidLowerBoundaryRevetmentEmpty.xml",
                                              "The 'ondergrensbekleding' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryRevetmentEmpty");
                yield return new TestCaseData("invalidLowerBoundaryRevetmentNoDouble.xml",
                                              "The 'ondergrensbekleding' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryRevetmentNoDouble");
                yield return new TestCaseData("invalidLowerBoundaryRevetmentWrongCulture.xml",
                                              "The 'ondergrensbekleding' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryRevetmentWrongCulture");
                yield return new TestCaseData("invalidUpperBoundaryRevetmentEmpty.xml",
                                              "The 'bovengrensbekleding' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryRevetmentEmpty");
                yield return new TestCaseData("invalidUpperBoundaryRevetmentNoDouble.xml",
                                              "The 'bovengrensbekleding' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryRevetmentNoDouble");
                yield return new TestCaseData("invalidUpperBoundaryRevetmentWrongCulture.xml",
                                              "The 'bovengrensbekleding' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryRevetmentWrongCulture");
                yield return new TestCaseData("invalidLowerBoundaryWaterLevelsEmpty.xml",
                                              "The 'ondergrenswaterstanden' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryWaterLevelsEmpty");
                yield return new TestCaseData("invalidLowerBoundaryWaterLevelsNoDouble.xml",
                                              "The 'ondergrenswaterstanden' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryWaterLevelsNoDouble");
                yield return new TestCaseData("invalidLowerBoundaryWaterLevelsWrongCulture.xml",
                                              "The 'ondergrenswaterstanden' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidLowerBoundaryWaterLevelsWrongCulture");
                yield return new TestCaseData("invalidUpperBoundaryWaterLevelsEmpty.xml",
                                              "The 'bovengrenswaterstanden' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryWaterLevelsEmpty");
                yield return new TestCaseData("invalidUpperBoundaryWaterLevelsNoDouble.xml",
                                              "The 'bovengrenswaterstanden' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryWaterLevelsNoDouble");
                yield return new TestCaseData("invalidUpperBoundaryWaterLevelsWrongCulture.xml",
                                              "The 'bovengrenswaterstanden' element is invalid - The value '1,2' is invalid according to its datatype 'Double'")
                    .SetName("invalidUpperBoundaryWaterLevelsWrongCulture");
                yield return new TestCaseData("invalidStepSizeEmpty.xml",
                                              "The 'stapgrootte' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidStepSizeEmpty");
                yield return new TestCaseData("invalidStepSizeNoDouble.xml",
                                              "The 'stapgrootte' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidStepSizeNoDouble");
                yield return new TestCaseData("invalidStepSizeWrongCulture.xml",
                                              "The 'stapgrootte' element is invalid - The value '0,5' is invalid according to its datatype 'Double'")
                    .SetName("invalidStepSizeWrongCulture");
                yield return new TestCaseData("invalidStepSizeUnknownValue.xml",
                                              "The 'stapgrootte' element is invalid - The value '1.3' is invalid according to its datatype 'Double' - The Enumeration constraint failed.")
                    .SetName("invalidStepSizeUnknownValue");
                yield return new TestCaseData("invalidOrientationEmpty.xml",
                                              "The 'orientatie' element is invalid - The value '' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationEmpty");
                yield return new TestCaseData("invalidOrientationNoDouble.xml",
                                              "The 'orientatie' element is invalid - The value 'string' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationNoDouble");
                yield return new TestCaseData("invalidOrientationWrongCulture.xml",
                                              "The 'orientatie' element is invalid - The value '0,5' is invalid according to its datatype 'Double'")
                    .SetName("invalidOrientationWrongCulture");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new WaveConditionsInputConfigurationReader(filePath);

            // Assert
            Assert.IsInstanceOf<ConfigurationReader<ReadWaveConditionsInput>>(reader);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new WaveConditionsInputConfigurationReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            Assert.IsTrue(exception.InnerException?.Message.Contains(expectedParsingMessage));
        }
    }
}