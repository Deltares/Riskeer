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
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.ReferenceLines;

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
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", referenceLineFileName));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", sectionsFileName));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            FailureMechanismSection[] sections = failureMechanism.Sections.ToArray();
            Assert.AreEqual(sectionCount, sections.Length);
            AssertSectionsAreValidForReferenceLine(sections, assessmentSection.ReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileCorrespondingToReferenceLineAndHasSectionImported_ReplaceSections()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "traject_1-1.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();
            failureMechanism.AddSection(new FailureMechanismSection("A", assessmentSection.ReferenceLine.Points));

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = importer.Import();

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
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            var importSuccessful = importer.Import();

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
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var progressChangeNotifications = new List<ProgressNotification>();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            var importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            var expectedProgressMessages = new[]
            {
                new ProgressNotification("Inlezen vakindeling.", 1, 3),
                new ProgressNotification("Valideren ingelezen vakindeling.", 2, 3),
                new ProgressNotification("Geïmporteerde gegevens toevoegen aan het toetsspoor.", 3, 3)
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
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "traject_1-1.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': bestandspad mag niet verwijzen naar een lege bestandsnaam. ", sectionsFilePath) + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "traject_1-1.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist.shp");

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = 
                string.Format(@"Fout bij het lezen van bestand '{0}': het bestand bestaat niet. ", sectionsFilePath) + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptyArtificialFile_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_EmptyVakken.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = "Het bestand heeft geen vakindeling. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartTooFarFromReferenceline")]
        [TestCase("EndTooFarFromReferenceline")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var shapeFileName = string.Format("Artificial_referencelijn_testA_InvalidVakken_Section{0}.shp", shapeCondition);
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", shapeFileName));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartTooFarFromReferencelineStart")]
        [TestCase("EndTooFarFromReferencelineEnd")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromStartEndOfReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var shapeFileName = string.Format("Artificial_referencelijn_testA_InvalidVakken_{0}.shp", shapeCondition);
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", shapeFileName));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSectionsDoNotFullyCoverReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_NotCoveringWholeReferenceLine.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSomePointsNotOnReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_PointsTooFarFromReferenceLine.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = "Vakindeling komt niet overeen met de huidige referentielijn. " + Environment.NewLine +
                                  "Er is geen vakindeling geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_MissingNameValue_CancelImportWithErrorMessage()
        {
            // Setup
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "vakindeling_Empty_Name_Value.shp"));

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), sectionsFilePath);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = string.Format(
                "Fout bij het lezen van bestand '{0}': voor één of meerdere vakken is geen naam opgegeven. {1}Er is geen vakindeling geïmporteerd.",
                sectionsFilePath,
                Environment.NewLine);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void Import_ValidFileImportBeingCancelled_CancelImportWithInfoMessage()
        {
            // Setup
            var referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                   Path.Combine("ReferenceLine", "traject_1-1.shp"));
            var sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, referenceLineFilePath);
            referenceLineImporter.Import();

            var failureMechanism = new Simple();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, sectionsFilePath);

            importer.Cancel();
            Assert.IsFalse(importer.Import());

            // Call
            var importSuccessful = importer.Import();

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