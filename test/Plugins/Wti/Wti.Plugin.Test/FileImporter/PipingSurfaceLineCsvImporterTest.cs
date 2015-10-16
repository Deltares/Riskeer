using System;
using System.Collections.Generic;

using DelftTools.Shell.Core;

using NUnit.Framework;

using Rhino.Mocks;

using Wti.Data;
using Wti.Plugin.FileImporter;
using WtiFormsResources = Wti.Forms.Properties.Resources;
using ApplicationResources = Wti.Plugin.Properties.Resources;

namespace Wti.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLineCsvImporterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var importer = new PipingSurfaceLinesCsvImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(WtiFormsResources.PipingSurfaceLinesCollectionName, importer.Name);
            Assert.AreEqual(ApplicationResources.WtiApplicationName, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            CollectionAssert.AreEqual(new[]{typeof(IEnumerable<PipingSurfaceLine>)}, importer.SupportedItemTypes);
            Assert.IsFalse(importer.CanImportOnRootLevel);
            var expectedFileFilter = String.Format("{0} {1} (*.csv)|*.csv", 
                WtiFormsResources.PipingSurfaceLinesCollectionName, ApplicationResources.CsvFileName);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
            Assert.IsNull(importer.TargetDataDirectory);
            Assert.IsFalse(importer.ShouldCancel);
            Assert.IsNull(importer.ProgressChanged);
            Assert.IsFalse(importer.OpenViewAfterImport);
        }

        [Test]
        public void CanImportOn_TargetIsCollectionOfPipingSurfaceLines_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var validTarget = mocks.StrictMock<ICollection<PipingSurfaceLine>>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

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
            var invalidTarget = mocks.StrictMock<IEnumerable<PipingSurfaceLine>>();
            mocks.ReplayAll();

            var importer = new PipingSurfaceLinesCsvImporter();

            // Call
            var importAllowed = importer.CanImportOn(invalidTarget);

            // Assert
            Assert.IsFalse(importAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void ImportItem_ImportingToValidTargetWithValidFile_ImportSurfaceLinesToCollection()
        {
            // Setup
            const string validFilePath = "";
            var piping = new PipingFailureMechanism();

            var importer = new PipingSurfaceLinesCsvImporter();

            var importTarget = piping.SurfaceLines;

            // Precondition
            Assert.IsTrue(importer.CanImportOn(importTarget));

            // Call
            var importedItem = importer.ImportItem(validFilePath, importTarget);

            // Assert
            Assert.AreSame(importTarget, importedItem);
        }
    }
}