﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        public void ParameterdConstructor_ExpectedValues()
        {
            // Setup
            var importTarget = new ObservableList<ForeshoreProfile>();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new ForeshoreProfilesImporter(importTarget, referenceLine, "");

            // Assert
            Assert.IsInstanceOf<ProfilesImporter<ObservableList<ForeshoreProfile>>>(importer);
        }

        [Test]
        public void ParameterdConstructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(null, new ReferenceLine(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void ParameterdConstructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ObservableList<ForeshoreProfile>(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void ParameterdConstructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ObservableList<ForeshoreProfile>(), new ReferenceLine(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Import_FromFileWithUnmatchableId_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
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
                string expectedMessage = "Kan geen geldige gegevens vinden voor voorlandprofiellocatie met ID 'unmatchable'.";
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FiveForeshoreProfilesWithoutDamsAndGeometries_TrueAndLogWarningAndTwoForeshoreProfiles()
        {
            // Setup
            string fileDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                              Path.Combine("DikeProfiles", "NoDamsAndNoForeshoreGeometries"));
            string filePath = Path.Combine(fileDirectory, "Voorlanden 12-2.shp");

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                string.Format("Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{0}' wordt overgeslagen.", Path.Combine(fileDirectory, "profiel001 - Ringtoets.prfl")),
                string.Format("Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{0}' wordt overgeslagen.", Path.Combine(fileDirectory, "profiel003 - Ringtoets.prfl")),
                string.Format("Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{0}' wordt overgeslagen.", Path.Combine(fileDirectory, "profiel004 - Ringtoets.prfl")),
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(2, foreshoreProfiles.Count);
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

            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            string expectedMessage = "Een profiellocatie met ID 'profiel005' ligt niet op de referentielijn. Locatie wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsTrue(importResult);
            Assert.AreEqual(4, foreshoreProfiles.Count);
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
            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
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
                new ProgressNotification("Inlezen van profielgegevens.", 5, 5)
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

            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            var targetContext = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);
            targetContext.Attach(observer);

            // Call
            foreshoreProfilesImporter.Import();

            // Assert
            ForeshoreProfile foreshoreProfile = foreshoreProfiles[4];
            Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), foreshoreProfile.WorldReferencePoint);
            Assert.AreEqual("profiel005", foreshoreProfile.Name);
            Assert.AreEqual(15.56165507, foreshoreProfile.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), foreshoreProfile.Orientation);
            Assert.IsTrue(foreshoreProfile.HasBreakWater);
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
            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            var targetContext = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
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
                new ProgressNotification("Inlezen van profielgegevens.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, foreshoreProfiles.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            // Precondition
            foreshoreProfilesImporter.Cancel();

            // Call
            var importResult = true;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Voorlandprofielen importeren is afgebroken. Geen gegevens ingelezen.", 1);
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfiles = new ObservableList<ForeshoreProfile>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath);

            foreshoreProfilesImporter.Cancel();
            bool importResult = foreshoreProfilesImporter.Import();
            Assert.IsFalse(importResult);

            // Call
            importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(5, foreshoreProfiles.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
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
                var notification = expectedProgressMessages[i];
                var actualNotification = progressChangeNotifications[i];
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

            public string Text { get; private set; }
            public int CurrentStep { get; private set; }
            public int TotalSteps { get; private set; }
        }
    }
}