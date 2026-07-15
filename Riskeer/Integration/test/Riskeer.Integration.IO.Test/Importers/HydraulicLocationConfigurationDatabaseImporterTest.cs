// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Importers;
using Riskeer.Integration.TestUtil;

namespace Riskeer.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImporterTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO,
                                                                                 nameof(HydraulicLocationConfigurationDatabaseImporter));

        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(),
                                                                              null, new HydraulicBoundaryData(),
                                                                              validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();

            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(),
                                                                              handler, null, validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            // Call
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              new HydraulicBoundaryData(), validHlcdFilePath);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicLocationConfigurationDatabase>>(importer);
        }

        [Test]
        public void Import_CancelImportDuringDialogInteraction_GenerateCanceledLogMessageAndReturnsFalse()
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(false);
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);

            var importResult = true;

            // Call
            void Call() => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD bestand importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            handler.Received().InquireConfirmation();
        }

        [Test]
        public void Import_HlcdInDifferentDirectoryThanHydraulicBoundaryDatabases_CancelImportWithErrorMessage()
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            string hlcdFilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO,
                                                                          nameof(HydraulicBoundaryDatabaseImporter)), "HLCD.sqlite");

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het HLCD bestand moet zich in dezelfde map bevinden als de toegevoegde HRD bestanden.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
            handler.Received().InquireConfirmation();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            string hlcdFilePath = Path.Combine(testDataPath, "empty.sqlite");

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het bevragen van de database is mislukt.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
        }

        [Test]
        [TestCase("scenarioInformationNoEntries")]
        [TestCase("scenarioInformationMultipleEntries")]
        public void Import_InvalidNumberOfScenarioInformationEntries_CancelImportWithErrorMessage(string hlcdFileName)
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            string hlcdFilePath = Path.Combine(testDataPath, $"{hlcdFileName}.sqlite");

            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': de tabel 'ScenarioInformation' moet exact 1 rij bevatten.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
        }

        [Test]
        public void Import_ReadHlcdDoesNotContainCurrentLocationId_CancelImportWithErrorMessage()
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            hydraulicBoundaryData.HydraulicBoundaryDatabases.First().Locations.Add(new TestHydraulicBoundaryLocation());
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{validHlcdFilePath}': 1 of meerdere locaties komen niet voor in de HLCD.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
        }

        [Test]
        [TestCaseSource(nameof(GetProgressNotificationTestCases))]
        public void Import_ValidHlcdFile_ExpectedProgressNotifications(HydraulicBoundaryData hydraulicBoundaryData, IEnumerable<ProgressNotification> expectedProgressNotifications)
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.Update(Arg.Any<ReadHydraulicLocationConfigurationDatabase>(), Arg.Any<IDictionary<HydraulicBoundaryDatabase, long>>(), Arg.Any<string>()).Returns(Enumerable.Empty<IObservable>());
            var progressChangeNotifications = new List<ProgressNotification>();
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressNotifications, progressChangeNotifications);
        }

        [Test]
        public void Import_ValidFileWithoutScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.Update(Arg.Is<ReadHydraulicLocationConfigurationDatabase>(x => x != null),
                           Arg.Is<IDictionary<HydraulicBoundaryDatabase, long>>(x =>
                                                                                    x == new Dictionary<HydraulicBoundaryDatabase, long>
                                                                                    {
                                                                                        {
                                                                                            hydraulicBoundaryData.HydraulicBoundaryDatabases.First(), 13
                                                                                        }
                                                                                    }),
                           hlcdFilePath).Returns(Enumerable.Empty<IObservable>());
            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, hlcdFilePath);

            // Call
            var importResult = false;
            void Call() => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{hlcdFilePath}'.", 1);
            Assert.IsTrue(importResult);
            handler.Received().InquireConfirmation();
            handler.Received().Update(Arg.Is<ReadHydraulicLocationConfigurationDatabase>(x => x != null),
                                      Arg.Is<IDictionary<HydraulicBoundaryDatabase, long>>(x =>
                                                                                               x == new Dictionary<HydraulicBoundaryDatabase, long>
                                                                                               {
                                                                                                   {
                                                                                                       hydraulicBoundaryData.HydraulicBoundaryDatabases.First(), 13
                                                                                                   }
                                                                                               }),
                                      hlcdFilePath);
        }

        [Test]
        public void Import_ValidFileWithScenarioInformation_UpdatesHydraulicBoundaryDataWithImportedData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            string filePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation.sqlite");
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.Update(Arg.Is<ReadHydraulicLocationConfigurationDatabase>(x => x != null),
                           Arg.Is<IDictionary<HydraulicBoundaryDatabase, long>>(x =>
                                                                                    x == new Dictionary<HydraulicBoundaryDatabase, long>
                                                                                    {
                                                                                        {
                                                                                            hydraulicBoundaryData.HydraulicBoundaryDatabases.First(), 13
                                                                                        }
                                                                                    }),
                           filePath).Returns(Enumerable.Empty<IObservable>());
            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, filePath);

            // Call
            var importResult = false;
            void Call() => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(importResult);
            handler.Received().InquireConfirmation();
            handler.Received().Update(Arg.Is<ReadHydraulicLocationConfigurationDatabase>(x => x != null),
                                      Arg.Is<IDictionary<HydraulicBoundaryDatabase, long>>(x =>
                                                                                               x == new Dictionary<HydraulicBoundaryDatabase, long>
                                                                                               {
                                                                                                   {
                                                                                                       hydraulicBoundaryData.HydraulicBoundaryDatabases.First(), 13
                                                                                                   }
                                                                                               }),
                                      filePath);
        }

        [Test]
        public void Import_InvalidHydraulicBoundaryDatabase_CancelImportAndLogs()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryData(assessmentSection, validHlcdFilePath, validHrdFilePath);
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            string invalidHrdFilePath = Path.Combine(testDataPath, "doesNotExist.sqlite");
            hydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                FilePath = invalidHrdFilePath
            });

            string filePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation.sqlite");
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, filePath);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{invalidHrdFilePath}': het bestand bestaat niet.";
            AssertImportFailed(Call, expectedMessage, ref importSuccessful);
            handler.Received().InquireConfirmation();
        }

        [Test]
        public void DoPostImportUpdates_WhenImportSuccessful_NotifyObserversOfReturnedObjects()
        {
            // Setup
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();
            var observable1 = Substitute.For<IObservable>();
            var observable2 = Substitute.For<IObservable>();

            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.Update(Arg.Is<ReadHydraulicLocationConfigurationDatabase>(x => x != null),
                           Arg.Is<IDictionary<HydraulicBoundaryDatabase, long>>(x => x != null),
                           Arg.Is<string>(x => x != null)).Returns(new[]
            {
                observable1,
                observable2
            });
            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase, handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            observable1.Received().NotifyObservers();
            observable2.Received().NotifyObservers();
            handler.Received().InquireConfirmation();
            handler.Received().Update(Arg.Is<ReadHydraulicLocationConfigurationDatabase>(x => x != null),
                                      Arg.Is<IDictionary<HydraulicBoundaryDatabase, long>>(x => x != null),
                                      Arg.Is<string>(x => x != null));
        }

        [Test]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs()
        {
            // Setup
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                importer.Cancel();
            });

            // Call
            var importResult = true;
            void Call() => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD bestand importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelImportDuringAddReadDataToDataModel_ContinuesImportAndLogs()
        {
            // Setup
            const int totalNumberOfSteps = 3;
            var handler = Substitute.For<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.Update(Arg.Any<ReadHydraulicLocationConfigurationDatabase>(), Arg.Any<IDictionary<HydraulicBoundaryDatabase, long>>(), Arg.Any<string>()).Returns(Enumerable.Empty<IObservable>());
            HydraulicBoundaryData hydraulicBoundaryData = CreateLinkedHydraulicBoundaryData();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationDatabase(), handler,
                                                                              hydraulicBoundaryData, validHlcdFilePath);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (step == totalNumberOfSteps)
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            importer.Import();
            void Call() => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 2);
            Assert.IsTrue(importResult);
        }

        private static IEnumerable<TestCaseData> GetProgressNotificationTestCases()
        {
            yield return new TestCaseData(
                new HydraulicBoundaryData(), new[]
                {
                    new ProgressNotification("Inlezen van het hydraulische locatie configuratie bestand.", 1, 2),
                    new ProgressNotification("Geïmporteerde data toevoegen aan het traject.", 2, 2)
                });
            yield return new TestCaseData(
                CreateLinkedHydraulicBoundaryData(), new[]
                {
                    new ProgressNotification("Inlezen van het hydraulische locatie configuratie bestand.", 1, 3),
                    new ProgressNotification("Inlezen van de hydraulische belastingen bestanden.", 2, 3),
                    new ProgressNotification("Geïmporteerde data toevoegen aan het traject.", 3, 3)
                });
        }

        private static HydraulicBoundaryData CreateLinkedHydraulicBoundaryData()
        {
            return new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath,
                    ScenarioName = "ScenarioName",
                    Year = 2022,
                    Scope = "Scope",
                    SeaLevel = "SeaLevel",
                    RiverDischarge = "RiverDischarge",
                    LakeLevel = "LakeLevel",
                    WindDirection = "WindDirection",
                    WindSpeed = "WindSpeed",
                    Comment = "Comment"
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        UsePreprocessorClosure = false
                    }
                }
            };
        }

        private static void AssertImportFailed(Action call, string errorMessage, ref bool importSuccessful)
        {
            string expectedMessage = $"{errorMessage}" +
                                     $"{Environment.NewLine}Er is geen HLCD geïmporteerd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importSuccessful);
        }
    }
}