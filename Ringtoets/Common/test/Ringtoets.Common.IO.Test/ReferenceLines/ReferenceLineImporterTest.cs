// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.ReferenceLines;

namespace Ringtoets.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLineImporterTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            var mocks = new MockRepository();
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ReferenceLineImporter(null, handler, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new ReferenceLineImporter(assessmentSection, handler, "");

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
            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                          Arg<ReferenceLine>.Is.NotNull))
                   .WhenCalled(invocation =>
                   {
                       var importedReferenceLine = (ReferenceLine) invocation.Arguments[1];
                       Point2D[] point2Ds = importedReferenceLine.Points.ToArray();
                       Assert.AreEqual(803, point2Ds.Length);
                       Assert.AreEqual(193515.719, point2Ds[467].X, 1e-6);
                       Assert.AreEqual(511444.750, point2Ds[467].Y, 1e-6);
                   })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                     Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ContextWithoutReferenceLine_GeneratedExpectedProgressMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                          Arg<ReferenceLine>.Is.NotNull))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                     Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification
                {
                    Text = "Inlezen referentielijn.",
                    CurrentStep = 1,
                    TotalNumberOfSteps = 2
                },
                new ExpectedProgressNotification
                {
                    Text = "Geïmporteerde data toevoegen aan het toetsspoor.",
                    CurrentStep = 2,
                    TotalNumberOfSteps = 2
                }
            };
            var progressChangedCallCount = 0;
            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            importer.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].TotalNumberOfSteps, steps);
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
            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.ConfirmReplace())
                   .Repeat.Never();
            handler.Expect(h => h.Replace(null, null))
                   .IgnoreArguments()
                   .Repeat.Never();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{path}': bestandspad mag niet verwijzen naar een lege bestandsnaam. "
                                     + $"{Environment.NewLine}Er is geen referentielijn geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ShapefileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.ConfirmReplace())
                   .Repeat.Never();
            handler.Expect(h => h.Replace(null, null))
                   .IgnoreArguments()
                   .Repeat.Never();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist");

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{path}': het bestand bestaat niet. "
                                     + $"{Environment.NewLine}Er is geen referentielijn geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelingImport_ReturnFalseAndNoChanges()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.ConfirmReplace()).Return(false);
            handler.Expect(h => h.Replace(null, null))
                   .IgnoreArguments()
                   .Repeat.Never();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsFalse(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Import_CancelImportDuringDialogInteraction_GenerateCanceledLogMessageAndReturnsFalse(bool acceptRemovalOfReferenceLineDependentData)
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            handler.Expect(h => h.ConfirmReplace())
                   .WhenCalled(invocation => importer.Cancel())
                   .Return(acceptRemovalOfReferenceLineDependentData);
            handler.Expect(h => h.Replace(null, null))
                   .IgnoreArguments()
                   .Repeat.Never();
            mocks.ReplayAll();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Referentielijn importeren afgebroken. Geen gegevens gewijzigd.", 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringReadReferenceLine_CancelsImportAndLogs()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.Combine("ReferenceLine", "traject_10-2.shp"));
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            handler.Stub(h => h.ConfirmReplace())
                   .Return(true);
            mocks.ReplayAll();

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen referentielijn."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Referentielijn importeren afgebroken. Geen gegevens gewijzigd.", 1);
            Assert.IsFalse(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelImportDuringAddReferenceLineToData_ContinuesImportAndLogs()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.Combine("ReferenceLine", "traject_10-2.shp"));
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            handler.Stub(h => h.ConfirmReplace())
                   .Return(true);
            handler.Stub(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                        Arg<ReferenceLine>.Is.NotNull))
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Geïmporteerde data toevoegen aan het toetsspoor."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            importer.Import();
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Huidige actie was niet meer te annuleren en is daarom voortgezet.", 1);
            Assert.IsTrue(importResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ReusingCanceledImporterForContextWithoutReferenceLine_ImportReferenceLineToAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.ConfirmReplace())
                   .Repeat.Never();
            handler.Expect(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                          Arg<ReferenceLine>.Is.NotNull))
                   .WhenCalled(invocation =>
                   {
                       var importedReferenceLine = (ReferenceLine) invocation.Arguments[1];
                       Point2D[] point2Ds = importedReferenceLine.Points.ToArray();
                       Assert.AreEqual(803, point2Ds.Length);
                       Assert.AreEqual(195203.563, point2Ds[321].X, 1e-6);
                       Assert.AreEqual(512826.406, point2Ds[321].Y, 1e-6);
                   })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                     Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            // Precondition
            bool importSuccessful = importer.Import();
            Assert.IsFalse(importSuccessful);
            importer.SetProgressChanged(null);

            // Call
            importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostImportUpdates_AssessmentSectionAlreadyHasReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextAndClearedObjects()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var contextObserver = mocks.Stub<IObserver>();
            contextObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Expect(section => section.Attach(contextObserver));
            assessmentSection.Expect(section => section.NotifyObservers()).Do((Action) (() => contextObserver.UpdateObserver()));

            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            IObservable[] observables =
            {
                observable1,
                observable2
            };

            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            handler.Expect(h => h.ConfirmReplace()).Return(true);
            handler.Expect(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                          Arg<ReferenceLine>.Is.NotNull))
                   .Return(observables);
            handler.Expect(h => h.DoPostReplacementUpdates());
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                     Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations and context
        }

        [Test]
        public void DoPostImportUpdates_CancelingImport_DoNotNotifyObserversAndNotDoPostReplacementUpdates()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var contextObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Expect(section => section.Attach(contextObserver));

            var handler = mocks.StrictMock<IReferenceLineReplaceHandler>();
            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            handler.Expect(h => h.ConfirmReplace())
                   .WhenCalled(invocation => importer.Cancel())
                   .Return(true);
            handler.Expect(h => h.Replace(null, null))
                   .IgnoreArguments()
                   .Repeat.Never();

            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            // Precondition
            Assert.IsFalse(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect no NotifyObserver calls
        }

        [Test]
        public void DoPostImportUpdates_ReuseImporterWithAssessmentSectionWithReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetContextAndClearedObjects()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var contextObserver = mocks.Stub<IObserver>();
            contextObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = originalReferenceLine;
            assessmentSection.Expect(section => section.Attach(contextObserver));
            assessmentSection.Expect(section => section.NotifyObservers()).Do((Action) (() => contextObserver.UpdateObserver()));

            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            IObservable[] observables =
            {
                observable1,
                observable2
            };

            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            handler.Stub(h => h.ConfirmReplace()).Return(true);
            handler.Expect(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                          Arg<ReferenceLine>.Is.NotNull))
                   .Return(observables);
            handler.Expect(h => h.DoPostReplacementUpdates());

            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                     Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var referenceLineContext = new ReferenceLineContext(assessmentSection);
            referenceLineContext.Attach(contextObserver);

            var importer = new ReferenceLineImporter(assessmentSection, handler, path);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            // Precondition
            Assert.IsFalse(importer.Import());

            importer.SetProgressChanged(null);
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations and context
        }

        private class ExpectedProgressNotification
        {
            public string Text { get; set; }
            public int CurrentStep { get; set; }
            public int TotalNumberOfSteps { get; set; }
        }
    }
}