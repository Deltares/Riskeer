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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLineImporterTest
    {
        [Test]
        public void Constructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            var mocks = new MockRepository();
            var handler = mocks.Stub<IReferenceLineUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ReferenceLineImporter(null, handler, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReferenceLineImporter(new ReferenceLine(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IReferenceLineUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<ReferenceLine>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_WhenSuccessful_UpdatesOriginalReferenceLineWithImportedReferenceLine()
        {
            // Setup
            var originalReferenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            handler.Expect(h => h.Update(Arg<ReferenceLine>.Is.NotNull,
                                         Arg<ReferenceLine>.Is.NotNull))
                   .WhenCalled(invocation =>
                   {
                       Assert.AreSame(originalReferenceLine, invocation.Arguments[0]);
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

            var importer = new ReferenceLineImporter(originalReferenceLine, handler, path);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_WhenSuccessful_GeneratedExpectedProgressMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IReferenceLineUpdateHandler>();
            handler.Expect(h => h.Update(Arg<ReferenceLine>.Is.NotNull,
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
            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, path);
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
            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, path);

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
            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist");

            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, path);

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
        public void Import_CancelImportDuringDialogInteraction_ReturnsFalseAndNoChanges()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            handler.Expect(h => h.ConfirmUpdate()).Return(false);
            mocks.ReplayAll();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");

            var importer = new ReferenceLineImporter(ReferenceLineTestFactory.CreateReferenceLineWithGeometry(), handler, path);

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

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            var importer = new ReferenceLineImporter(ReferenceLineTestFactory.CreateReferenceLineWithGeometry(), handler, path);
            handler.Expect(h => h.ConfirmUpdate())
                   .WhenCalled(invocation => importer.Cancel())
                   .Return(acceptRemovalOfReferenceLineDependentData);
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

            var mocks = new MockRepository();
            var handler = mocks.Stub<IReferenceLineUpdateHandler>();
            mocks.ReplayAll();

            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, path);
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

            var mocks = new MockRepository();
            var handler = mocks.Stub<IReferenceLineUpdateHandler>();
            handler.Stub(h => h.Update(null, null))
                   .IgnoreArguments()
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, path);
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
        public void Import_ReusingCanceledImporterForReferenceLineWithoutGeometry_ImportReferenceLine()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            handler.Expect(h => h.Update(Arg<ReferenceLine>.Is.NotNull,
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

            var importer = new ReferenceLineImporter(new ReferenceLine(), handler, path);
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
        public void DoPostImportUpdates_ReferenceLineHasGeometryAndAnswerDialogToContinue_NotifyObserversOfTargetAndClearedObjects()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.Combine("ReferenceLine", "traject_10-2.shp"));
            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();

            var mocks = new MockRepository();
            var referenceLineObserver = mocks.Stub<IObserver>();
            referenceLineObserver.Expect(o => o.UpdateObserver());

            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            handler.Expect(h => h.ConfirmUpdate()).Return(true);
            handler.Expect(h => h.Update(Arg<ReferenceLine>.Is.Same(referenceLine),
                                         Arg<ReferenceLine>.Is.NotNull))
                   .Return(new[]
                   {
                       observable1,
                       observable2
                   });
            handler.Expect(h => h.DoPostUpdateActions());
            mocks.ReplayAll();

            referenceLine.Attach(referenceLineObserver);

            var importer = new ReferenceLineImporter(referenceLine, handler, path);

            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations
        }

        [Test]
        public void DoPostImportUpdates_CancelingImport_DoNotNotifyObserversAndNotDoPostReplacementUpdates()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_10-2.shp");
            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();

            var handler = mocks.StrictMock<IReferenceLineUpdateHandler>();
            var importer = new ReferenceLineImporter(referenceLine, handler, path);
            handler.Expect(h => h.ConfirmUpdate())
                   .WhenCalled(invocation => importer.Cancel())
                   .Return(true);

            mocks.ReplayAll();

            referenceLine.Attach(observer);

            // Precondition
            Assert.IsFalse(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect no NotifyObserver calls
        }

        [Test]
        public void DoPostImportUpdates_ReuseImporterWithReferenceLineAndAnswerDialogToContinue_NotifyObserversOfTargetAndClearedObjects()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.Combine("ReferenceLine", "traject_10-2.shp"));
            var referenceLine = new ReferenceLine();

            var mocks = new MockRepository();
            var referenceLineObserver = mocks.Stub<IObserver>();
            referenceLineObserver.Expect(o => o.UpdateObserver());

            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler = mocks.Stub<IReferenceLineUpdateHandler>();
            handler.Expect(h => h.Update(Arg<ReferenceLine>.Is.Same(referenceLine),
                                         Arg<ReferenceLine>.Is.NotNull))
                   .Return(new[]
                   {
                       observable1,
                       observable2
                   });
            handler.Expect(h => h.DoPostUpdateActions());

            mocks.ReplayAll();

            referenceLine.Attach(referenceLineObserver);

            var importer = new ReferenceLineImporter(referenceLine, handler, path);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            // Precondition
            Assert.IsFalse(importer.Import());

            importer.SetProgressChanged(null);
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Expect NotifyObservers on cleared calculations
        }
    }
}