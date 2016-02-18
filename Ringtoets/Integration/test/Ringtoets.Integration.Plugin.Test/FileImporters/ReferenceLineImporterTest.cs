using System;
using System.IO;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;

using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ReferenceLineImporterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var importer = new ReferenceLineImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
            Assert.AreEqual("Referentielijn", importer.Name);
            Assert.AreEqual("Algemeen", importer.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.ReferenceLineIcon, importer.Image);
            Assert.AreEqual(typeof(ReferenceLineContext), importer.SupportedItemType);
            Assert.AreEqual("Referentielijn shapefile (*.shp)|*.shp", importer.FileFilter);
            Assert.IsNull(importer.ProgressChanged);
        }

        [Test]
        public void Import_ContextWithoutReferenceLineAndFileProperFile_ImportReferenceLineToAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();

            // Call
            bool importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            Assert.IsTrue(importSuccesful);
            Assert.IsInstanceOf<ReferenceLine>(assessmentSection.ReferenceLine);
            Assert.AreSame(assessmentSection.ReferenceLine, referenceLineContext.WrappedData);
            Point2D[] point2Ds = assessmentSection.ReferenceLine.Points.ToArray();
            Assert.AreEqual(803, point2Ds.Length);
            Assert.AreEqual(193515.719, point2Ds[467].X, 1e-6);
            Assert.AreEqual(511444.750, point2Ds[467].Y, 1e-6);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.", path) + Environment.NewLine +
                                  "Er is geen referentielijn geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ShapefileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", path) + Environment.NewLine +
                                   "Er is geen referentielijn geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
            mocks.VerifyAll();
        }

        // TODO: Cancel
        // TODO: Import when ReferenceLine already exists
        // TODO: Progress reporting
    }
}