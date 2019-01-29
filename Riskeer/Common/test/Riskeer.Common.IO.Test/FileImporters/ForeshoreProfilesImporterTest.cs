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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
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
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            var strategy = mockRepository.Stub<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            var strategy = mockRepository.Stub<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            var strategy = mockRepository.Stub<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            var strategy = mockRepository.Stub<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            mockRepository.ReplayAll();

            TestDelegate call = () => new ForeshoreProfilesImporter(new ForeshoreProfileCollection(), new ReferenceLine(), "path", null, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("foreshoreProfileUpdateStrategy", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_MessageProviderNull_ThrowArgumentNullException()
        {
            // Setup
            var strategy = mockRepository.Stub<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var foreshoreProfiles = new ForeshoreProfileCollection();

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);
            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
        public void Import_TwoForeshoreProfilesWithoutDamsAndGeometries_TrueAndLogWarning()
        {
            // Setup
            string fileDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "NoDamsAndNoForeshoreGeometries"));
            string filePath = Path.Combine(fileDirectory, "Voorlanden 12-2.shp");

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(filePath, invocation.Arguments[1]);

                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) invocation.Arguments[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                    });

            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            mockRepository.ReplayAll();

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

            // Call
            var importResult = false;
            Action call = () => importResult = foreshoreProfilesImporter.Import();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create(
                    $"Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel001NoForeshoreNoDam - Ringtoets.prfl")}' wordt overgeslagen.",
                    LogLevelConstant.Warn),
                Tuple.Create(
                    $"Profielgegevens definiëren geen dam en geen voorlandgeometrie. Bestand '{Path.Combine(fileDirectory, "profiel002NoForeshoreNoDam - Ringtoets.prfl")}' wordt overgeslagen.",
                    LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_FalseAndLogError()
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

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var messageProvider = mockRepository.StrictMock<IImporterMessageProvider>();
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);
            mockRepository.ReplayAll();

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(filePath, invocation.Arguments[1]);

                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) invocation.Arguments[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                    });

            const string expectedAddingDataToModelMessage = "Adding data to model";
            var messageProvider = mockRepository.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddingDataToModelMessage);
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);
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
                new ProgressNotification(expectedAddingDataToModelMessage, 1, 1)
            };
            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);
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
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();

            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(filePath, invocation.Arguments[1]);

                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) invocation.Arguments[0];
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
                    });

            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            mockRepository.ReplayAll();

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(filePath, invocation.Arguments[1]);

                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) invocation.Arguments[0];
                        Assert.AreEqual(5, readForeshoreProfiles.Count());
                    });

            const string expectedAddingDataToModelMessage = "Adding data to model";
            var messageProvider = mockRepository.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddingDataToModelMessage);
            mockRepository.ReplayAll();

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);

            const string cancelledLogMessage = "Operation cancelled";
            var messageProvider = mockRepository.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Voorlandprofielen")).Return(cancelledLogMessage);

            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);

            const string cancelledLogMessage = "Operation cancelled";
            var messageProvider = mockRepository.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Voorlandprofielen")).Return(cancelledLogMessage);
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            mockRepository.ReplayAll();

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(filePath, invocation.Arguments[1]);

                        var readForeshoreProfiles = (IEnumerable<ForeshoreProfile>) invocation.Arguments[0];
                        {
                            Assert.AreEqual(5, readForeshoreProfiles.Count());
                        }
                    });

            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            mockRepository.ReplayAll();

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
            var messageProvider = mockRepository.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return("");
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText("Dijkprofielen"))
                           .IgnoreArguments()
                           .Return("error {0}");

            const string exceptionMessage = "Look, an exception!";
            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .Throw(new UpdateDataException(exceptionMessage));
            mockRepository.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observableA = mockRepository.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mockRepository.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(referenceLine);

            var strategy = mockRepository.StrictMock<IForeshoreProfileUpdateDataStrategy>();
            var foreshoreProfiles = new ForeshoreProfileCollection();
            strategy.Expect(strat => strat.UpdateForeshoreProfilesWithImportedData(null, null))
                    .IgnoreArguments()
                    .Return(new[]
                    {
                        observableA,
                        observableB
                    });

            var messageProvider = mockRepository.Stub<IImporterMessageProvider>();
            mockRepository.ReplayAll();

            var foreshoreProfilesImporter = new ForeshoreProfilesImporter(foreshoreProfiles, referenceLine, filePath, strategy, messageProvider);

            foreshoreProfilesImporter.Import();

            // Call
            foreshoreProfilesImporter.DoPostImport();

            // Assert
            // Assertions are handled in the TearDown
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