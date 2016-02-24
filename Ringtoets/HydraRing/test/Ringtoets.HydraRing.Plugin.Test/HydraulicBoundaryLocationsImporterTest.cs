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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
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

        private HydraulicBoundaryLocationsImporter importer;

        [SetUp]
        public void SetUp()
        {
            progress = 0;

            importer = new HydraulicBoundaryLocationsImporter();
        }

        [TearDown]
        public void TearDown()
        {
            importer.Dispose();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var expectedFileFilter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsHydraRingFormsResources.SelectHydraulicBoundaryDatabaseFile_FilterName);
            var expectedDisplayName = "Locaties van de hydraulische randvoorwaarden";

            // Call is done in SetUp

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
        public void ValidateAndConnectTo_ExistingFile_DoesNotThrowException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            TestDelegate test = () => importer.ValidateAndConnectTo(validFilePath);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void ValidateAndConnectTo_NonExistingFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "nonexisting.sqlite");
            var expectedExceptionMessage = String.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", filePath);

            // Call
            TestDelegate test = () => importer.ValidateAndConnectTo(filePath);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedExceptionMessage, exception.Message);
        }

        [Test]
        public void ValidateAndConnectTo_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");
            var expectedExceptionMessage = String.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.", filePath);

            // Call
            TestDelegate test = () => importer.ValidateAndConnectTo(filePath);

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
            importer.ValidateAndConnectTo(validFilePath);

            // Call
            string version = importer.GetHydraulicBoundaryDatabaseVersion();

            // Assert
            Assert.IsNotNullOrEmpty(version);
        }

        [Test]
        public void Import_ConnectionNotOpened_ThrowsInValidOperationException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

            var expectedMessage = "Er is nog geen bestand geopend.";

            // Call
            TestDelegate call = () => importer.Import(context, validFilePath);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportHydraulicBoundaryLocationsToCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var importTarget = new HydraulicBoundaryDatabaseContext(assessmentSection);

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Precondition
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            importer.ValidateAndConnectTo(validFilePath);

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
            ICollection<HydraulicBoundaryLocation> importedLocations = importTarget.Parent.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(18, importedLocations.Count);
            CollectionAssert.AllItemsAreNotNull(importedLocations);
            CollectionAssert.AllItemsAreUnique(importedLocations);
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var importTarget = new HydraulicBoundaryDatabaseContext(assessmentSection);
            importTarget.Attach(observer);

            importer.ProgressChanged = IncrementProgress;

            // Precondition
            Assert.IsNull(importTarget.Parent.HydraulicBoundaryDatabase);
            Assert.IsTrue(File.Exists(validFilePath), string.Format("Precodition failed. File does not exist: {0}", validFilePath));

            importer.ValidateAndConnectTo(validFilePath);

            importer.Cancel();
            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(importTarget, validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Het importeren van hydraulische randvoorwaarden locaties is afgebroken. Er is geen data ingelezen.", 1);
            Assert.IsFalse(importResult);
            Assert.IsNull(importTarget.Parent.HydraulicBoundaryDatabase);
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var importTarget = new HydraulicBoundaryDatabaseContext(assessmentSection);
            importTarget.Attach(observer);

            importer.ProgressChanged = IncrementProgress;

            // Precondition
            Assert.IsNull(importTarget.Parent.HydraulicBoundaryDatabase);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.ValidateAndConnectTo(validFilePath);

            // Setup (second part)
            importer.Cancel();
            var importResult = importer.Import(importTarget, validFilePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = importer.Import(importTarget, validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.IsNotNull(importTarget.Parent.HydraulicBoundaryDatabase);
            Assert.AreEqual(18, importTarget.Parent.HydraulicBoundaryDatabase.Locations.Count);
            CollectionAssert.AllItemsAreNotNull(importTarget.Parent.HydraulicBoundaryDatabase.Locations);
            CollectionAssert.AllItemsAreUnique(importTarget.Parent.HydraulicBoundaryDatabase.Locations);
        }

        [Test]
        public void Import_ImportingFileWithCorruptSchema_AbortAndLog()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var importTarget = new HydraulicBoundaryDatabaseContext(assessmentSection);

            string corruptPath = Path.Combine(testDataPath, "corruptschema.sqlite");
            var expectedLogMessage = string.Format("Fout bij het lezen van bestand '{0}': Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database. Het bestand wordt overgeslagen.", corruptPath);

            importer.ProgressChanged = IncrementProgress;

            importTarget.Attach(observer);

            var importResult = true;

            importer.ValidateAndConnectTo(corruptPath);

            // Call
            Action call = () => importResult = importer.Import(importTarget, corruptPath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            Assert.IsNull(importTarget.Parent.HydraulicBoundaryDatabase, "No HydraulicBoundaryDatabase object should be created when import from corrupt database.");
            Assert.AreEqual(1, progress);

            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }
    }
}