using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.Base;
using Core.Common.TestUtils;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;
using WtiIOResources = Ringtoets.Piping.IO.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSoilProfilesImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Prepare
            var expectedFileFilter = String.Format("{0} {1} (*.soil)|*.soil",
                WtiFormsResources.PipingSoilProfilesCollection_DisplayName, ApplicationResources.Soil_file_name);

            // Call
            var importer = new PipingSoilProfilesImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(WtiFormsResources.PipingSoilProfilesCollection_DisplayName, importer.Name);
            Assert.AreEqual(ApplicationResources.Wti_application_name, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            CollectionAssert.AreEqual(new[] { typeof(IEnumerable<PipingSoilProfile>) }, importer.SupportedItemTypes);
            Assert.IsFalse(importer.CanImportOnRootLevel);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
            Assert.IsNull(importer.TargetDataDirectory);
            Assert.IsFalse(importer.ShouldCancel);
            Assert.IsNull(importer.ProgressChanged);
            Assert.IsFalse(importer.OpenViewAfterImport);
        }

        [Test]
        public void CanImportOn_TargetIsCollectionOfPipingSoilProfile_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var validTarget = mocks.StrictMock<ICollection<PipingSoilProfile>>();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            // Call
            var importAllowed = importer.CanImportOn(validTarget);

            // Assert
            Assert.IsTrue(importAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanImportOn_InvalidTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var invalidTarget = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            // Call
            var importAllowed = importer.CanImportOn(invalidTarget);

            // Assert
            Assert.IsFalse(importAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithValidFile_ImportSoilProfilesToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");
            var piping = new PipingFailureMechanism();

            var importer = new PipingSoilProfilesImporter();

            var importTarget = piping.SoilProfiles;

            // Precondition
            Assert.IsTrue(importer.CanImportOn(importTarget));

            // Call
            var importedItem = importer.ImportItem(validFilePath, importTarget);

            // Assert
            Assert.AreSame(importTarget, importedItem);
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            var observableSoilProfileList = new ObservableList<PipingSoilProfile>();
            observableSoilProfileList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSoilProfileList);

            // Assert
            var internalErrorMessage = string.Format(WtiIOResources.Error_SoilProfile_read_from_database,
                                                     Path.GetFileName(corruptPath));
            var expectedLogMessage = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorReading_0_Cause_1_,
                                                   corruptPath, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreSame(observableSoilProfileList, importedItem);
            CollectionAssert.IsEmpty(observableSoilProfileList,
                "No items should be added to collection when import is aborted.");
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithProfileContainingInvalidValue_SkipImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "invalid2dGeometry.soil");

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var importer = new PipingSoilProfilesImporter();

            var observableSoilProfileList = new ObservableList<PipingSoilProfile>();
            observableSoilProfileList.Attach(observer);

            object importedItem = null;

            // Call
            Action call = () => importedItem = importer.ImportItem(corruptPath, observableSoilProfileList);

            // Assert
            var internalErrorMessage = string.Format(WtiIOResources.PipingSoilProfileReader_CouldNotParseGeometryOfLayer_0_InProfile_1_,
                                                     1, "Profile");
            var expectedLogMessage = string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_File_0_Message_1_,
                                                   corruptPath, internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);

            Assert.AreSame(observableSoilProfileList, importedItem);
            Assert.AreEqual(1, observableSoilProfileList.Count);

            mocks.VerifyAll();
        }
    }
}