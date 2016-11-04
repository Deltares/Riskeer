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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.ReferenceLines;

namespace Ringtoets.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLineImporterTest : NUnitFormsAssertTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReferenceLineImporter(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var importer = new ReferenceLineImporter(assessmentSection, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<IAssessmentSection>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ContextWithoutReferenceLine_ImportReferenceLineToAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importer = new ReferenceLineImporter(assessmentSection, path);

            // Call
            bool importSuccesful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccesful);
            Assert.IsNotNull(assessmentSection.ReferenceLine);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification
                {
                    Text = "Inlezen referentielijn.", CurrentStep = 1, MaxNrOfSteps = 2
                },
                new ExpectedProgressNotification
                {
                    Text = "Geïmporteerde data toevoegen aan het traject.", CurrentStep = 2, MaxNrOfSteps = 2
                }
            };
            var progressChangedCallCount = 0;
            var importer = new ReferenceLineImporter(assessmentSection, path);
            importer.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].MaxNrOfSteps, steps);
                progressChangedCallCount++;
            });

            // Call
            importer.Import();

            // Assert
            Assert.AreEqual(expectedProgressMessages.Length, progressChangedCallCount);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new ReferenceLineImporter(assessmentSection, path);

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': Bestandspad mag niet verwijzen naar een lege bestandsnaam. ", path) + Environment.NewLine +
                                  "Er is geen referentielijn geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
            Assert.IsNull(assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ShapefileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist");

            var importer = new ReferenceLineImporter(assessmentSection, path);

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': Het bestand bestaat niet. ", path) + Environment.NewLine +
                                  "Er is geen referentielijn geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
            Assert.IsNull(assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToCancel_NoChanges()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.StrictMock<ICalculation>();
            var calculation3 = mocks.StrictMock<ICalculation>();

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation3
            });

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var importer = new ReferenceLineImporter(assessmentSection, path);
            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);

                messageBoxTitle = messageBoxTester.Title;
                messageBoxText = messageBoxTester.Text;

                messageBoxTester.ClickCancel();
            };

            // Call
            bool importSuccesful = importer.Import();

            // Assert
            Assert.IsFalse(importSuccesful);
            Assert.AreSame(originalReferenceLine, assessmentSection.ReferenceLine);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            var expectedText = "Als u de referentielijn vervangt, zullen alle vakindelingen, berekende hydraulische randvoorwaarden en berekeningsresultaten worden verwijderd." + Environment.NewLine +
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
            var calculation1 = mocks.Stub<ICalculation>();
            calculation1.Expect(c => c.ClearOutput());
            var calculation2 = mocks.Stub<ICalculation>();
            calculation2.Expect(c => c.ClearOutput());
            var calculation3 = mocks.Stub<ICalculation>();
            calculation3.Expect(c => c.ClearOutput());
            var calculation4 = mocks.Stub<ICalculation>();
            calculation4.Expect(c => c.ClearOutput());

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Expect(fm => fm.ClearAllSections());
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Expect(fm => fm.ClearAllSections());
            failureMechanism2.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation3,
                calculation4
            });

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importer = new ReferenceLineImporter(assessmentSection, path);

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);

                messageBoxTitle = messageBoxTester.Title;
                messageBoxText = messageBoxTester.Text;

                messageBoxTester.ClickOk();
            };

            // Call
            bool importSuccesful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccesful);
            Assert.AreNotSame(originalReferenceLine, assessmentSection.ReferenceLine);
            Point2D[] point2Ds = assessmentSection.ReferenceLine.Points.ToArray();
            Assert.AreEqual(803, point2Ds.Length);
            Assert.AreEqual(198237.375, point2Ds[123].X, 1e-6);
            Assert.AreEqual(514879.781, point2Ds[123].Y, 1e-6);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            var expectedText = "Als u de referentielijn vervangt, zullen alle vakindelingen, berekende hydraulische randvoorwaarden en berekeningsresultaten worden verwijderd." + Environment.NewLine +
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
            var calculation1 = mocks.Stub<ICalculation>();
            var calculation2 = mocks.Stub<ICalculation>();

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Expect(fm => fm.ClearAllSections());
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2
            });

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1
            });
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification
                {
                    Text = "Inlezen referentielijn.", CurrentStep = 1, MaxNrOfSteps = 4
                },
                new ExpectedProgressNotification
                {
                    Text = "Geïmporteerde data toevoegen aan het traject.", CurrentStep = 2, MaxNrOfSteps = 4
                },
                new ExpectedProgressNotification
                {
                    Text = "Wissen rekenresultaten en vakindelingen van toetsspoor.", CurrentStep = 3, MaxNrOfSteps = 4
                },
                new ExpectedProgressNotification
                {
                    Text = "Verwijderen uitvoer van hydraulische randvoorwaarden.", CurrentStep = 4, MaxNrOfSteps = 4
                }
            };
            var progressChangedCallCount = 0;
            var importer = new ReferenceLineImporter(assessmentSection, path);
            importer.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].MaxNrOfSteps, steps);
                progressChangedCallCount++;
            });

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Call
            importer.Import();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var importer = new ReferenceLineImporter(assessmentSection, path);

            DialogBoxHandler = (name, wnd) =>
            {
                importer.Cancel();

                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Call
            bool importSuccesful = importer.Import();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var importer = new ReferenceLineImporter(assessmentSection, path);

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
            Action call = () => importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Referentielijn importeren afgebroken. Geen data ingelezen.", 1);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ReusingCancelledImporterForContextWithoutReferenceLine_ImportReferenceLineToAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importer = new ReferenceLineImporter(assessmentSection, path);
            importer.Cancel();

            // Call
            bool importSuccesful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccesful);
            Assert.IsNotNull(assessmentSection.ReferenceLine);
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
            var calculation1 = mocks.Stub<ICalculation>();
            calculation1.Stub(c => c.ClearOutput());
            calculation1.Expect(c => c.NotifyObservers());
            var calculation2 = mocks.Stub<ICalculation>();
            calculation2.Stub(c => c.ClearOutput());
            calculation2.Expect(c => c.NotifyObservers());
            var calculation3 = mocks.Stub<ICalculation>();
            calculation3.Stub(c => c.ClearOutput());
            calculation3.Expect(c => c.NotifyObservers());
            var calculation4 = mocks.Stub<ICalculation>();
            calculation4.Stub(c => c.ClearOutput());
            calculation4.Expect(c => c.NotifyObservers());

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Expect(fm => fm.ClearAllSections());
            failureMechanism1.Expect(fm => fm.NotifyObservers());
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Expect(fm => fm.ClearAllSections());
            failureMechanism2.Expect(fm => fm.NotifyObservers());
            failureMechanism2.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation3,
                calculation4
            });

            var contextObserver = mocks.Stub<IObserver>();
            contextObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(section => section.Attach(contextObserver));
            assessmentSection.Expect(section => section.NotifyObservers()).Do((Action) (() => contextObserver.UpdateObserver()));

            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter(assessmentSection, path);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImportUpdates();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations and context
        }

        [Test]
        public void DoPostImportUpdates_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextParent()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Expect(section => section.Attach(observer));
            assessmentSection.Expect(section => section.NotifyObservers());
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.WrappedData.Attach(observer);

            var importer = new ReferenceLineImporter(assessmentSection, path);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImportUpdates();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on context parent
        }

        [Test]
        public void DoPostImportUpdates_CancellingImport_DoNotNotifyObservers()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.StrictMock<ICalculation>();

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1
            });

            var contextObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1
            });
            assessmentSection.Expect(section => section.Attach(contextObserver));

            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter(assessmentSection, path);

            DialogBoxHandler = (name, wnd) =>
            {
                importer.Cancel();

                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Precondition
            Assert.IsFalse(importer.Import());

            // Call
            importer.DoPostImportUpdates();

            // Assert
            mocks.VerifyAll(); // Expect no NotifyObserver calls
        }

        [Test]
        public void DoPostImportUpdates_ReuseImporterWithAssessmentSectionWithReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextAndClearedObjects()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var calculation1 = mocks.Stub<ICalculation>();
            calculation1.Stub(c => c.ClearOutput());
            calculation1.Expect(c => c.NotifyObservers());
            var calculation2 = mocks.Stub<ICalculation>();
            calculation2.Stub(c => c.ClearOutput());
            calculation2.Expect(c => c.NotifyObservers());
            var calculation3 = mocks.Stub<ICalculation>();
            calculation3.Stub(c => c.ClearOutput());
            calculation3.Expect(c => c.NotifyObservers());
            var calculation4 = mocks.Stub<ICalculation>();
            calculation4.Stub(c => c.ClearOutput());
            calculation4.Expect(c => c.NotifyObservers());

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Expect(fm => fm.ClearAllSections()).Repeat.Twice();
            failureMechanism1.Expect(fm => fm.NotifyObservers());
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2
            });

            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Expect(fm => fm.ClearAllSections()).Repeat.Twice();
            failureMechanism2.Expect(fm => fm.NotifyObservers());
            failureMechanism2.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation3,
                calculation4
            });

            var contextObserver = mocks.Stub<IObserver>();
            contextObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism1,
                failureMechanism2
            });
            assessmentSection.Expect(section => section.Attach(contextObserver));
            assessmentSection.Expect(section => section.NotifyObservers()).Do((Action) (() => contextObserver.UpdateObserver()));

            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter(assessmentSection, path);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };

            // Precondition
            Assert.IsTrue(importer.Import());
            importer.Cancel();
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBoxTester = new MessageBoxTester(wnd);
                messageBoxTester.ClickOk();
            };
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImportUpdates();

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