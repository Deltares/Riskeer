using System;
using System.IO;
using System.Linq;

using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;

using NUnit.Extensions.Forms;
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
    public class ReferenceLineImporterTest : NUnitFormsAssertTest
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
            Assert.IsNull(assessmentSection.ReferenceLine);
            Assert.IsNull(referenceLineContext.WrappedData);
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
            Assert.IsNull(assessmentSection.ReferenceLine);
            Assert.IsNull(referenceLineContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToCancel_NoChanges()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.StrictMock<ICalculationItem>();
            var calculation3 = mocks.StrictMock<ICalculationItem>();

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation3,
            });

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();
            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);

                messageBoxTitle = messageBoxTester.Title;
                messageBoxText = messageBoxTester.Text;

                messageBoxTester.ClickCancel();
            };

            // Call
            bool importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            Assert.IsFalse(importSuccesful);
            Assert.AreSame(originalReferenceLine, assessmentSection.ReferenceLine);
            Assert.AreSame(assessmentSection.ReferenceLine, referenceLineContext.WrappedData);

            Assert.AreEqual("Referentielijn vervangen?", messageBoxTitle);
            var expectedText = "Weet u zeker dat u de referentielijn wilt vervangen?" + Environment.NewLine +
                               "Als u door gaat zullen alle vakindelingen, berekende hydrolische randvoorwaarden en berekeningsresultaten worden verwijderd.";
            Assert.AreEqual(expectedText, messageBoxText);

            mocks.VerifyAll();
        }

        [Test]
        public void Import_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToContinue_ClearDataDependentOnReferenceLine()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.Stub<ICalculationItem>();
            calculation1.Expect(c => c.ClearOutput());
            var calculation2 = mocks.Stub<ICalculationItem>();
            calculation2.Expect(c => c.ClearOutput());
            var calculation3 = mocks.Stub<ICalculationItem>();
            calculation3.Expect(c => c.ClearOutput());
            var calculation4 = mocks.Stub<ICalculationItem>();
            calculation4.Expect(c => c.ClearOutput());

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation3,
                calculation4
            });

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);

                messageBoxTitle = messageBoxTester.Title;
                messageBoxText = messageBoxTester.Text;

                messageBoxTester.ClickOk();
            };

            // Call
            bool importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            Assert.IsTrue(importSuccesful);
            Assert.AreNotSame(originalReferenceLine, assessmentSection.ReferenceLine);
            Point2D[] point2Ds = assessmentSection.ReferenceLine.Points.ToArray();
            Assert.AreEqual(803, point2Ds.Length);
            Assert.AreEqual(198237.375, point2Ds[123].X, 1e-6);
            Assert.AreEqual(514879.781, point2Ds[123].Y, 1e-6);

            Assert.AreEqual("Referentielijn vervangen?", messageBoxTitle);
            var expectedText = "Weet u zeker dat u de referentielijn wilt vervangen?" + Environment.NewLine +
                                   "Als u door gaat zullen alle vakindelingen, berekende hydrolische randvoorwaarden en berekeningsresultaten worden verwijderd.";
            Assert.AreEqual(expectedText, messageBoxText);

            // TODO: Clear 'vakindelingen' on all failure mechanisms
            // TODO: Clear calculated HR
            mocks.VerifyAll(); // Expect calculation output cleared
        }

        [Test]
        public void DoPostImportUpdates_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextAndClearedObjects()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.Stub<ICalculationItem>();
            calculation1.Stub(c => c.ClearOutput());
            calculation1.Expect(c => c.NotifyObservers());
            var calculation2 = mocks.Stub<ICalculationItem>();
            calculation2.Stub(c => c.ClearOutput());
            calculation2.Expect(c => c.NotifyObservers());
            var calculation3 = mocks.Stub<ICalculationItem>();
            calculation3.Stub(c => c.ClearOutput());
            calculation3.Expect(c => c.NotifyObservers());
            var calculation4 = mocks.Stub<ICalculationItem>();
            calculation4.Stub(c => c.ClearOutput());
            calculation4.Expect(c => c.NotifyObservers());

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation3,
                calculation4
            });

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });

            var contextObserver = mocks.Stub<IObserver>();
            contextObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Precondition
            Assert.IsTrue(importer.Import(referenceLineContext, path));

            // Call
            importer.DoPostImportUpdates(referenceLineContext);

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations and context
        }

        [Test]
        public void Import_CancellingImport_ReturnFalseAndNoChanges()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();

            DialogBoxHandler = (name, wnd) =>
            {
                importer.Cancel();

                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Call
            bool importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            Assert.IsFalse(importSuccesful);
            Assert.AreSame(originalReferenceLine, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostImportUpdates_CancellingImport_DoNotNotifyObservers()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.StrictMock<ICalculationItem>();

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1
            });

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1
            });

            var contextObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter();

            DialogBoxHandler = (name, wnd) =>
            {
                importer.Cancel();

                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Precondition
            Assert.IsFalse(importer.Import(referenceLineContext, path));

            // Call
            importer.DoPostImportUpdates(referenceLineContext);

            // Assert
            mocks.VerifyAll(); // Expect no NotifyObserver calls
        }

        // TODO: Progress reporting
        // TODO: Instance reuse
    }
}