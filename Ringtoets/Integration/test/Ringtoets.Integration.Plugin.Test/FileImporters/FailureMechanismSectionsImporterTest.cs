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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Plugin.FileImporters;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionsImporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var importer = new FailureMechanismSectionsImporter();

            // Assert
            Assert.IsInstanceOf<FileImporterBase<FailureMechanismSectionsContext>>(importer);
            Assert.AreEqual("Vakindeling", importer.Name);
            Assert.AreEqual("Algemeen", importer.Category);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.Sections, importer.Image);
            Assert.AreEqual("Vakindeling shapefile (*.shp)|*.shp", importer.FileFilter);
        }

        [Test]
        public void CanImportOn_ValidContextWithReferenceLine_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var targetContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            var importer = new FailureMechanismSectionsImporter();

            // Call
            var canImport = importer.CanImportOn(targetContext);

            // Assert
            Assert.IsTrue(canImport);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_ValidContextWithoutReferenceLine_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            mocks.ReplayAll();

            var targetContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            var importer = new FailureMechanismSectionsImporter();

            // Call
            var canImport = importer.CanImportOn(targetContext);

            // Assert
            Assert.IsFalse(canImport);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1_vakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            var importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(62, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileCorrespondingToReferenceLineAndHasSectionImported_ReplaceSections()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1_vakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            failureMechanism.AddSection(new FailureMechanismSection("A", assessmentSection.ReferenceLine.Points));
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            var importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(62, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidArtificialFileImperfectlyCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA_ValidVakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            var importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(7, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidImport_GenerateExpectedProgressMessages()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA_ValidVakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var progressChangeNotifications = new List<ProgressNotification>();

            var importer = new FailureMechanismSectionsImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            var importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            Assert.IsTrue(importSuccessful);
            var expectedProgressMessages = new[]
            {
                new ProgressNotification("Inlezen vakindeling.", 1, 3),
                new ProgressNotification("Valideren ingelezen vakindeling.", 2, 3),
                new ProgressNotification("Geïmporteerde data toevoegen aan het toetsspoor.", 3, 3),
            };
            Assert.AreEqual(expectedProgressMessages.Length, progressChangeNotifications.Count);
            for (int i = 0; i < expectedProgressMessages.Length; i++)
            {
                var notification = expectedProgressMessages[i];
                var actualNotification = progressChangeNotifications[i];
                Assert.AreEqual(notification.Text, actualNotification.Text);
                Assert.AreEqual(notification.CurrentStep, actualNotification.CurrentStep);
                Assert.AreEqual(notification.TotalSteps, actualNotification.TotalSteps);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen. ", sectionsFilePath) + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': Het bestand bestaat niet. ", sectionsFilePath) + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_NoReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1_vakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            mocks.ReplayAll();

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = "Er is geen referentielijn beschikbaar om een vakindeling voor te definiëren. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptyArtificialFile_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA_EmptyVakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = "Het bestand heeft geen vakindeling. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartTooFarFromReferenceline")]
        [TestCase("EndTooFarFromReferenceline")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var shapeFileName = String.Format("Artificial_referencelijn_testA_InvalidVakken_Section{0}.shp", shapeCondition);
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, shapeFileName);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartTooFarFromReferencelineStart")]
        [TestCase("EndTooFarFromReferencelineEnd")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromStartEndOfReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var shapeFileName = String.Format("Artificial_referencelijn_testA_InvalidVakken_{0}.shp", shapeCondition);
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, shapeFileName);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSectionsDoNotFullyCoverReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA_InvalidVakken_NotCoveringWholeReferenceLine.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSomePointsNotOnReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Artificial_referencelijn_testA_InvalidVakken_PointsTooFarFromReferenceLine.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanismSectionsContext.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileImportBeingCancelled_CancelImportWithInfoMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1.shp");
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "traject_1-1_vakken.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            var referenceLineImporter = new ReferenceLineImporter();
            referenceLineImporter.Import(referenceLineContext, referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter();

            var failureMechanism = new Simple();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            importer.Cancel();
            Assert.IsFalse(importer.Import(failureMechanismSectionsContext, sectionsFilePath));

            // Call
            var importSuccessful = importer.Import(failureMechanismSectionsContext, sectionsFilePath);

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(62, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        private void AssertSectionsAreValidForReferenceLine(FailureMechanismSection[] sections, ReferenceLine referenceLine)
        {
            Point2D[] referenceLineGeometry = referenceLine.Points.ToArray();

            // 1. Start & End coherence:
            Assert.AreEqual(referenceLineGeometry[0], sections[0].GetStart(),
                            "Start of the sections should correspond to the Start of the reference line.");
            Assert.AreEqual(referenceLineGeometry[referenceLineGeometry.Length - 1], sections[sections.Length - 1].GetLast(),
                            "End of the sections should correspond to the End of the reference line.");

            // 2. Total length coherence:
            var totalLengthOfSections = sections.Sum(s => GetLengthOfLine(s.Points));
            var totalLengthOfReferenceLine = GetLengthOfLine(referenceLineGeometry);
            Assert.AreEqual(totalLengthOfReferenceLine, totalLengthOfSections, 1e-6,
                            "The length of all sections should sum up to the length of the reference line.");

            // 3. Section Start and End coherence
            IEnumerable<Point2D> allStartAndEndPoints = sections.Select(s => s.GetStart()).Concat(sections.Select(s => s.GetLast()));
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
                    Assert.AreEqual(sectionTowardsStart.GetLast(), sectionTowardsEnd.GetStart(),
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

        private double GetLengthOfLine(IEnumerable<Point2D> linePoints)
        {
            return GetLineSegments(linePoints).Sum(segment => segment.Length);
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

        private class Simple : FailureMechanismBase<FailureMechanismSectionResult>
        {
            public Simple() : base("Stubbed name","Stubbed code") {}

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            protected override FailureMechanismSectionResult CreateFailureMechanismSectionResult(FailureMechanismSection section)
            {
                return new FailureMechanismSectionResult(section); 
            }
        }

        private class ProgressNotification
        {
            public ProgressNotification(string description, int currentStep, int totalSteps)
            {
                Text = description;
                CurrentStep = currentStep;
                TotalSteps = totalSteps;
            }

            public string Text { get; private set; }
            public int CurrentStep { get; private set; }
            public int TotalSteps { get; private set; }
        }
    }
}