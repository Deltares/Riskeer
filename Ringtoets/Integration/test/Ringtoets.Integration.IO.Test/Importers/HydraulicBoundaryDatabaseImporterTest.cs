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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.IO.Importers;

namespace Ringtoets.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO, nameof(HydraulicBoundaryDatabaseImporter));

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var updateHandler = mocks.Stub<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), updateHandler, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicBoundaryDatabase>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{path}': bestandspad mag niet verwijzen naar een lege bestandsnaam."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO, "I_dont_exist");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{path}': het bestand bestaat niet."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "corruptschema.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "empty.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kon geen locaties verkrijgen van de database."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutHlcd_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "withoutHLCD", "complete.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithEmptyHlcdSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "EmptyHLCDSchema", "complete.sqlite");
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(path), "hlcd.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': het bevragen van de database is mislukt."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithInvalidHlcdSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "InvalidHLCDSchema", "complete.sqlite");
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(path), "hlcd.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hlcdFilePath}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ExistingFileWithoutSettings_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "withoutSettings", "complete.sqlite");

            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kon het rekeninstellingen bestand niet openen. " +
                                     $"Fout bij het lezen van bestand '{HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(path)}': het bestand bestaat niet."
                                     + $"{Environment.NewLine}Er is geen hydraulische belastingen database gekoppeld.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_WhenSuccessful_UpdatesHydraulicBoundaryDatabaseWithImportedData()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            handler.Expect(h => h.Update(Arg<HydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicBoundaryDatabase>.Is.NotNull,
                                         Arg<ReadHydraulicLocationConfigurationDatabase>.Is.NotNull))
                   .WhenCalled(invocation =>
                   {
                       Assert.AreSame(hydraulicBoundaryDatabase, invocation.Arguments[0]);

                       var readHydraulicBoundaryDatabase = (ReadHydraulicBoundaryDatabase) invocation.Arguments[1];
                       Assert.AreEqual("Dutch coast South19-11-2015 12:0013", readHydraulicBoundaryDatabase.Version);
                       Assert.AreEqual((long) 13, readHydraulicBoundaryDatabase.TrackId);
                       Assert.AreEqual(18, readHydraulicBoundaryDatabase.Locations.Count());
                       ReadHydraulicBoundaryLocation location = readHydraulicBoundaryDatabase.Locations.First();
                       Assert.AreEqual(1, location.Id);
                       Assert.AreEqual("punt_flw_ 1", location.Name);
                       Assert.AreEqual(52697.5, location.CoordinateX);
                       Assert.AreEqual(427567.0, location.CoordinateY);

                       var readHydraulicLocationConfigurationDatabase = (ReadHydraulicLocationConfigurationDatabase) invocation.Arguments[2];
                       Assert.AreEqual(18, readHydraulicLocationConfigurationDatabase.LocationIds.Count);
                   })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryDatabaseImporter(hydraulicBoundaryDatabase, handler, filePath);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFiles_ExpectedProgressNotifications()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            var totalSteps = 0;

            string filePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, filePath);
            importer.SetProgressChanged((currentStepName, currentStep, totalNumberOfSteps) =>
            {
                totalSteps = totalNumberOfSteps;
                if (currentStep == 1)
                {
                    Assert.AreEqual("Inlezen van het hydraulische belastingen bestand.", currentStepName);
                }

                if (currentStep == 2)
                {
                    Assert.AreEqual("Inlezen van het hydraulische locatie configuratie bestand.", currentStepName);
                }

                if (currentStep == 3)
                {
                    Assert.AreEqual("Controleren van het rekeninstellingen bestand.", currentStepName);
                }
            });

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(3, totalSteps);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Import_CancelOfImportWhilePerformingStep_CancelsImportAndLogs(int stepNumber)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicBoundaryDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryDatabaseImporter(new HydraulicBoundaryDatabase(), handler, filePath);
            importer.SetProgressChanged((description, currentStep, steps) =>
            {
                if (currentStep == stepNumber)
                {
                    importer.Cancel();
                }
            });

            // Precondition
            Assert.IsTrue(File.Exists(filePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create("Hydraulische belastingen database koppelen afgebroken. Geen gegevens gewijzigd.", LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }
    }
}