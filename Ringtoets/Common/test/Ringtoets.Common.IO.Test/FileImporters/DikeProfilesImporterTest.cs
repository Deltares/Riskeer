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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class DikeProfilesImporterTest
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithValidParameters_ReturnsNewInstance()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var importTarget = new DikeProfileCollection();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new DikeProfilesImporter(importTarget, referenceLine, "", new TestDikeProfileUpdateStrategy(), messageProvider);

            // Assert
            Assert.IsInstanceOf<ProfilesImporter<DikeProfileCollection>>(importer);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DikeProfilesImporter(null, new ReferenceLine(), "", new TestDikeProfileUpdateStrategy(), messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void Constructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DikeProfilesImporter(new DikeProfileCollection(), null, "", new TestDikeProfileUpdateStrategy(), messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DikeProfilesImporter(new DikeProfileCollection(), new ReferenceLine(), null, new TestDikeProfileUpdateStrategy(), messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Constructor_UpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DikeProfilesImporter(new DikeProfileCollection(), new ReferenceLine(), string.Empty, null, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dikeProfileUpdateStrategy", exception.ParamName);
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfilesImporter(new DikeProfileCollection(), new ReferenceLine(), string.Empty,
                                                               new TestDikeProfileUpdateStrategy(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void Import_FromFileWithUnmatchableId_TrueAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(),
                                                                referenceLine, filePath,
                                                                new TestDikeProfileUpdateStrategy(), messageProvider);
            var importResult = false;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Kan geen geldige gegevens vinden voor dijkprofiellocatie met ID 'unmatchable'.";
            var expectedLogMessage = new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_FiveDikeProfilesWithoutGeometries_TrueAndLogWarningAndNoDikeProfiles()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string fileDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                              Path.Combine("DikeProfiles", "NoDikeProfileGeometries"));
            string filePath = Path.Combine(fileDirectory, "Voorlanden 12-2.shp");

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var updateStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath, updateStrategy, messageProvider);

            // Call
            var importResult = false;
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            var expectedMessages = new[]
            {
                Tuple.Create($"Profielgegevens definiëren geen dijkgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel001 - Ringtoets.prfl")}' wordt overgeslagen.",
                             LogLevelConstant.Warn),
                Tuple.Create($"Profielgegevens definiëren geen dijkgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel002 - Ringtoets.prfl")}' wordt overgeslagen.",
                             LogLevelConstant.Warn),
                Tuple.Create($"Profielgegevens definiëren geen dijkgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel003 - Ringtoets.prfl")}' wordt overgeslagen.",
                             LogLevelConstant.Warn),
                Tuple.Create($"Profielgegevens definiëren geen dijkgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel004 - Ringtoets.prfl")}' wordt overgeslagen.",
                             LogLevelConstant.Warn),
                Tuple.Create($"Profielgegevens definiëren geen dijkgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel005 - Ringtoets.prfl")}' wordt overgeslagen.",
                             LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.IsTrue(updateStrategy.Updated);
            Assert.AreEqual(0, updateStrategy.ReadDikeProfiles.Length);
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_TrueAndLogErrorAndFourDikeProfiles()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

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

            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath, updateDataStrategy, messageProvider);

            var importResult = false;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            string expectedMessage = "Fout bij het lezen van profiellocatie 5. De profiellocatie met " +
                                     "ID 'profiel005' ligt niet op de referentielijn. " +
                                     "Dit profiel wordt overgeslagen.";
            var expectedLogMessage = new Tuple<string, LogLevelConstant>(expectedMessage,
                                                                         LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsTrue(importResult);
            Assert.IsTrue(updateDataStrategy.Updated);
            Assert.AreEqual(4, updateDataStrategy.ReadDikeProfiles.Length);
        }

        [Test]
        public void Import_AllOkTestData_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var progressChangeNotifications = new List<ProgressNotification>();

            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath, updateDataStrategy, messageProvider);
            dikeProfilesImporter.SetProgressChanged((description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); });

            // Call
            bool importResult = dikeProfilesImporter.Import();

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
                new ProgressNotification(expectedAddDataToModelProgressText, 1, 1)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);

            Assert.IsTrue(updateDataStrategy.Updated);
            Assert.AreEqual(5, updateDataStrategy.ReadDikeProfiles.Length);
        }

        [Test]
        public void Import_AllOkTestData_CorrectDikeProfileProperties()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath, updateDataStrategy, messageProvider);

            // Call
            dikeProfilesImporter.Import();

            // Assert
            Assert.IsTrue(updateDataStrategy.Updated);

            DikeProfile[] dikeProfiles = updateDataStrategy.ReadDikeProfiles;
            DikeProfile dikeProfile1 = dikeProfiles[0];
            Assert.AreEqual("profiel001", dikeProfile1.Id);
            Assert.AreEqual("profiel001", dikeProfile1.Name);

            DikeProfile dikeProfile2 = dikeProfiles[1];
            Assert.AreEqual("profiel002", dikeProfile2.Id);
            Assert.AreEqual("profiel002", dikeProfile2.Name);

            DikeProfile dikeProfile3 = dikeProfiles[2];
            Assert.AreEqual("profiel003", dikeProfile3.Id);
            Assert.AreEqual("profiel003", dikeProfile3.Name);

            DikeProfile dikeProfile4 = dikeProfiles[3];
            Assert.AreEqual(new Point2D(136432.12250000238, 538235.26300000318), dikeProfile4.WorldReferencePoint);
            Assert.AreEqual("profiel004", dikeProfile4.Id);
            Assert.AreEqual("Valide naam", dikeProfile4.Name);
            Assert.AreEqual(-17.93475471, dikeProfile4.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), dikeProfile4.Orientation);
            Assert.IsFalse(dikeProfile4.HasBreakWater);

            DikeProfile dikeProfile5 = dikeProfiles[4];
            Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), dikeProfile5.WorldReferencePoint);
            Assert.AreEqual("profiel005", dikeProfile5.Id);
            Assert.AreEqual("Heeeeeeeeeeeeeeeeeeeeeeeele laaaaaaaaaaaaaaaaaaaange naaaaaaaaaaam", dikeProfile5.Name);
            Assert.AreEqual(15.56165507, dikeProfile5.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), dikeProfile5.Orientation);
            Assert.IsTrue(dikeProfile5.HasBreakWater);
            Assert.AreEqual(new RoundedDouble(2, 6.0), dikeProfile5.DikeHeight);
        }

        [Test]
        public void Import_AllDamTypes_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            const string expectedAddDataToModelProgressText = "Adding data";
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var progressChangeNotifications = new List<ProgressNotification>();

            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath, updateDataStrategy, messageProvider);
            dikeProfilesImporter.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importResult = dikeProfilesImporter.Import();

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
                new ProgressNotification(expectedAddDataToModelProgressText, 1, 1)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.IsTrue(updateDataStrategy.Updated);
            Assert.AreEqual(5, updateDataStrategy.ReadDikeProfiles.Length);
        }

        [Test]
        public void Import_CancelOfImportWhileReadingProfileLocations_CancelImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";

            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Dijkprofielen")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath,
                                                                updateDataStrategy, messageProvider);
            dikeProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van profiellocaties uit een shapebestand."))
                {
                    dikeProfilesImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            var expectedLogMessage = new Tuple<string, LogLevelConstant>(cancelledLogMessage,
                                                                         LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            Assert.IsFalse(updateDataStrategy.Updated);
        }

        [Test]
        public void Import_CancelOfImportWhileReadingDikeProfileData_CancelImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Dijkprofielen")).Return(cancelledLogMessage);
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath,
                                                                updateDataStrategy, messageProvider);
            dikeProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van profielgegevens uit een prfl bestand."))
                {
                    dikeProfilesImporter.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import();

            // Assert
            var expectedLogMessage = new Tuple<string, LogLevelConstant>(cancelledLogMessage,
                                                                         LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            Assert.IsFalse(updateDataStrategy.Updated);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            var dikeProfilesImporter = new DikeProfilesImporter(new DikeProfileCollection(), referenceLine, filePath, updateDataStrategy, messageProvider);
            dikeProfilesImporter.SetProgressChanged((description, step, steps) => dikeProfilesImporter.Cancel());

            // Pre-condition
            bool importResult = dikeProfilesImporter.Import();
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(new DikeProfileCollection());

            dikeProfilesImporter.SetProgressChanged(null);

            // Call
            importResult = dikeProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.IsTrue(updateDataStrategy.Updated);
            Assert.AreEqual(5, updateDataStrategy.ReadDikeProfiles.Length);
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var updateDataStrategy = new TestDikeProfileUpdateStrategy();
            updateDataStrategy.UpdatedInstances = new[]
            {
                observableA,
                observableB
            };

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));
            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var importer = new DikeProfilesImporter(new DikeProfileCollection(),
                                                    referenceLine,
                                                    filePath,
                                                    updateDataStrategy, messageProvider);
            importer.Import();

            // Call
            importer.DoPostImport();

            // Assert
            // Asserts done in TearDown()
        }

        private static ReferenceLine CreateMatchingReferenceLine()
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

        private static void ValidateProgressMessages(List<ProgressNotification> expectedProgressMessages,
                                                     List<ProgressNotification> progressChangeNotifications)
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