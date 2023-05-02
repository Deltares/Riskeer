﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraulicBoundaryDataConnectionValidatorTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));

        [Test]
        public void Validate_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataConnectionValidator.Validate(null, new TestHydraulicBoundaryLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", paramName);
        }

        [Test]
        public void Validate_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataConnectionValidator.Validate(new HydraulicBoundaryData(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Validate_HydraulicBoundaryLocationNotPartOfHydraulicBoundaryData_ThrowsArgumentException()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "hlcd.sqlite")
                }
            };

            // Call
            void Call() => HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData, new TestHydraulicBoundaryLocation());

            // Assert
            string message = Assert.Throws<ArgumentException>(Call).Message;
            Assert.AreEqual("'hydraulicBoundaryLocation' is not part of 'hydraulicBoundaryData'.", message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseNotExisting_ReturnsErrorMessage()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = "I_do_not_exist.sqlite",
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData, hydraulicBoundaryLocation);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand 'I_do_not_exist.sqlite': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseWithoutSettings_ReturnsErrorMessage()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            string invalidSettingsSchemaPath = Path.Combine(testDataPath, "invalidSettingsSchema");
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(invalidSettingsSchemaPath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = Path.Combine(invalidSettingsSchemaPath, "complete.sqlite"),
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData, hydraulicBoundaryLocation);

            // Assert
            const string expectedMessage = "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. De rekeninstellingen database heeft niet het juiste schema.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_UsePreprocessorClosureTrueWithoutPreprocessorClosureDatabase_ReturnsErrorMessage()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            string withoutPreprocessorClosurePath = Path.Combine(testDataPath, "withoutPreprocessorClosure");
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(withoutPreprocessorClosurePath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = Path.Combine(withoutPreprocessorClosurePath, "complete.sqlite"),
                        UsePreprocessorClosure = true,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData, hydraulicBoundaryLocation);

            // Assert
            string preprocessorClosureFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "hlcd_preprocClosure.sqlite");
            var expectedMessage = $"Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseWithDifferentVersion_ReturnsErrorMessage()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = hrdFilePath,
                        Version = "Dutch coast South19-11-2015 12:0113",
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData, hydraulicBoundaryLocation);

            // Assert
            var expectedMessage = $"De versie van de geselecteerde hydraulische belastingendatabase wijkt af van de versie zoals gevonden in het bestand '{hrdFilePath}'.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_ValidHydraulicBoundaryData_ReturnsNull(bool usePreprocessorClosure)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = Path.Combine(testDataPath, "complete.sqlite"),
                        Version = "Dutch coast South19-11-2015 12:0013",
                        UsePreprocessorClosure = usePreprocessorClosure,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            // Call
            string message = HydraulicBoundaryDataConnectionValidator.Validate(hydraulicBoundaryData, hydraulicBoundaryLocation);

            // Assert
            Assert.IsNull(message);
        }
    }
}