using System;
using System.Collections.Generic;

using DelftTools.Shell.Core;

using NUnit.Framework;

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
            // call
            var importer = new PipingSurfaceLinesCsvImporter();

            // assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual(WtiFormsResources.PipingSurfaceLinesCollectionName, importer.Name);
            Assert.AreEqual(ApplicationResources.WtiApplicationName, importer.Category);
            Assert.AreEqual(16, importer.Image.Width);
            Assert.AreEqual(16, importer.Image.Height);
            CollectionAssert.AreEqual(new[]{typeof(IEnumerable<PipingSurfaceLine>)}, importer.SupportedItemTypes);
            Assert.IsFalse(importer.CanImportOnRootLevel);
            var expectedFileFilter = String.Format("{0} {1}(*.csv)|*.csv", 
                WtiFormsResources.PipingSurfaceLinesCollectionName, ApplicationResources.CsvFileName);
            Assert.AreEqual(expectedFileFilter, importer.FileFilter);
            Assert.IsNull(importer.TargetDataDirectory);
            Assert.IsFalse(importer.ShouldCancel);
            Assert.IsNull(importer.ProgressChanged);
            Assert.IsFalse(importer.OpenViewAfterImport);
        }
    }
}