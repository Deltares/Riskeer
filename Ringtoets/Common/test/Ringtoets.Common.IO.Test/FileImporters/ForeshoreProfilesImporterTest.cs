// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfilesImporterTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var importTarget = new ForeshoreProfileCollection();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new ForeshoreProfilesImporter(importTarget, referenceLine, "");

            // Assert
            Assert.IsInstanceOf<ProfilesImporter<ForeshoreProfileCollection>>(importer);
        }

        [Test]
        public void ParameteredConstructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(null, new ReferenceLine(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), new ReferenceLine(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Import_FromFileWithUnmatchableId_FalseAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                const string expectedMessage = "Kan geen geldige gegevens vinden voor voorlandprofiellocatie met ID 'unmatchable'.";
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_TwoForeshoreProfilesWithoutDamsAndGeometries_TrueAndLogWarning()
        {
            // Setup
            string fileDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "NoDamsAndNoForeshoreGeometries"));
            string filePath = Path.Combine(fileDirectory, "Voorlanden 12-2.shp");

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            // Call
            var importResult = false;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                $"Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel001NoForeshoreNoDam - Ringtoets.prfl")}' wordt overgeslagen.",
                $"Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel002NoForeshoreNoDam - Ringtoets.prfl")}' wordt overgeslagen.",
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(importResult);
            Assert.AreEqual(5, foreshoreProfiles.Count);
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_TrueAndLogErrorAndFourForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(146039.4, 533920.2)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Fout bij het lezen van profiellocatie 5. De profiellocatie met " +
                                           "ID 'profiel005' ligt niet op de referentielijn.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsFalse(importResult);
            Assert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void Import_AllOkTestData_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van profiellocaties uit een shapebestand.", 1, 1),
                new ProgressNotification("Inlezen van profiellocatie.", 1, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 2, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 3, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 4, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 5, 5),
                new ProgressNotification("Inlezen van profielgegevens uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van profielgegevens.", 1, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 2, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 3, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 4, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 5, 5),
                new ProgressNotification("Geïmporteerde data toevoegen aan het toetsspoor.", 1, 1)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, foreshoreProfiles.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_AllOkTestData_CorrectForeshoreProfileProperties()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            var targetContext = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);
            targetContext.Attach(observer);

            // Call
            foreshoreProfilesImporter.Import();

            // Assert
            ForeshoreProfile foreshoreProfile1 = foreshoreProfiles[0];
            Assert.AreEqual("profiel001", foreshoreProfile1.Id);
            Assert.AreEqual("profiel001", foreshoreProfile1.Name);

            ForeshoreProfile foreshoreProfile2 = foreshoreProfiles[1];
            Assert.AreEqual("profiel002", foreshoreProfile2.Id);
            Assert.AreEqual("profiel002", foreshoreProfile2.Name);

            ForeshoreProfile foreshoreProfile3 = foreshoreProfiles[2];
            Assert.AreEqual("profiel003", foreshoreProfile3.Id);
            Assert.AreEqual("profiel003", foreshoreProfile3.Name);

            ForeshoreProfile foreshoreProfile4 = foreshoreProfiles[3];
            Assert.AreEqual(new Point2D(136432.12250000238, 538235.26300000318), foreshoreProfile4.WorldReferencePoint);
            Assert.AreEqual("profiel004", foreshoreProfile4.Id);
            Assert.AreEqual("Valide naam", foreshoreProfile4.Name);
            Assert.AreEqual(-17.93475471, foreshoreProfile4.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), foreshoreProfile4.Orientation);
            Assert.IsFalse(foreshoreProfile4.HasBreakWater);

            ForeshoreProfile foreshoreProfile5 = foreshoreProfiles[4];
            Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), foreshoreProfile5.WorldReferencePoint);
            Assert.AreEqual("profiel005", foreshoreProfile5.Id);
            Assert.AreEqual("Heeeeeeeeeeeeeeeeeeeeeeeele laaaaaaaaaaaaaaaaaaaange naaaaaaaaaaam", foreshoreProfile5.Name);
            Assert.AreEqual(15.56165507, foreshoreProfile5.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), foreshoreProfile5.Orientation);
            Assert.IsTrue(foreshoreProfile5.HasBreakWater);

            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_AllDamTypes_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            var targetContext = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van profiellocaties uit een shapebestand.", 1, 1),
                new ProgressNotification("Inlezen van profiellocatie.", 1, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 2, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 3, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 4, 5),
                new ProgressNotification("Inlezen van profiellocatie.", 5, 5),
                new ProgressNotification("Inlezen van profielgegevens uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van profielgegevens.", 1, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 2, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 3, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 4, 5),
                new ProgressNotification("Inlezen van profielgegevens.", 5, 5),
                new ProgressNotification("Geïmporteerde data toevoegen aan het toetsspoor.", 1, 1)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, foreshoreProfiles.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_CancelOfImporWhileReadingProfileLocations_CancelsImportAndLogs()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van profiellocaties uit een shapebestand."))
                {
                    foreshoreProfilesImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Voorlandprofielen importeren is afgebroken. Geen gegevens ingelezen.", 1);
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportWhileReadingDikeProfileData_CancelsImportAndLogs()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van profielgegevens uit een prfl bestand."))
                {
                    foreshoreProfilesImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Voorlandprofielen importeren is afgebroken. Geen gegevens ingelezen.", 1);
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) => foreshoreProfilesImporter.Cancel());

            // Precondition
            bool importResult = foreshoreProfilesImporter.Import();
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(foreshoreProfiles);

            foreshoreProfilesImporter.SetProgressChanged(null);

            // Call
            importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(5, foreshoreProfiles.Count);
            mockRepository.VerifyAll();
        }

        private ReferenceLine CreateMatchingReferenceLine()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(136039.4, 533920.2)
            });
            return referenceLine;
        }

        private static void ValidateProgressMessages(List<ProgressNotification> expectedProgressMessages, List<ProgressNotification> progressChangeNotifications)
        {
            Assert.AreEqual(expectedProgressMessages.Count, progressChangeNotifications.Count);
            for (var i = 0; i < expectedProgressMessages.Count; i++)
            {
                ProgressNotification notification = expectedProgressMessages[i];
                ProgressNotification actualNotification = progressChangeNotifications[i];
                Assert.AreEqual(notification.Text, actualNotification.Text);
                Assert.AreEqual(notification.CurrentStep, actualNotification.CurrentStep);
                Assert.AreEqual(notification.TotalSteps, actualNotification.TotalSteps);
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

            public string Text { get; }
            public int CurrentStep { get; }
            public int TotalSteps { get; }
        }
    }
}