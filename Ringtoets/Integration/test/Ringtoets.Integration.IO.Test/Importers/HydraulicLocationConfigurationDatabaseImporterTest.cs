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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.TestUtil;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Importers;

namespace Ringtoets.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImporterTest
    {
        private const int totalNumberOfSteps = 3;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO,
                                                                                 nameof(HydraulicLocationConfigurationDatabaseImporter));

        private readonly string validHrdFilePath = Path.Combine(testDataPath, "completeHrd.sqlite");
        private readonly string validHlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), null,
                                                                                         new HydraulicBoundaryDatabase(), validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                                         null, validHlcdFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              new HydraulicBoundaryDatabase(), validHlcdFilePath);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicLocationConfigurationSettings>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_HlcdInDifferentDirectoryThanHydraulicBoundaryDatabase_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string hydraulicBoundaryDatabasePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO,
                                                                                           nameof(HydraulicBoundaryDatabaseImporter)), "complete.sqlite");

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(hydraulicBoundaryDatabasePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{validHlcdFilePath}': het HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_HrdInvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "CorruptHrd");

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(Path.Combine(path, "corruptschema.sqlite"));
            
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, Path.Combine(path, "HLCD.sqlite"));

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabase.FilePath}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_HrdEmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "EmptyHrd");

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(Path.Combine(path, "empty.sqlite"));

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, Path.Combine(path, "HLCD.sqlite"));

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabase.FilePath}': kon geen locaties verkrijgen van de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "empty.sqlite");

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bevragen van de database is mislukt.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "invalid.sqlite");

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("scenarioInformationNoEntries")]
        [TestCase("scenarioInformationMultipleEntries")]
        public void Import_InvalidNumberOfScenarioInformationEntries_CancelImportWithErrorMessage(string fileName)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, $"{fileName}.sqlite");

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': de tabel 'ScenarioInformation' moet exact 1 rij bevatten.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_LocationIdNotInHlcd_CancelImportWithErrorMessage()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validHrdFilePath);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation());

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{validHlcdFilePath}': 1 of meerdere locaties komen niet voor in de HLCD.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFiles_ExpectedProgressNotifications()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            handler.Stub(h => h.Update(null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            var expectedProgressNotifications = new[]
            {
                new ProgressNotification("Inlezen van het hydraulische belastingen bestand.", 1, totalNumberOfSteps),
                new ProgressNotification("Inlezen van het hydraulische locatie configuratie bestand.", 2, totalNumberOfSteps),
                new ProgressNotification("Geïmporteerde data toevoegen aan het traject.", 3, totalNumberOfSteps)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressNotifications, progressChangeNotifications);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs(int stepNumber)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            mocks.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                if (currentStep == stepNumber)
                {
                    importer.Cancel();
                }
            });

            // Call
            var importResult = true;
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD bestand importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringAddReadDataToDataModel_ContinuesImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Stub(h => h.InquireConfirmation()).Return(true);
            handler.Stub(h => h.Update(null, null, null)).IgnoreArguments().Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);
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
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringDialogInteraction_GenerateCanceledLogMessageAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(false);
            mocks.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            const string expectedMessage = "HLCD bestand importeren afgebroken. Geen gegevens gewijzigd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Info), 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileWithoutScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validHrdFilePath);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            string filePath = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.Same(hydraulicBoundaryDatabase),
                                         Arg<ReadHydraulicLocationConfigurationDatabaseSettings>.Is.Null,
                                         Arg<string>.Is.Equal(filePath)))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, handler,
                                                                              hydraulicBoundaryDatabase, filePath);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileWithScenarioInformation_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, validHrdFilePath);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            string filePath = Path.Combine(testDataPath, "hlcdWithValidScenarioInformation.sqlite");

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.Same(hydraulicBoundaryDatabase),
                                         Arg<ReadHydraulicLocationConfigurationDatabaseSettings>.Is.NotNull,
                                         Arg<string>.Is.Equal(filePath)))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, handler,
                                                                              hydraulicBoundaryDatabase, filePath);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostImportUpdates_WhenImportSuccessful_NotifyObserversOfReturnedObjects()
        {
            // Setup
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = CreateHydraulicBoundaryDatabase(validHrdFilePath);

            var mocks = new MockRepository();
            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            handler.Expect(h => h.InquireConfirmation()).Return(true);
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationDatabaseSettings>.Is.Null,
                                         Arg<string>.Is.NotNull))
                   .Return(new[]
                   {
                       observable1,
                       observable2
                   });
            mocks.ReplayAll();

            var importer = new HydraulicLocationConfigurationDatabaseImporter(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings, handler,
                                                                              hydraulicBoundaryDatabase, validHlcdFilePath);

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on updated observables
        }

        private static HydraulicBoundaryDatabase CreateHydraulicBoundaryDatabase(string filePath)
        {
            return new HydraulicBoundaryDatabase
            {
                FilePath = filePath
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