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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
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
            // Prepare
            var expectedFileFilter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsHydraRingFormsResources.SelectDatabaseFile_FilterName);

            // Call
            var importer = new HydraulicBoundaryLocationsImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(RingtoetsHydraRingFormsResources.HydraulicBoundaryLocationsCollection_DisplayName, importer.Name);
            Assert.AreEqual(RingtoetsFormsResources.Ringtoets_Category, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            Assert.AreEqual(typeof(HydraulicBoundaryLocation), importer.SupportedItemType);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
            Assert.IsNull(importer.ProgressChanged);
            Assert.IsNull(importer.Version);
        }

        [Test]
        [TestCase("/")]
        [TestCase("nonexisting.sqlit")]
        public void ValidateFile_NonExistingFileOrInvalidFile_LogError(string filename)
        {
            // Setup
            string filePath = Path.Combine(testDataPath, filename);
            var importer = new HydraulicBoundaryLocationsImporter();
            var expectedMessage = string.Format(RingtoetsHydraRingPluginResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, String.Empty);

            // Call
            Action call = () => importer.ValidateFile(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                StringAssert.EndsWith(expectedMessage, messageArray[0]);
            });
        }

        [Test]
        public void ValidateFile_ValidFile_GetDatabaseVersion()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var importer = new HydraulicBoundaryLocationsImporter();

            // Call
            importer.ValidateFile(validFilePath);

            // Assert
            Assert.IsNotNullOrEmpty(importer.Version);
        }

        [Test]
        [TestCase("/")]
        [TestCase("nonexisting.sqlite")]
        public void Import_FromNonExistingFileOrInvalidFile_LogError(string filename)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, filename);
            var expectedMessage = string.Format(RingtoetsHydraRingPluginResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, String.Empty);

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
            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(observableList, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                StringAssert.EndsWith(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
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
        public void Import_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.sqlite");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

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

            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(RingtoetsHydraRingIOResources.Error_HydraulicBoundaryLocation_read_from_database);
            var expectedLogMessage = string.Format(RingtoetsHydraRingPluginResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);

            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(observableHydraulicBoundaryLocationList, "No items should be added to collection when importin an empty database.");
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void Import_ImportingFileWithCorruptSchema_AbortAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

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
            var expectedLogMessage = string.Format(RingtoetsHydraRingPluginResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, corruptPath);

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