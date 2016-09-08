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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class DikeProfilesImporterTest
    {
        [Test]
        public void Constructor_WithValidParameters_ReturnsNewInstance()
        {
            // Setup
            var importTarget = new ObservableList<DikeProfile>();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new DikeProfilesImporter(importTarget, referenceLine, "");

            // Assert
            Assert.IsInstanceOf<ProfilesImporter<ObservableList<DikeProfile>>>(importer);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfilesImporter(null, new ReferenceLine(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void Constructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfilesImporter(new ObservableList<DikeProfile>(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfilesImporter(new ObservableList<DikeProfile>(), new ReferenceLine(), null);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath);
            var importResult = false;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = "Kan geen geldige voorland- en dijkprofieldata vinden voor dijkprofiel locatie met ID: unmatchable";
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_TrueAndLogErrorAndFourDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath);

            var importResult = false;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            string expectedMessage = "Een profiellocatie met ID 'profiel005' ligt niet op de referentielijn. Locatie wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_AllOkTestData_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var progressChangeNotifications = new List<ProgressNotification>();
            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath)
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            // Call
            bool importResult = dikeProfilesImporter.Import();

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
                new ProgressNotification("Inlezen van profieldata uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van profieldata.", 1, 5),
                new ProgressNotification("Inlezen van profieldata.", 2, 5),
                new ProgressNotification("Inlezen van profieldata.", 3, 5),
                new ProgressNotification("Inlezen van profieldata.", 4, 5),
                new ProgressNotification("Inlezen van profieldata.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
        }

        [Test]
        public void Import_AllOkTestData_CorrectDikeProfileProperties()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath);

            // Call
            dikeProfilesImporter.Import();

            // Assert
            DikeProfile dikeProfile = failureMechanism.DikeProfiles[4];
            Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), dikeProfile.WorldReferencePoint);
            Assert.AreEqual("profiel005", dikeProfile.Name);
            Assert.AreEqual(15.56165507, dikeProfile.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), dikeProfile.Orientation);
            Assert.IsTrue(dikeProfile.HasBreakWater);
            Assert.AreEqual(new RoundedDouble(2, 6.0), dikeProfile.DikeHeight);
        }

        [Test]
        public void Import_AllDamTypes_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var progressChangeNotifications = new List<ProgressNotification>();
            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath)
            {
                ProgressChanged = (description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps))
            };

            // Call
            bool importResult = dikeProfilesImporter.Import();

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
                new ProgressNotification("Inlezen van profieldata uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van profieldata.", 1, 5),
                new ProgressNotification("Inlezen van profieldata.", 2, 5),
                new ProgressNotification("Inlezen van profieldata.", 3, 5),
                new ProgressNotification("Inlezen van profieldata.", 4, 5),
                new ProgressNotification("Inlezen van profieldata.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, failureMechanism.DikeProfiles.Count);
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath);

            dikeProfilesImporter.Cancel();
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Dijkprofielen importeren is afgebroken. Geen data ingelezen.", 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine, filePath);

            dikeProfilesImporter.Cancel();
            bool importResult = dikeProfilesImporter.Import();
            Assert.IsFalse(importResult);

            // Call
            importResult = dikeProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(5, failureMechanism.DikeProfiles.Count);
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