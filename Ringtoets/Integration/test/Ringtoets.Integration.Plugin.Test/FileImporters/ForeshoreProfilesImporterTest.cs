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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Asphalt.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = "Kan geen geldige voorlandprofieldata vinden voor voorlandprofiel locatie met ID: unmatchable";
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_TrueAndLogErrorAndFourForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
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

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            string expectedMessage = "Een profiel locatie met ID 'profiel005' ligt niet op de referentielijn. Locatie wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_AllOkTestData_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath)
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            // Call
            bool importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van profiel locaties uit een shapebestand.", 1, 1),
                new ProgressNotification("Inlezen van profiel locatie.", 1, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 2, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 3, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 4, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 5, 5),
                new ProgressNotification("Inlezen van profieldata uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van profieldata.", 1, 5),
                new ProgressNotification("Inlezen van profieldata.", 2, 5),
                new ProgressNotification("Inlezen van profieldata.", 3, 5),
                new ProgressNotification("Inlezen van profieldata.", 4, 5),
                new ProgressNotification("Inlezen van profieldata.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_AllOkTestData_CorrectForeshoreProfileProperties()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath);

            var targetContext = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            foreshoreProfilesImporter.Import();
            ForeshoreProfile foreshoreProfile = targetContext.WrappedData[4];

            // Assert
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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath)
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van profiel locaties uit een shapebestand.", 1, 1),
                new ProgressNotification("Inlezen van profiel locatie.", 1, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 2, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 3, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 4, 5),
                new ProgressNotification("Inlezen van profiel locatie.", 5, 5),
                new ProgressNotification("Inlezen van profieldata uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van profieldata.", 1, 5),
                new ProgressNotification("Inlezen van profieldata.", 2, 5),
                new ProgressNotification("Inlezen van profieldata.", 3, 5),
                new ProgressNotification("Inlezen van profieldata.", 4, 5),
                new ProgressNotification("Inlezen van profieldata.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, targetContext.WrappedData.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath);

            // Precondition
            foreshoreProfilesImporter.Cancel();
            bool importResult = true;

            // Call
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Voorlandprofielen importeren is afgebroken. Geen data ingelezen.", 1);
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var targetContext = new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection);

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(failureMechanism.ForeshoreProfiles, referenceLine, filePath);

            foreshoreProfilesImporter.Cancel();
            bool importResult = foreshoreProfilesImporter.Import();
            Assert.IsFalse(importResult);

            // Call
            importResult = foreshoreProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(5, targetContext.WrappedData.Count);
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