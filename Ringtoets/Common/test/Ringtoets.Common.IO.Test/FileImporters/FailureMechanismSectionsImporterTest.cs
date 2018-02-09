﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionsImporterTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsImporter(null, referenceLine, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
        }

        [Test]
        public void Constructor_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsImporter(failureMechanism, null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("referenceLine", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var referenceLine = new ReferenceLine();

            // Call
            var importer = new FailureMechanismSectionsImporter(failureMechanism, referenceLine, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<IFailureMechanism>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("traject_1-1.shp", "traject_1-1_vakken.shp", 62)]
        [TestCase("traject_19-1.shp", "traject_19-1_vakken.shp", 17)]
        public void Import_ValidFileCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections(string referenceLineFileName, string sectionsFileName, int sectionCount)
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", referenceLineFileName));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", sectionsFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(sectionCount, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
        }

        [Test]
        public void Import_ValidFileCorrespondingToReferenceLineAndHasSectionImported_ReplaceSections()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();
            failureMechanism.AddSection(new FailureMechanismSection("A", importReferenceLine.Points));
            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(62, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
        }

        [Test]
        public void Import_ValidArtificialFileImperfectlyCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(7, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
        }

        [Test]
        public void Import_ValidImport_GenerateExpectedProgressMessages()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var progressChangeNotifications = new List<ProgressNotification>();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            var expectedProgressMessages = new[]
            {
                new ProgressNotification("Inlezen vakindeling.", 1, 3),
                new ProgressNotification("Valideren ingelezen vakindeling.", 2, 3),
                new ProgressNotification("Geïmporteerde data toevoegen aan het toetsspoor.", 3, 3)
            };

            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{sectionsFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist.shp");

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage =
                $@"Fout bij het lezen van bestand '{sectionsFilePath}': het bestand bestaat niet. " +
                $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_EmptyArtificialFile_CancelImportWithErrorMessage()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_EmptyVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = "Het bestand heeft geen vakindeling. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        [TestCase("StartTooFarFromReferenceline")]
        [TestCase("EndTooFarFromReferenceline")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string shapeFileName = $"Artificial_referencelijn_testA_InvalidVakken_Section{shapeCondition}.shp";
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", shapeFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = "De geografische ligging van ieder vak moet overeenkomen met de ligging van (een deel van) de referentielijn. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        [TestCase("StartTooFarFromReferencelineStart")]
        [TestCase("EndTooFarFromReferencelineEnd")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromStartEndOfReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string shapeFileName = $"Artificial_referencelijn_testA_InvalidVakken_{shapeCondition}.shp";
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", shapeFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = "De geografische ligging van ieder vak moet overeenkomen met de ligging van (een deel van) de referentielijn. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSectionsDoNotFullyCoverReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_NotCoveringWholeReferenceLine.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = "De opgetelde lengte van de vakken moet overeenkomen met de trajectlengte. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSomePointsNotOnReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_PointsTooFarFromReferenceLine.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = "De opgetelde lengte van de vakken moet overeenkomen met de trajectlengte. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_MissingNameValue_CancelImportWithErrorMessage()
        {
            // Setup
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "vakindeling_Empty_Name_Value.shp"));

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), sectionsFilePath);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{sectionsFilePath}': voor één of meerdere vakken is geen naam opgegeven. " +
                                     $"{Environment.NewLine}Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingFailureMechanismSections_CancelsImportAndLogs()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen vakindeling."))
                {
                    importer.Cancel();
                }
            });

            var importSuccessful = true;

            // Call
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Vakindeling importeren afgebroken. Geen gegevens gewijzigd.", 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_CancelOfImportWhenValidatingImportedections_CancelsImportAndLogs()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Valideren ingelezen vakindeling."))
                {
                    importer.Cancel();
                }
            });

            var importSuccessful = true;

            // Call
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Vakindeling importeren afgebroken. Geen gegevens gewijzigd.", 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ContinuesImportAndLogs()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Geïmporteerde data toevoegen aan het toetsspoor."))
                {
                    importer.Cancel();
                }
            });

            var importSuccessful = true;

            // Call
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Huidige actie was niet meer te annuleren en is daarom voortgezet.", 1);
            Assert.IsTrue(importSuccessful);
            CollectionAssert.IsNotEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_TrueAndLogMessagesAndExpectedImportedData()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            Assert.IsFalse(importer.Import());
            importer.SetProgressChanged(null);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(62, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObserver>();
            observable.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath);

            importer.Import();
            failureMechanism.SectionResults.Attach(observable);

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll();
        }

        private static ReferenceLine ImportReferenceLine(string referenceLineFilePath)
        {
            ReferenceLine importedReferenceLine = null;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            handler.Stub(h => h.ConfirmReplace()).Return(true);
            handler.Stub(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                        Arg<ReferenceLine>.Is.NotNull))
                   .WhenCalled(invocation => { importedReferenceLine = (ReferenceLine) invocation.Arguments[1]; })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, handler, referenceLineFilePath);
            referenceLineImporter.Import();

            mocks.VerifyAll();

            return importedReferenceLine;
        }

        private void AssertSectionsAreValidForReferenceLine(FailureMechanismSection[] sections, ReferenceLine referenceLine)
        {
            Point2D[] referenceLineGeometry = referenceLine.Points.ToArray();

            // 1. Start & End coherence:
            Assert.AreEqual(referenceLineGeometry[0], sections[0].StartPoint,
                            "Start of the sections should correspond to the Start of the reference line.");
            Assert.AreEqual(referenceLineGeometry.Last(), sections.Last().EndPoint,
                            "End of the sections should correspond to the End of the reference line.");

            // 2. Total length coherence:
            double totalLengthOfSections = sections.Sum(s => GetLineSegments(s.Points).Sum(segment => segment.Length));
            double totalLengthOfReferenceLine = referenceLine.Length;
            Assert.AreEqual(totalLengthOfReferenceLine, totalLengthOfSections, 1e-6,
                            "The length of all sections should sum up to the length of the reference line.");

            // 3. Section Start and End coherence
            IEnumerable<Point2D> allStartAndEndPoints = sections.Select(s => s.StartPoint).Concat(sections.Select(s => s.EndPoint));
            foreach (Point2D point in allStartAndEndPoints)
            {
                Assert.Less(GetDistanceToReferenceLine(point, referenceLine), 1e-6,
                            "All start- and end points should be on the reference line.");
            }

            // 4. Section Start and End points coherence
            FailureMechanismSection sectionTowardsEnd = null;
            foreach (FailureMechanismSection section in sections)
            {
                FailureMechanismSection sectionTowardsStart = sectionTowardsEnd;
                sectionTowardsEnd = section;

                if (sectionTowardsStart != null)
                {
                    Assert.AreEqual(sectionTowardsStart.EndPoint, sectionTowardsEnd.StartPoint,
                                    "All sections should be connected and in order of connectedness.");
                }
            }
        }

        private double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points)
                   .Select(segment => segment.GetEuclideanDistanceToPoint(point))
                   .Min();
        }

        private IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            Point2D endPoint = null;
            foreach (Point2D linePoint in linePoints)
            {
                Point2D startPoint = endPoint;
                endPoint = linePoint;

                if (startPoint != null)
                {
                    yield return new Segment2D(startPoint, endPoint);
                }
            }
        }

        private class Simple : FailureMechanismBase
        {
            public Simple() : base("Stubbed name", "Stubbed code") {}

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}