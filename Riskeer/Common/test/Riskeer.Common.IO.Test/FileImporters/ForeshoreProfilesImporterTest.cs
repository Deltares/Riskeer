// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.TestUtil;

namespace Riskeer.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfilesImporterTest
    {
        [SetUp]
        public void SetUp() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            var importTarget = new ForeshoreProfileCollection();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new ForeshoreProfilesImporter(importTarget, referenceLine, "", strategy, messageProvider);

            // Assert
            Assert.IsInstanceOf<ProfilesImporter<ForeshoreProfileCollection>>(importer);
        }

        [Test]
        public void ParameteredConstructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(null, new ReferenceLine(), "", strategy, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), null, "", strategy, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), new ReferenceLine(), null, strategy, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_ForeshoreProfileUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Call
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), new ReferenceLine(), "path", null, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("foreshoreProfileUpdateStrategy", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_MessageProviderNull_ThrowArgumentNullException()
        {
            // Setup
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            // Call
            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), new ReferenceLine(), "path", strategy, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void Import_FromFileWithUnmatchableId_FalseAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var foreshoreProfiles = new ForeshoreProfileCollection();

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

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
        }

        [Test]
        public void Import_ThreeInvalidForeshoreProfileDefinitions_TrueAndLogWarning()
        {
            // Setup
            string fileDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("DikeProfiles", "NoDamsAndNoForeshoreGeometries"));
            string filePath = Path.Combine(fileDirectory, "Voorlanden 12-2.shp");

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(callInfo =>
                    {
                        Assert.AreSame(filePath, callInfo.Args()[1]);
                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) callInfo.Args()[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                        return new IObservable[]
                            {};
                    });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

            // Call
            var importResult = false;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create(
                    $"Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel001NoForeshoreNoDam.prfl")}' wordt overgeslagen.",
                    LogLevelConstant.Warn),
                Tuple.Create(
                    $"Profielgegevens definiëren geen geldige voorlandgeometrie. De voorlandgeometrie moet bestaan uit 0 of tenminste 2 punten. Bestand '{Path.Combine(fileDirectory, "profiel002ForeshoreOnePointNoDam.prfl")}' wordt overgeslagen.",
                    LogLevelConstant.Warn),
                Tuple.Create(
                    $"Profielgegevens definiëren geen geldige voorlandgeometrie. De voorlandgeometrie moet bestaan uit 0 of tenminste 2 punten. Bestand '{Path.Combine(fileDirectory, "profiel003ForeshoreOnePointWithDam.prfl")}' wordt overgeslagen.",
                    LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 4);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_FalseAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
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

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Fout bij het lezen van profiellocatie 5. De profiellocatie met " +
                                           "ID 'profiel005' ligt niet op de referentielijn.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_AllOkTestData_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(callInfo =>
                    {
                        Assert.AreSame(filePath, callInfo.Args()[1]);
                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) callInfo.Args()[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                        return new IObservable[]
                            {};
                    });

            const string expectedAddingDataToModelMessage = "Adding data to model";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddingDataToModelMessage);
            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);
            foreshoreProfilesImporter.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            var importResult = false;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
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
                new ProgressNotification(expectedAddingDataToModelMessage, 1, 1)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);
        }

        [Test]
        public void Import_AllOkTestData_CorrectForeshoreProfileProperties()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = Substitute.For<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();

            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(callInfo =>
                    {
                        Assert.AreSame(filePath, callInfo.Args()[1]);
                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) callInfo.Args()[0];
                        ForeshoreProfile[] readForeshoreProfilesArray = readForeshoreProfiles.ToArray();
                        Assert.AreEqual(5, readForeshoreProfilesArray.Length);

                        ForeshoreProfile foreshoreProfile1 = readForeshoreProfilesArray[0];
                        Assert.AreEqual("profiel001", foreshoreProfile1.Id);
                        Assert.AreEqual("profiel001", foreshoreProfile1.Name);

                        ForeshoreProfile foreshoreProfile2 = readForeshoreProfilesArray[1];
                        Assert.AreEqual("profiel002", foreshoreProfile2.Id);
                        Assert.AreEqual("profiel002", foreshoreProfile2.Name);

                        ForeshoreProfile foreshoreProfile3 = readForeshoreProfilesArray[2];
                        Assert.AreEqual("profiel003", foreshoreProfile3.Id);
                        Assert.AreEqual("profiel003", foreshoreProfile3.Name);

                        ForeshoreProfile foreshoreProfile4 = readForeshoreProfilesArray[3];
                        Assert.AreEqual(new Point2D(136432.12250000238, 538235.26300000318), foreshoreProfile4.WorldReferencePoint);
                        Assert.AreEqual("profiel004", foreshoreProfile4.Id);
                        Assert.AreEqual("Valide naam", foreshoreProfile4.Name);
                        Assert.AreEqual(-17.93475471, foreshoreProfile4.X0);
                        Assert.AreEqual(330.0, foreshoreProfile4.Orientation, foreshoreProfile4.Orientation.GetAccuracy());
                        Assert.IsFalse(foreshoreProfile4.HasBreakWater);

                        ForeshoreProfile foreshoreProfile5 = readForeshoreProfilesArray[4];
                        Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), foreshoreProfile5.WorldReferencePoint);
                        Assert.AreEqual("profiel005", foreshoreProfile5.Id);
                        Assert.AreEqual("Heeeeeeeeeeeeeeeeeeeeeeeele laaaaaaaaaaaaaaaaaaaange naaaaaaaaaaam", foreshoreProfile5.Name);
                        Assert.AreEqual(15.56165507, foreshoreProfile5.X0);
                        Assert.AreEqual(330.0, foreshoreProfile5.Orientation, foreshoreProfile5.Orientation.GetAccuracy());
                        Assert.IsTrue(foreshoreProfile5.HasBreakWater);
                        return new IObservable[]
                            {};
                    });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

            var targetContext = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);
            targetContext.Attach(observer);

            // Call
            foreshoreProfilesImporter.Import();

            // Assert
            // Assertions are handled in the TearDown
            // 'observer' should not be notified
        }

        [Test]
        public void Import_AllDamTypes_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            var observer = Substitute.For<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(callInfo =>
                    {
                        Assert.AreSame(filePath, callInfo.Args()[1]);
                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) callInfo.Args()[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                        return new IObservable[]
                            {};
                    });

            const string expectedAddingDataToModelMessage = "Adding data to model";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddingDataToModelMessage);
            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);
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
                new ProgressNotification(expectedAddingDataToModelMessage, 1, 1)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);
            // 'observer' should not be notified
        }

        [Test]
        public void Import_CancelOfImportWhileReadingProfileLocations_CancelsImportAndLogs()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);

            const string cancelledLogMessage = "Operation cancelled";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetCancelledLogMessageText("Voorlandprofielen").Returns(cancelledLogMessage);

            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine,
                                                                          filePath, strategy, messageProvider);
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
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage,
                                                                              LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhileReadingDikeProfileData_CancelImportAndLogInfo()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);

            const string cancelledLogMessage = "Operation cancelled";
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetCancelledLogMessageText("Voorlandprofielen").Returns(cancelledLogMessage);
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath,
                                                                          strategy, messageProvider);
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
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveForeshoreProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(callInfo =>
                    {
                        Assert.AreSame(filePath, callInfo.Args()[1]);
                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) callInfo.Args()[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                        return new IObservable[]
                            {};
                    });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath,
                                                                          strategy, messageProvider);
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
        }

        [Test]
        public void Import_ThrowsUpdateDataException_ReturnsFalseAndLogsError()
        {
            // Setup
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns("");
            messageProvider.GetUpdateDataFailedLogMessageText(Arg.Any<string>()).Returns("error {0}");

            const string exceptionMessage = "Look, an exception!";
            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(_ => throw new UpdateDataException(exceptionMessage));
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));
            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var importer = new ForeshoreProfilesImporter(new ForeshoreProfileCollection(),
                                                         referenceLine,
                                                         filePath, strategy, messageProvider);
            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string expectedMessage = $"error {exceptionMessage}";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observableA = Substitute.For<IObservable>();
            var observableB = Substitute.For<IObservable>();

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(referenceLine);

            var strategy = Substitute.For<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfiles = new ForeshoreProfileCollection();

            strategy.UpdateForeshoreProfilesWithImportedData(Arg.Any<IEnumerable<ForeshoreProfile>>(), Arg.Any<string>())
                    .Returns(new[]
                    {
                        observableA,
                        observableB
                    });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

            foreshoreProfilesImporter.Import();

            // Call
            foreshoreProfilesImporter.DoPostImport();

            // Assert
            observableA.Received().NotifyObservers();
            observableB.Received().NotifyObservers();
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
    }
}