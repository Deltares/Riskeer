using System;
using System.Collections.Generic;
using Core.Common.BaseDelftTools;
using System.IO;
using Core.Common.TestUtils;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

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
                WtiFormsResources.PipingSoilProfilesCollectionName, ApplicationResources.SoilFileName);

            // Call
            var importer = new PipingSoilProfilesImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(WtiFormsResources.PipingSoilProfilesCollectionName, importer.Name);
            Assert.AreEqual(ApplicationResources.WtiApplicationName, importer.Category);
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
        public void ImportItem_ImportingToInvalidTargetWithValidFile_ImportSoilProfilesToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "empty.soil");
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
    }
}