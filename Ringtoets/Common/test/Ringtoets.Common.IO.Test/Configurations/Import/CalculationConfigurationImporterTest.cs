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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.Configurations.Import
{
    [TestFixture]
    public class CalculationConfigurationImporterTest
    {
        private readonly string readerPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(CalculationConfigurationReader));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new CalculationConfigurationImporter("",
                                                                new CalculationGroup());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<CalculationGroup>>(importer);
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new CalculationConfigurationImporter(filePath,
                                                                new CalculationGroup());

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam. " + Environment.NewLine +
                                     "Er is geen berekeningenconfiguratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist");

            var importer = new CalculationConfigurationImporter(filePath,
                                                                new CalculationGroup());

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet. " + Environment.NewLine +
                                     "Er is geen berekeningenconfiguratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_InvalidFile_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "invalidFolderNoName.xml");
            var importer = new CalculationConfigurationImporter(filePath,
                                                                new CalculationGroup());

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith($"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie voor de berekeningen beschrijft is niet geldig.", msgs[0]);
            });

            Assert.IsFalse(importSuccessful);
        }

        [Test]
        [TestCase("Inlezen")]
        [TestCase("Valideren")]
        public void Import_CancelingImport_CancelImportAndLog(string expectedProgressMessage)
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");
            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedProgressMessage))
                {
                    importer.Cancel();
                }
            });

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Berekeningenconfiguratie importeren afgebroken. Geen gegevens gewijzigd.", 1);
            CollectionAssert.IsEmpty(calculationGroup.Children);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void GivenImport_WhenImporting_ThenExpectedProgressMessagesGenerated()
        {
            // Given
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");
            var progressChangeNotifications = new List<ProgressNotification>();

            var importer = new CalculationConfigurationImporter(filePath, new CalculationGroup());
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // When
            importer.Import();

            // Then
            var expectedProgressNotifications = new[]
            {
                new ProgressNotification("Inlezen berekeningenconfiguratie.", 1, 3),
                new ProgressNotification("Valideren berekeningenconfiguratie.", 2, 3),
                new ProgressNotification("Geïmporteerde data toevoegen aan het toetsspoor.", 3, 3)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressNotifications, progressChangeNotifications);
        }

        [Test]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            AssertCalculationGroup(GetExpectedNestedData(), calculationGroup);
        }

        [Test]
        [TestCase(true, true, 1.2, ConfigurationBreakWaterType.Wall, BreakWaterType.Wall)]
        [TestCase(false, false, 2.2, ConfigurationBreakWaterType.Caisson, BreakWaterType.Caisson)]
        [TestCase(false, true, 11.332, ConfigurationBreakWaterType.Wall, BreakWaterType.Wall)]
        [TestCase(true, false, 9.3, ConfigurationBreakWaterType.Dam, BreakWaterType.Dam)]
        public void ReadWaveReduction_DifferentScenarios_CorrectParametersSet(bool useForeshoreProfile, bool useBreakWater, double height, ConfigurationBreakWaterType type, BreakWaterType expectedType)
        {
            // Setup
            var testInput = new TestInputWithForeshoreProfileAndBreakWater(new BreakWater(BreakWaterType.Caisson, 0.0));

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            var waveReductionConfiguration = new WaveReductionConfiguration
            {
                UseForeshoreProfile = useForeshoreProfile,
                UseBreakWater = useBreakWater,
                BreakWaterHeight = height,
                BreakWaterType = type
            };

            // Call
            importer.PublicReadWaveReductionParameters(waveReductionConfiguration, testInput);

            // Assert
            Assert.AreEqual(testInput.UseForeshore, useForeshoreProfile);
            Assert.AreEqual(testInput.UseBreakWater, useBreakWater);
            Assert.AreEqual(testInput.BreakWater.Height, height, testInput.BreakWater.Height.GetAccuracy());
            Assert.AreEqual(testInput.BreakWater.Type, expectedType);
        }

        [Test]
        public void ReadWaveReduction_WithoutConfiguration_ParametersUnchanged()
        {
            // Setup
            var random = new Random(21);
            bool useForeshoreProfile = random.NextBoolean();
            bool useBreakWater = random.NextBoolean();
            double height = random.NextDouble();
            var breakWaterType = random.NextEnumValue<BreakWaterType>();

            var testInput = new TestInputWithForeshoreProfileAndBreakWater(new BreakWater(breakWaterType, height))
            {
                UseBreakWater = useBreakWater,
                UseForeshore = useForeshoreProfile
            };

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            // Call
            importer.PublicReadWaveReductionParameters(null, testInput);

            // Assert
            Assert.AreEqual(testInput.UseForeshore, useForeshoreProfile);
            Assert.AreEqual(testInput.UseBreakWater, useBreakWater);
            Assert.AreEqual(testInput.BreakWater.Height, height, testInput.BreakWater.Height.GetAccuracy());
            Assert.AreEqual(testInput.BreakWater.Type, breakWaterType);
        }

        [Test]
        public void ReadWaveReduction_WithConfigurationWithMissingParameter_MissingParameterUnchanged([Values(0, 1, 2, 3)] int parameterNotSet)
        {
            // Setup
            const bool useForeshoreProfile = false;
            const bool useBreakWater = false;
            const double height = 2.55;
            const BreakWaterType breakWaterType = BreakWaterType.Dam;

            const bool newUseForeshoreProfile = true;
            const bool newUseBreakWater = true;
            const double newheight = 11.1;
            const ConfigurationBreakWaterType newBreakWaterType = ConfigurationBreakWaterType.Wall;
            const BreakWaterType expectedNewBreakWaterType = BreakWaterType.Wall;

            var testInput = new TestInputWithForeshoreProfileAndBreakWater(new BreakWater(breakWaterType, height))
            {
                UseBreakWater = useBreakWater,
                UseForeshore = useForeshoreProfile
            };

            var waveReductionConfiguration = new WaveReductionConfiguration();
            if (parameterNotSet != 0)
            {
                waveReductionConfiguration.UseForeshoreProfile = newUseForeshoreProfile;
            }

            if (parameterNotSet != 1)
            {
                waveReductionConfiguration.UseBreakWater = newUseBreakWater;
            }

            if (parameterNotSet != 2)
            {
                waveReductionConfiguration.BreakWaterHeight = newheight;
            }

            if (parameterNotSet != 3)
            {
                waveReductionConfiguration.BreakWaterType = newBreakWaterType;
            }

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            // Call
            importer.PublicReadWaveReductionParameters(waveReductionConfiguration, testInput);

            // Assert
            Assert.AreEqual(testInput.UseForeshore, parameterNotSet == 0 ? useForeshoreProfile : newUseForeshoreProfile);
            Assert.AreEqual(testInput.UseBreakWater, parameterNotSet == 1 ? useBreakWater : newUseBreakWater);
            Assert.AreEqual(testInput.BreakWater.Height, parameterNotSet == 2 ? height : newheight, testInput.BreakWater.Height.GetAccuracy());
            Assert.AreEqual(testInput.BreakWater.Type, parameterNotSet == 3 ? breakWaterType : expectedNewBreakWaterType);
        }

        [Test]
        public void TryReadHydraulicBoundaryLocation_NoCalculationName_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            HydraulicBoundaryLocation location;

            // Call
            TestDelegate test = () => importer.PublicTryReadHydraulicBoundaryLocation(null, null, Enumerable.Empty<HydraulicBoundaryLocation>(), out location);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationName", exception.ParamName);
        }

        [Test]
        public void TryReadHydraulicBoundaryLocation_NoHydraulicBoundaryLocations_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            HydraulicBoundaryLocation location;

            // Call
            TestDelegate test = () => importer.PublicTryReadHydraulicBoundaryLocation(null, "name", null, out location);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void TryReadHydraulicBoundaryLocation_NoHydraulicBoundaryLocationToFindHydraulicBoundaryLocationsEmpty_ReturnsTrue()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            HydraulicBoundaryLocation location;

            // Call
            bool valid = importer.PublicTryReadHydraulicBoundaryLocation(null, "name", Enumerable.Empty<HydraulicBoundaryLocation>(), out location);

            // Assert
            Assert.IsTrue(valid);
            Assert.IsNull(location);
        }

        [Test]
        public void TryReadHydraulicBoundaryLocation_WithHydraulicBoundaryLocationToFindHydraulicBoundaryLocationsEmpty_LogsErrorReturnsFalse()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            HydraulicBoundaryLocation location = null;
            var valid = true;

            const string locationName = "someName";
            const string calculationName = "name";

            // Call
            Action validate = () => valid = importer.PublicTryReadHydraulicBoundaryLocation(locationName, calculationName, Enumerable.Empty<HydraulicBoundaryLocation>(), out location);

            // Assert
            string expectedMessage = $"De hydraulische belastingenlocatie '{locationName}' bestaat niet. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            Assert.IsNull(location);
        }

        [Test]
        public void TryReadHydraulicBoundaryLocation_WithHydraulicBoundaryLocationToFindHydraulicBoundaryLocationsContainsLocation_ReturnsTrue()
        {
            // Setup
            const string locationName = "someName";
            const string calculationName = "name";

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            HydraulicBoundaryLocation expectedLocation = new TestHydraulicBoundaryLocation(locationName);
            HydraulicBoundaryLocation location;

            // Call
            bool valid = importer.PublicTryReadHydraulicBoundaryLocation(locationName,
                                                                         calculationName,
                                                                         new[]
                                                                         {
                                                                             new TestHydraulicBoundaryLocation("otherNameA"),
                                                                             expectedLocation,
                                                                             new TestHydraulicBoundaryLocation("otherNameB")
                                                                         },
                                                                         out location);

            // Assert
            Assert.IsTrue(valid);
            Assert.AreSame(expectedLocation, location);
        }

        [Test]
        public void TryReadForeshoreProfile_NoCalculationName_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            ForeshoreProfile profile;

            // Call
            TestDelegate test = () => importer.PublicTryReadForeshoreProfile(null, null, Enumerable.Empty<ForeshoreProfile>(), out profile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationName", exception.ParamName);
        }

        [Test]
        public void TryReadForeshoreProfile_NoForeshoreProfiles_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            ForeshoreProfile profile;

            // Call
            TestDelegate test = () => importer.PublicTryReadForeshoreProfile(null, "name", null, out profile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void TryReadForeshoreProfile_NoForeshoreProfileToFindForeshoreProfilesEmpty_ReturnsTrue()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            ForeshoreProfile profile;

            // Call
            bool valid = importer.PublicTryReadForeshoreProfile(null, "name", Enumerable.Empty<ForeshoreProfile>(), out profile);

            // Assert
            Assert.IsTrue(valid);
            Assert.IsNull(profile);
        }

        [Test]
        public void TryReadForeshoreProfile_WithForeshoreProfileToFindForeshoreProfilesEmpty_LogsErrorReturnsFalse()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            ForeshoreProfile profile = null;
            var valid = true;

            const string profileName = "someName";
            const string calculationName = "name";

            // Call
            Action validate = () => valid = importer.PublicTryReadForeshoreProfile(profileName, calculationName, Enumerable.Empty<ForeshoreProfile>(), out profile);

            // Assert
            string expectedMessage = $"Het voorlandprofiel met ID '{profileName}' bestaat niet. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            Assert.IsNull(profile);
        }

        [Test]
        public void TryReadForeshoreProfile_WithForeshoreProfileToFindForeshoreProfilesContainsProfile_ReturnsTrue()
        {
            // Setup
            const string profileName = "someName";
            const string calculationName = "name";

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            ForeshoreProfile expectedProfile = new TestForeshoreProfile(profileName);
            ForeshoreProfile profile;

            // Call
            bool valid = importer.PublicTryReadForeshoreProfile(profileName,
                                                                calculationName,
                                                                new[]
                                                                {
                                                                    new TestForeshoreProfile("otherNameA"),
                                                                    expectedProfile,
                                                                    new TestForeshoreProfile("otherNameB")
                                                                },
                                                                out profile);

            // Assert
            Assert.IsTrue(valid);
            Assert.AreSame(expectedProfile, profile);
        }

        [Test]
        public void TryReadStructure_NoCalculationName_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            StructureBase structure;

            // Call
            TestDelegate test = () => importer.PublicTryReadStructure(null, null, Enumerable.Empty<TestStructure>(), out structure);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationName", exception.ParamName);
        }

        [Test]
        public void TryReadStructure_NoStructures_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            StructureBase structure;

            // Call
            TestDelegate test = () => importer.PublicTryReadStructure(null, "name", null, out structure);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structures", exception.ParamName);
        }

        [Test]
        public void TryReadStructure_NoStructureToFindStructuresEmpty_ReturnsTrue()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            StructureBase structure;

            // Call
            bool valid = importer.PublicTryReadStructure(null, "name", Enumerable.Empty<StructureBase>(), out structure);

            // Assert
            Assert.IsTrue(valid);
            Assert.IsNull(structure);
        }

        [Test]
        public void TryReadStructure_WithStructureToFindStructuresEmpty_LogsErrorReturnsFalse()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            StructureBase profile = null;
            var valid = true;

            const string structureId = "someAwesomeId";
            const string calculationName = "name";

            // Call
            Action validate = () => valid = importer.PublicTryReadStructure(structureId, calculationName, Enumerable.Empty<StructureBase>(), out profile);

            // Assert
            string expectedMessage = $"Het kunstwerk met ID '{structureId}' bestaat niet. Berekening '{calculationName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(validate, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
            Assert.IsNull(profile);
        }

        [Test]
        public void TryReadStructure_WithStructureToFindStructuresContainsStructure_ReturnsTrue()
        {
            // Setup
            const string structureId = "someId";
            const string calculationName = "name";

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new CalculationConfigurationImporter(filePath,
                                                                calculationGroup);

            var expectedProfile = new TestStructure(structureId);
            StructureBase structure;

            // Call
            bool valid = importer.PublicTryReadStructure(structureId,
                                                         calculationName,
                                                         new[]
                                                         {
                                                             new TestStructure("otherIdA"),
                                                             expectedProfile,
                                                             new TestStructure("otherIdB")
                                                         },
                                                         out structure);

            // Assert
            Assert.IsTrue(valid);
            Assert.AreSame(expectedProfile, structure);
        }

        [Test]
        public void PublicTrySetScenarioParameters_NoScenario_NoDataAddedToModelReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationScenario = mockRepository.StrictMock<ICalculationScenario>();
            mockRepository.ReplayAll();

            var importer = new CalculationConfigurationImporter(Path.Combine(readerPath, "validConfiguration.xml"),
                                                                new CalculationGroup());

            // Call
            bool successful = importer.PublicTrySetScenarioParameters(null, calculationScenario);

            // Assert
            Assert.IsTrue(successful);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PublicTrySetScenarioParameters_ScenarioEmpty_LogMessageReturnsFalse()
        {
            // Setup
            const string calculationScenarioName = "calculationScenario";

            var mockRepository = new MockRepository();
            var calculationScenario = mockRepository.StrictMock<ICalculationScenario>();
            calculationScenario.Expect(cs => cs.Name).Return(calculationScenarioName);
            mockRepository.ReplayAll();

            var importer = new CalculationConfigurationImporter(Path.Combine(readerPath, "validConfiguration.xml"),
                                                                new CalculationGroup());

            // Call
            var successful = true;
            Action call = () => successful = importer.PublicTrySetScenarioParameters(new ScenarioConfiguration(),
                                                                                     calculationScenario);

            // Assert
            string expectedMessage = "In een berekening moet voor het scenario tenminste de relevantie of contributie worden opgegeven. " +
                                     $"Berekening '{calculationScenarioName}' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(successful);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_ScenarioWithContributionSet_DataAddedToModelReturnsTrue()
        {
            // Setup
            var random = new Random(45);
            double contribution = random.NextDouble();

            var mockRepository = new MockRepository();
            var calculationScenario = mockRepository.StrictMock<ICalculationScenario>();
            calculationScenario.Expect(cs => cs.Contribution)
                               .SetPropertyWithArgument((RoundedDouble) (contribution / 100));
            mockRepository.ReplayAll();

            var importer = new CalculationConfigurationImporter(Path.Combine(readerPath, "validConfiguration.xml"),
                                                                new CalculationGroup());

            // Call
            bool successful = importer.PublicTrySetScenarioParameters(new ScenarioConfiguration
            {
                Contribution = contribution
            }, calculationScenario);

            // Assert
            Assert.IsTrue(successful);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_ScenarioWithRevelantSet_DataAddedToModelReturnsTrue()
        {
            // Setup
            var random = new Random(45);
            bool isRelevant = random.NextBoolean();

            var mockRepository = new MockRepository();
            var calculationScenario = mockRepository.StrictMock<ICalculationScenario>();
            calculationScenario.Expect(cs => cs.IsRelevant).SetPropertyWithArgument(isRelevant);
            mockRepository.ReplayAll();

            var importer = new CalculationConfigurationImporter(Path.Combine(readerPath, "validConfiguration.xml"),
                                                                new CalculationGroup());

            // Call
            bool successful = importer.PublicTrySetScenarioParameters(new ScenarioConfiguration
            {
                IsRelevant = isRelevant
            }, calculationScenario);

            // Assert
            Assert.IsTrue(successful);

            mockRepository.VerifyAll();
        }

        private class CalculationConfigurationImporter : CalculationConfigurationImporter<CalculationConfigurationReader, ReadCalculation>
        {
            public CalculationConfigurationImporter(string filePath, CalculationGroup importTarget)
                : base(filePath, importTarget) {}

            public void PublicReadWaveReductionParameters<T>(WaveReductionConfiguration waveReduction, T input)
                where T : IUseBreakWater, IUseForeshore
            {
                SetWaveReductionParameters(waveReduction, input);
            }

            public bool PublicTryReadHydraulicBoundaryLocation(string locationName, string calculationName, IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, out HydraulicBoundaryLocation location)
            {
                return TryReadHydraulicBoundaryLocation(locationName, calculationName, hydraulicBoundaryLocations, out location);
            }

            public bool PublicTryReadForeshoreProfile(string locationName, string calculationName, IEnumerable<ForeshoreProfile> foreshoreProfiles, out ForeshoreProfile location)
            {
                return TryReadForeshoreProfile(locationName, calculationName, foreshoreProfiles, out location);
            }

            public bool PublicTryReadStructure(string locationName, string calculationName, IEnumerable<StructureBase> structures, out StructureBase location)
            {
                return TryReadStructure(locationName, calculationName, structures, out location);
            }

            public bool PublicTrySetScenarioParameters(ScenarioConfiguration scenarioConfiguration, ICalculationScenario scenario)
            {
                return TrySetScenarioParameters(scenarioConfiguration, scenario);
            }

            protected override CalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
            {
                return new CalculationConfigurationReader(xmlFilePath);
            }

            protected override ICalculation ParseReadCalculation(ReadCalculation readCalculation)
            {
                return new TestCalculation
                {
                    Name = readCalculation.Name
                };
            }
        }

        private class CalculationConfigurationReader : CalculationConfigurationReader<ReadCalculation>
        {
            private static readonly string mainSchemaDefinition =
                File.ReadAllText(Path.Combine(TestHelper.GetTestDataPath(
                                                  TestDataPath.Ringtoets.Common.IO,
                                                  "CalculationConfigurationReader"),
                                              "validConfigurationSchema.xsd"));

            public CalculationConfigurationReader(string xmlFilePath)
                : base(xmlFilePath, mainSchemaDefinition, new Dictionary<string, string>()) {}

            protected override ReadCalculation ParseCalculationElement(XElement calculationElement)
            {
                return new ReadCalculation(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value);
            }
        }

        private class ReadCalculation : IConfigurationItem
        {
            public ReadCalculation(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        private static CalculationGroup GetExpectedNestedData()
        {
            return new CalculationGroup
            {
                Name = "Root",
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "Group 1",
                        Children =
                        {
                            new TestCalculation
                            {
                                Name = "Calculation 3"
                            }
                        }
                    },
                    new TestCalculation
                    {
                        Name = "Calculation 1"
                    },
                    new CalculationGroup
                    {
                        Name = "Group 2",
                        Children =
                        {
                            new CalculationGroup
                            {
                                Name = "Group 4",
                                Children =
                                {
                                    new TestCalculation
                                    {
                                        Name = "Calculation 5"
                                    }
                                }
                            },
                            new TestCalculation
                            {
                                Name = "Calculation 4"
                            }
                        }
                    },
                    new TestCalculation
                    {
                        Name = "Calculation 2"
                    },
                    new CalculationGroup
                    {
                        Name = "Group 3"
                    }
                }
            };
        }

        private static void AssertCalculationGroup(CalculationGroup expectedCalculationGroup, CalculationGroup actualCalculationGroup)
        {
            Assert.AreEqual(expectedCalculationGroup.Children.Count, actualCalculationGroup.Children.Count);

            for (var i = 0; i < expectedCalculationGroup.Children.Count; i++)
            {
                Assert.AreEqual(expectedCalculationGroup.Children[i].Name, actualCalculationGroup.Children[i].Name);
                var innerCalculationgroup = expectedCalculationGroup.Children[i] as CalculationGroup;
                var innerCalculation = expectedCalculationGroup.Children[i] as TestCalculation;

                if (innerCalculationgroup != null)
                {
                    AssertCalculationGroup(innerCalculationgroup, (CalculationGroup) actualCalculationGroup.Children[i]);
                }

                if (innerCalculation != null)
                {
                    Assert.AreEqual(innerCalculation.Name, ((TestCalculation) actualCalculationGroup.Children[i]).Name);
                }
            }
        }

        private class TestInputWithForeshoreProfileAndBreakWater : Observable, IUseBreakWater, IUseForeshore
        {
            public TestInputWithForeshoreProfileAndBreakWater(BreakWater breakWater)
            {
                BreakWater = breakWater;
            }

            public bool UseBreakWater { get; set; }
            public BreakWater BreakWater { get; }
            public bool UseForeshore { get; set; }

            public RoundedPoint2DCollection ForeshoreGeometry
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}