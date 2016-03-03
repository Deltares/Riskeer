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
            Assert.IsInstanceOf<FileImporterBase<ReferenceLineContext>>(importer);
            Assert.AreEqual("Referentielijn", importer.Name);
            Assert.AreEqual("Algemeen", importer.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.ReferenceLineIcon, importer.Image);
            Assert.AreEqual("Referentielijn shapefile (*.shp)|*.shp", importer.FileFilter);
        }

        [Test]
        public void Import_ContextWithoutReferenceLine_ImportReferenceLineToAssessmentSection()
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
        public void Import_ContextWithoutReferenceLine_GeneratedExpectedProgressMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification{ Text = "Inlezen referentielijn.", CurrentStep = 1, MaxNrOfSteps = 2 },
                new ExpectedProgressNotification{ Text = "Geïmporteerde data toevoegen aan het traject.", CurrentStep = 2, MaxNrOfSteps = 2 },
            };
            var progressChangedCallCount = 0;
            var importer = new ReferenceLineImporter
            {
                ProgressChanged = (description, step, steps) =>
                {
                    Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                    Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                    Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].MaxNrOfSteps, steps);
                    progressChangedCallCount++;
                }
            };

            // Call
            importer.Import(referenceLineContext, path);

            // Assert
            Assert.AreEqual(expectedProgressMessages.Length, progressChangedCallCount);
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

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            var expectedText = "Als u de referentielijn vervangt zullen alle vakindelingen, berekende hydraulische randvoorwaarden en berekeningsresultaten worden verwijderd." + Environment.NewLine +
                               Environment.NewLine + "Weet u zeker dat u wilt doorgaan?";
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
            failureMechanism1.Expect(fm => fm.ClearAllSections());
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Expect(fm => fm.ClearAllSections());
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

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            var expectedText = "Als u de referentielijn vervangt zullen alle vakindelingen, berekende hydraulische randvoorwaarden en berekeningsresultaten worden verwijderd." + Environment.NewLine +
                               Environment.NewLine + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedText, messageBoxText);

            // TODO: Clear calculated HR
            mocks.VerifyAll(); // Expect calculation output cleared
        }

        [Test]
        public void Import_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToContinue_GenerateExpectedProgressMessages()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.Stub<ICalculationItem>();
            var calculation2 = mocks.Stub<ICalculationItem>();

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Expect(fm => fm.ClearAllSections());
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
                calculation2
            });

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1
            });
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification { Text = "Inlezen referentielijn.", CurrentStep = 1, MaxNrOfSteps = 4 }, 
                new ExpectedProgressNotification { Text = "Geïmporteerde data toevoegen aan het traject.", CurrentStep = 2, MaxNrOfSteps = 4 }, 
                new ExpectedProgressNotification { Text = "Wissen rekenresultaten en vakindelingen van faalmechanismen.", CurrentStep = 3, MaxNrOfSteps = 4 }, 
                new ExpectedProgressNotification { Text = "Verwijderen uitvoer van hydraulische randvoorwaarden.", CurrentStep = 4, MaxNrOfSteps = 4 }, 
            };
            var progressChangedCallCount = 0;
            var importer = new ReferenceLineImporter();
            importer.ProgressChanged = (description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].MaxNrOfSteps, steps);
                progressChangedCallCount++;
            };

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Call
            importer.Import(referenceLineContext, path);

            // Assert
            Assert.AreEqual(expectedProgressMessages.Length, progressChangedCallCount);
            mocks.VerifyAll();
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
        [TestCase(true)]
        [TestCase(false)]
        public void Import_CancelImportDuringDialogInteraction_GenerateCancelledLogMessage(bool acceptRemovalOfReferenceLineDependentData)
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
                if (acceptRemovalOfReferenceLineDependentData)
                {
                    messageBoxTester.ClickOk();
                }
                else
                {
                    messageBoxTester.ClickCancel();
                }
            };

            // Call
            Action call = () => importer.Import(referenceLineContext, path);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Referentielijn importeren afgebroken. Geen data ingelezen.", 1);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ReusingCancelledImporterForContextWithoutReferenceLine_ImportReferenceLineToAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var importer = new ReferenceLineImporter();
            importer.Cancel();

            // Call
            bool importSuccesful = importer.Import(referenceLineContext, path);

            // Assert
            Assert.IsTrue(importSuccesful);
            Assert.IsInstanceOf<ReferenceLine>(assessmentSection.ReferenceLine);
            Assert.AreSame(assessmentSection.ReferenceLine, referenceLineContext.WrappedData);
            Point2D[] point2Ds = assessmentSection.ReferenceLine.Points.ToArray();
            Assert.AreEqual(803, point2Ds.Length);
            Assert.AreEqual(195203.563, point2Ds[321].X, 1e-6);
            Assert.AreEqual(512826.406, point2Ds[321].Y, 1e-6);
            mocks.VerifyAll();
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
            failureMechanism1.Expect(fm => fm.ClearAllSections());
            failureMechanism1.Expect(fm => fm.NotifyObservers());
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Expect(fm => fm.ClearAllSections());
            failureMechanism2.Expect(fm => fm.NotifyObservers());
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
        public void DoPostImportUpdates_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextParent()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.ReferenceLine = originalReferenceLine;

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Parent.Attach(observer);

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
            mocks.VerifyAll(); // Expect NotifyObservers on context parent
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

        [Test]
        public void DoPostImportUpdates_ReuseImporterWithAssessmentSectionWithReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextAndClearedObjects()
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
            failureMechanism1.Expect(fm => fm.ClearAllSections()).Repeat.Twice();
            failureMechanism1.Expect(fm => fm.NotifyObservers());
            failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Expect(fm => fm.ClearAllSections()).Repeat.Twice();
            failureMechanism2.Expect(fm => fm.NotifyObservers());
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
            importer.Cancel();
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };
            Assert.IsTrue(importer.Import(referenceLineContext, path));

            // Call
            importer.DoPostImportUpdates(referenceLineContext);

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations and context
        }

        private class ExpectedProgressNotification
        {
            public string Text { get; set; }
            public int CurrentStep { get; set; }
            public int MaxNrOfSteps { get; set; }
        }
    }
}