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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsHydraRingFormsResources = Ringtoets.HydraRing.Forms.Properties.Resources;
using RingtoetsHydraRingPluginResources = Ringtoets.HydraRing.Plugin.Properties.Resources;
using RingtoetsHydraRingIOResources = Ringtoets.HydraRing.IO.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HydraRing.Plugin.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationsImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");
        private int progress;

        [SetUp]
        public void SetUp()
        {
            progress = 0;
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var expectedFileFilter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsHydraRingFormsResources.SelectHydraulicBoundaryDatabaseFile_FilterName);
            var expectedDisplayName = "Locaties van de hydraulische randvoorwaarden";

            // Call
            var importer = new HydraulicBoundaryLocationsImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(expectedDisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            Assert.AreEqual(typeof(ICollection<HydraulicBoundaryLocation>), importer.SupportedItemType);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
        }

        [Test]
        public void GetHydraulicBoundaryDatabaseVersion_NonExistingFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "nonexisting.sqlite");
            var importer = new HydraulicBoundaryLocationsImporter();
            var expectedExceptionMessage = String.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", filePath);

            // Call
            TestDelegate test = () => importer.GetHydraulicBoundaryDatabaseVersion(filePath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void GetHydraulicBoundaryDatabaseVersion_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");
            var importer = new HydraulicBoundaryLocationsImporter();
            var expectedExceptionMessage = String.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.", filePath);

            // Call
            TestDelegate test = () => importer.GetHydraulicBoundaryDatabaseVersion(filePath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void GetHydraulicBoundaryDatabaseVersion_ValidFile_GetDatabaseVersion()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryLocationsImporter();

            // Call
            string version = importer.GetHydraulicBoundaryDatabaseVersion(validFilePath);

            // Assert
            Assert.IsNotNullOrEmpty(version);
        }

        [Test]
        [TestCase("/", "Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.")]
        [TestCase("nonexisting.sqlite", "Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.")]
        public void Import_FromNonExistingFileOrInvalidFile_ThrowsCriticalFileReadException(string filename, string exceptionMessage)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, filename);
            var expectedMessage = string.Format(exceptionMessage, validFilePath);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableList = new ObservableList<HydraulicBoundaryLocation>();
            observableList.Attach(observer);
            var importer = new HydraulicBoundaryLocationsImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(observableList);

            // Call
            TestDelegate test = () => importer.Import(observableList, validFilePath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            CollectionAssert.IsEmpty(observableList);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportHydraulicBoundaryLocationsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryLocationsImporter();
            var importTarget = new List<HydraulicBoundaryLocation>();
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import(importTarget, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                StringAssert.EndsWith(RingtoetsHydraRingPluginResources.HydraulicBoundaryLocationsImporter_Import_Import_successful, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            Assert.AreEqual(18, importTarget.Count);
            CollectionAssert.AllItemsAreNotNull(importTarget);
            CollectionAssert.AllItemsAreUnique(importTarget);
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableList = new ObservableList<HydraulicBoundaryLocation>();
            observableList.Attach(observer);

            var importer = new HydraulicBoundaryLocationsImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(observableList);
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            importer.Cancel();
            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableList, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Locaties van hydraulische randvoorwaarden importeren is afgebroken. Er is geen data ingelezen.", 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableList);
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_ImportHydraulicBoundaryLocationsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableList = new ObservableList<HydraulicBoundaryLocation>();
            observableList.Attach(observer);

            var importer = new HydraulicBoundaryLocationsImporter
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(observableList);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            importer.Cancel();
            var importResult = importer.Import(observableList, validFilePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = importer.Import(observableList, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(18, observableList.Count);
            CollectionAssert.AllItemsAreNotNull(observableList);
            CollectionAssert.AllItemsAreUnique(observableList);
        }

        [Test]
        public void Import_ImportingToValidTargetWithEmptyFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(testDataPath, "empty.sqlite");
            var expectedExceptionMessage = new FileReaderErrorMessageBuilder(corruptPath).
                Build(RingtoetsHydraRingIOResources.Error_HydraulicBoundaryLocation_read_from_database);

            var importer = new HydraulicBoundaryLocationsImporter
            {
                ProgressChanged = IncrementProgress
            };

            var observableHydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            observableHydraulicBoundaryLocationList.Attach(observer);

            // Call
            TestDelegate test = () => importer.Import(observableHydraulicBoundaryLocationList, corruptPath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            CollectionAssert.IsEmpty(observableHydraulicBoundaryLocationList, "No items should be added to collection when import in an empty database.");
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_ImportingFileWithCorruptSchema_AbortAndLog()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            string corruptPath = Path.Combine(testDataPath, "corruptschema.sqlite");
            var expectedLogMessage = string.Format("Fout bij het lezen van bestand '{0}': Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database. Het bestand wordt overgeslagen.", corruptPath);

            var importer = new HydraulicBoundaryLocationsImporter
            {
                ProgressChanged = IncrementProgress
            };

            var observableHydraulicBoundaryLocationList = new ObservableList<HydraulicBoundaryLocation>();
            observableHydraulicBoundaryLocationList.Attach(observer);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableHydraulicBoundaryLocationList, corruptPath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableHydraulicBoundaryLocationList, "No items should be added to collection when import from corrupt database.");
            Assert.AreEqual(2, progress);

            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }
    }
}