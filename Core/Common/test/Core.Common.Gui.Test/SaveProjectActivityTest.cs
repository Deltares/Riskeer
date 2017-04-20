// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.ComponentModel;
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
            CollectionAssert.IsEmpty(activity.LogMessages);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);

            string exitingPrefix = savingExistingProject ? "bestaand " : "";
            string expectedName = $"Opslaan van {exitingPrefix}project";
            Assert.AreEqual(expectedName, activity.Name);

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
        public void Run_UnstagedProject_StageAndSaveProjectWithoutLogMessages(bool saveExistingProject)
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
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_AlreadyStagedProject_SaveProjectWithoutLogMessages(bool saveExistingProject)
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
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Run_SaveProjectAsThrowsException_FailedWithLogMessage(
            [Values(true, false)] bool saveExistingProject,
            [Values(SaveProjectAsExceptionType.StorageException,
                SaveProjectAsExceptionType.CouldNotConnectException,
                SaveProjectAsExceptionType.ArgumentException)] SaveProjectAsExceptionType exceptionType)
        {
            // Setup
            const string filePath = "A";
            const string message = "<error message>";

            Exception exception = CreateException(exceptionType, message);

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(sp => sp.HasStagedProject)
                        .Return(true);
            storeProject.Stub(sp => sp.SaveProjectAs(filePath))
                        .Throw(exception);
            mocks.ReplayAll();

            var activity = new SaveProjectActivity(project, filePath, saveExistingProject, storeProject, projectOwner);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error), 1);
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
            var expectedProgressMessages = new[]
            {
                $"Stap 1 van {totalSteps} | Voorbereiding opslaan",
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
            var expectedProgressMessages = new[]
            {
                $"Stap 1 van {totalSteps} | Project opslaan"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_SuccessfullySavedNewProject_UpdateProjectAndProjectOwnerWithMessage()
        {
            // Setup
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

            // Call
            Action call = () => activity.Finish();

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Uitvoeren van 'Opslaan van project' is gelukt.",
                                                                           LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.AreEqual(ActivityState.Finished, activity.State);
            Assert.AreEqual(fileName, project.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_SuccessfullySavedExistingProject_DoNotUpdateProjectAndProjectOwnerWithMessage()
        {
            // Setup
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

            // Call
            Action call = () => activity.Finish();

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Uitvoeren van 'Opslaan van bestaand project' is gelukt.",
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
            var expectedProgressMessages = new[]
            {
                $"Stap {totalSteps} van {totalSteps} | Opgeslagen project initialiseren"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivityStagingProject_WhenCancelling_ThenProjectNotSavedWithLogMessage(bool saveExistingProject)
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
                activity.Finish();
            };

            // Then
            string prefix = saveExistingProject ? "bestaand " : "";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call,
                                                            Tuple.Create($"Uitvoeren van 'Opslaan van {prefix}project' is geannuleerd.",
                                                                         LogLevelConstant.Warn),
                                                            1);

            Assert.AreEqual(ActivityState.Canceled, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivitySavingStagedProject_WhenCancelling_ThenProjectSavedWithLogMessage(bool saveExistingProject)
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
                activity.Finish();
            };

            // Then
            string prefix = saveExistingProject ? "bestaand " : "";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call,
                                                            Tuple.Create($"Uitvoeren van 'Opslaan van {prefix}project' is gelukt.",
                                                                         LogLevelConstant.Info),
                                                            1);

            Assert.AreEqual(ActivityState.Finished, activity.State);
            mocks.VerifyAll();
        }

        private static Exception CreateException(SaveProjectAsExceptionType exceptionType, string message)
        {
            switch (exceptionType)
            {
                case SaveProjectAsExceptionType.ArgumentException:
                    return new ArgumentException(message);
                case SaveProjectAsExceptionType.CouldNotConnectException:
                    return new CouldNotConnectException(message);
                case SaveProjectAsExceptionType.StorageException:
                    return new StorageException(message);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public enum SaveProjectAsExceptionType
        {
            CouldNotConnectException,
            StorageException,
            ArgumentException
        }
    }
}