// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class SaveProjectActivityTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(bool savingExistingProject)
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            var activity = new SaveProjectActivity(project, "", savingExistingProject, storeProject, projectOwner);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);

            string exitingPrefix = savingExistingProject ? "bestaand " : "";
            string expectedName = $"Opslaan van {exitingPrefix}project";
            Assert.AreEqual(expectedName, activity.Description);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SaveProjectActivity(project, null, true, storeProject, projectOwner);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ProjectNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SaveProjectActivity(null, "", true, storeProject, projectOwner);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("project", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_StoreProjectNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SaveProjectActivity(project, "", true, null, projectOwner);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("storeProject", paramName);
        }

        [Test]
        public void Constructor_ProjectOwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var storeProject = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SaveProjectActivity(project, "", true, storeProject, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("projectOwner", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_UnstagedProject_StageAndSaveProjectWithoutAdditionalLogMessages(bool saveExistingProject)
        {
            // Setup
            const string filePath = "A";
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(false);
            using (mocks.Ordered())
            {
                storeProject.Expect(sp => sp.StageProject(project));
                storeProject.Expect(sp => sp.SaveProjectAs(filePath));
            }

            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);

            // Call
            Action call = () => activity.Run();

            // Assert
            string prefix = saveExistingProject ? "bestaand " : "";
            TestHelper.AssertLogMessageIsGenerated(call, $"Opslaan van {prefix}project is gestart.", 1);
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_AlreadyStagedProject_SaveProjectWithoutAdditionalLogMessages(bool saveExistingProject)
        {
            // Setup
            const string filePath = "A";
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(true);
            storeProject.Expect(sp => sp.SaveProjectAs(filePath));
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);

            // Call
            Action call = () => activity.Run();

            // Assert
            string prefix = saveExistingProject ? "bestaand " : "";
            TestHelper.AssertLogMessageIsGenerated(call, $"Opslaan van {prefix}project is gestart.", 1);
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Run_SaveProjectAsThrowsException_FailedWithAdditionalLogMessages(Exception exception, string errorMessage)
        {
            // Setup
            const string filePath = "A";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(true);
            storeProject.Stub(sp => sp.SaveProjectAs(filePath))
                        .Throw(exception);
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, false, storeProject, projectOwner);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call,
                                                              new[]
                                                              {
                                                                  Tuple.Create("Opslaan van project is gestart.", LogLevelConstant.Info),
                                                                  Tuple.Create(errorMessage, LogLevelConstant.Error)
                                                              }, 2);
            Assert.AreEqual(ActivityState.Failed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetExceptions))]
        public void Run_SaveExistingProjectAsThrowsException_FailedWithAdditionalLogMessages(Exception exception, string errorMessage)
        {
            // Setup
            const string filePath = "A";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(true);
            storeProject.Stub(sp => sp.SaveProjectAs(filePath))
                        .Throw(exception);
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, true, storeProject, projectOwner);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call,
                                                              new[]
                                                              {
                                                                  Tuple.Create("Opslaan van bestaand project is gestart.", LogLevelConstant.Info),
                                                                  Tuple.Create(errorMessage, LogLevelConstant.Error)
                                                              }, 2);
            Assert.AreEqual(ActivityState.Failed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_UnstagedProject_ExpectedProgressMessages(bool saveExistingProject)
        {
            // Setup
            const string filePath = "A";
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(false);
            using (mocks.Ordered())
            {
                storeProject.Expect(sp => sp.StageProject(project));
                storeProject.Expect(sp => sp.SaveProjectAs(filePath));
            }

            mocks.ReplayAll();

            var progressMessages = new List<string>();

            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);
            activity.ProgressChanged += (sender, args) =>
            {
                Assert.AreSame(activity, sender);
                Assert.AreEqual(EventArgs.Empty, args);

                progressMessages.Add(activity.ProgressText);
            };

            // Call
            activity.Run();

            // Assert
            int totalSteps = saveExistingProject ? 2 : 3;
            string[] expectedProgressMessages =
            {
                $"Stap 1 van {totalSteps} | Voorbereidingen opslaan",
                $"Stap 2 van {totalSteps} | Project opslaan"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_AlreadyStagedProject_ExpectedProgressMessages(bool saveExistingProject)
        {
            // Setup
            const string filePath = "A";
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(true);
            storeProject.Expect(sp => sp.SaveProjectAs(filePath));
            mocks.ReplayAll();

            var progressMessages = new List<string>();

            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);
            activity.ProgressChanged += (sender, args) =>
            {
                Assert.AreSame(activity, sender);
                Assert.AreEqual(EventArgs.Empty, args);

                progressMessages.Add(activity.ProgressText);
            };

            // Call
            activity.Run();

            // Assert
            int totalSteps = saveExistingProject ? 1 : 2;
            string[] expectedProgressMessages =
            {
                $"Stap 1 van {totalSteps} | Project opslaan"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenSaveProjectActivityAndSuccessfullySavedNewProject_WhenFinishingSaveProjectActivity_ThenUpdateProjectAndProjectOwnerWithMessage()
        {
            // Given
            const string fileName = "A";
            string filePath = $@"C:\\folder\{fileName}.rtd";

            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();

            var project = mocks.Stub<IProject>();
            project.Expect(p => p.NotifyObservers());

            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Expect(po => po.SetProject(project, filePath));
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, false, storeProject, projectOwner);
            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            // When
            Action call = () =>
            {
                activity.LogState();
                activity.Finish();
            };

            // Then
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Opslaan van project is gelukt.",
                                                                           LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.AreEqual(ActivityState.Finished, activity.State);
            Assert.AreEqual(fileName, project.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenSaveProjectActivityAndSuccessfullySavedExistingProject_WhenFinishingSaveProjectActivity_ThenDoNotUpdateProjectAndProjectOwnerWithMessage()
        {
            // Given
            const string fileName = "A";
            string filePath = $@"C:\\folder\{fileName}.rtd";

            var mocks = new MockRepository();
            var storeProject = mocks.Stub<IStoreProject>();
            var project = mocks.StrictMock<IProject>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, true, storeProject, projectOwner);
            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            // When
            Action call = () =>
            {
                activity.LogState();
                activity.Finish();
            };

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Opslaan van bestaand project is gelukt.",
                                                                           LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.AreEqual(ActivityState.Finished, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Finish_SuccessfullySavedNewProject_ExpectedProgressMessages(bool hasStagedProject)
        {
            // Setup
            const string fileName = "A";
            string filePath = $@"C:\\folder\{fileName}.rtd";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject).Return(hasStagedProject);
            storeProject.Stub(sp => sp.StageProject(project));
            storeProject.Stub(sp => sp.SaveProjectAs(filePath));

            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, false, storeProject, projectOwner);
            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            var progressMessages = new List<string>();
            activity.ProgressChanged += (sender, args) =>
            {
                Assert.AreSame(activity, sender);
                Assert.AreEqual(EventArgs.Empty, args);

                progressMessages.Add(activity.ProgressText);
            };

            // Call
            activity.Finish();

            // Assert
            int totalSteps = hasStagedProject ? 2 : 3;
            string[] expectedProgressMessages =
            {
                $"Stap {totalSteps} van {totalSteps} | Initialiseren van opgeslagen project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivityStagingProject_WhenCancelling_ThenProjectNotSavedWithAdditionalLogMessages(bool saveExistingProject)
        {
            // Given
            const string filePath = "A";
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(false);
            using (mocks.Ordered())
            {
                storeProject.Expect(sp => sp.StageProject(project));
                storeProject.Expect(sp => sp.SaveProjectAs(filePath))
                            .Repeat.Never();
            }

            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);
            activity.ProgressChanged += (sender, args) => activity.Cancel();

            // When
            Action call = () =>
            {
                activity.Run(); // Cancel called mid-progress
                activity.LogState();
                activity.Finish();
            };

            // Then
            string prefix = saveExistingProject ? "bestaand " : "";
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call,
                                                              new[]
                                                              {
                                                                  Tuple.Create($"Opslaan van {prefix}project is gestart.",
                                                                               LogLevelConstant.Info),
                                                                  Tuple.Create($"Opslaan van {prefix}project is geannuleerd.",
                                                                               LogLevelConstant.Warn)
                                                              }, 2);

            Assert.AreEqual(ActivityState.Canceled, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivitySavingStagedProject_WhenCancelling_ThenProjectSavedWithAdditionalLogMessage(bool saveExistingProject)
        {
            // Given
            const string filePath = "A";
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            if (!saveExistingProject)
            {
                projectOwner.Expect(po => po.SetProject(project, filePath));
            }

            var storeProject = mocks.StrictMock<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(true);
            storeProject.Expect(sp => sp.SaveProjectAs(filePath));
            mocks.ReplayAll();

            var calledCancel = false;
            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);
            activity.ProgressChanged += (sender, args) =>
            {
                if (calledCancel)
                {
                    activity.Cancel();
                    calledCancel = true;
                }
            };

            // When
            Action call = () =>
            {
                activity.Run(); // Cancel called mid-progress but beyond 'point of no return'
                activity.LogState();
                activity.Finish();
            };

            // Then
            string prefix = saveExistingProject ? "bestaand " : "";
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call,
                                                              new[]
                                                              {
                                                                  Tuple.Create($"Opslaan van {prefix}project is gestart.",
                                                                               LogLevelConstant.Info),
                                                                  Tuple.Create($"Opslaan van {prefix}project is gelukt.",
                                                                               LogLevelConstant.Info)
                                                              }, 2);

            Assert.AreEqual(ActivityState.Finished, activity.State);
            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetExceptions()
        {
            const string exceptionMessage = "I am an error message";

            yield return new TestCaseData(new ArgumentException(exceptionMessage), exceptionMessage)
                .SetName("ArgumentException");
            yield return new TestCaseData(new CouldNotConnectException(exceptionMessage), exceptionMessage)
                .SetName("CouldNotConnectException");
            yield return new TestCaseData(new StorageValidationException(exceptionMessage), exceptionMessage)
                .SetName("StorageValidationException");
        }
    }
}