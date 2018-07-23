// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class OpenProjectActivityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            // Call
            var activity = new OpenProjectActivity(openProjectProperties);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual("Openen van project", activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_OpenProjectConstructionPropertiesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new OpenProjectActivity(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("requiredOpenProjectProperties", paramName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = null,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            // Call
            TestDelegate call = () => new OpenProjectActivity(openProjectProperties);

            // Assert
            const string expectedMessage = "Filepath should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("requiredOpenProjectProperties", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ProjectOwnerNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectOwner = null,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            // Call
            TestDelegate call = () => new OpenProjectActivity(openProjectProperties);

            // Assert
            const string expectedMessage = "Project owner should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("requiredOpenProjectProperties", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ProjectFactoryNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var projectStorage = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectOwner = projectOwner,
                ProjectFactory = null,
                ProjectStorage = projectStorage
            };

            // Call
            TestDelegate call = () => new OpenProjectActivity(openProjectProperties);

            // Assert
            const string expectedMessage = "Project factory should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("requiredOpenProjectProperties", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_StoreProjectNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = null
            };

            // Call
            TestDelegate call = () => new OpenProjectActivity(openProjectProperties);

            // Assert
            const string expectedMessage = "Project storage should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("requiredOpenProjectProperties", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MigrationFilePathNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = null,
                Migrator = projectMigrator
            };

            // Call
            TestDelegate call = () => new OpenProjectActivity(openProjectProperties, migrateProjectProperties);

            // Assert
            const string expectedMessage = "Migration target file path should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("optionalProjectMigrationProperties", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MigrateProjectNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = "",
                Migrator = null
            };

            // Call
            TestDelegate call = () => new OpenProjectActivity(openProjectProperties, migrateProjectProperties);

            // Assert
            const string expectedMessage = "Project migrator should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("optionalProjectMigrationProperties", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectDoesNotThrow_ActivityExecutedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someFilePath))
                          .Return(project);

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Openen van project is gestart.", 1);

            Assert.AreEqual(ActivityState.Executed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectReturnsNull_ActivityFailedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";

            var mocks = new MockRepository();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someFilePath))
                          .Return(null);

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Openen van project is gestart.", 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectThrowsStorageException_ActivityFailedWithAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";

            const string message = "<some exception message>";
            var innerException = new Exception("A");

            var mocks = new MockRepository();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someFilePath))
                          .Throw(new StorageException(message, innerException));

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                "Openen van project is gestart.",
                message
            }, 2);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectThrowsArgumentException_ActivityFailedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";

            var mocks = new MockRepository();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someFilePath))
                          .Throw(new ArgumentException());

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Openen van project is gestart.", 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_SuccessfulMigrateAndLoadProject_ActivityExecutedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            const string someMigrationFilePath = "<path to some migrated file>";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someMigrationFilePath))
                          .Return(project);

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Expect(m => m.Migrate(someFilePath, someMigrationFilePath))
                           .Return(true);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = someMigrationFilePath,
                Migrator = projectMigrator
            };

            var activity = new OpenProjectActivity(openProjectProperties, migrateProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Openen van project is gestart.", 1);

            Assert.AreEqual(ActivityState.Executed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_FailedToMigrate_ActivityFailedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            const string someMigrationFilePath = "<path to some migrated file>";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someMigrationFilePath))
                          .Return(project)
                          .Repeat.Never();

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Expect(m => m.Migrate(someFilePath, someMigrationFilePath))
                           .Return(false);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = someMigrationFilePath,
                Migrator = projectMigrator
            };

            var activity = new OpenProjectActivity(openProjectProperties, migrateProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Openen van project is gestart.", 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_MigrateThrowsArgumentException_ActivityFailedWithAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            const string someMigrationFilePath = "<path to some migrated file>";
            const string exceptionMessage = "<some exception message>";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.LoadProject(someMigrationFilePath))
                          .Return(project)
                          .Repeat.Never();

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            projectMigrator.Expect(m => m.Migrate(someFilePath, someMigrationFilePath))
                           .Throw(new ArgumentException(exceptionMessage));
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = someMigrationFilePath,
                Migrator = projectMigrator
            };

            var activity = new OpenProjectActivity(openProjectProperties, migrateProjectProperties);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                "Openen van project is gestart.",
                exceptionMessage
            }, 2);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_WithMigration_ExpectedProgressNotifications()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            var migrateProject = mocks.Stub<IMigrateProject>();
            migrateProject.Stub(pm => pm.Migrate(null, null))
                          .IgnoreArguments()
                          .Return(true);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectOwner = projectOwner,
                ProjectStorage = storeProject
            };
            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = "",
                Migrator = migrateProject
            };
            var activity = new OpenProjectActivity(openProjectProperties,
                                                   migrateProjectProperties);

            var progressMessages = new List<string>();
            activity.ProgressChanged += (sender, args) =>
            {
                Assert.AreSame(activity, sender);
                Assert.AreEqual(EventArgs.Empty, args);

                progressMessages.Add(activity.ProgressText);
            };

            // Call
            activity.Run();

            // Assert
            var expectedProgressMessages = new[]
            {
                "Stap 1 van 3 | Migreren van project",
                "Stap 2 van 3 | Inlezen van project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_WithoutMigration_ExpectedProgressNotifications()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectOwner = projectOwner,
                ProjectStorage = storeProject
            };
            var activity = new OpenProjectActivity(openProjectProperties);

            var progressMessages = new List<string>();
            activity.ProgressChanged += (sender, args) =>
            {
                Assert.AreSame(activity, sender);
                Assert.AreEqual(EventArgs.Empty, args);

                progressMessages.Add(activity.ProgressText);
            };

            // Call
            activity.Run();

            // Assert
            var expectedProgressMessages = new[]
            {
                "Stap 1 van 2 | Inlezen van project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenSuccessfullyExecutedOpenProjectActivity_WhenFinishingOpenProjectActivity_ThenProjectOwnerAndNewProjectUpdatedWithLogMessage()
        {
            // Given
            const string someFilePath = @"c:\\folder\someFilePath.rtd";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            project.Expect(p => p.NotifyObservers());

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(someFilePath))
                          .Return(project);

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Expect(po => po.SetProject(project, someFilePath));
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

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
            const string expectedMessage = "Openen van project is gelukt.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Finished, activity.State);

            Assert.AreEqual(Path.GetFileNameWithoutExtension(someFilePath), project.Name);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenOpenProjectActivityAndFailedDueToNoProject_WhenFinishingOpenProjectActivity_ThenProjectOwnerHasNewEmptyProjectWithLogMessage()
        {
            // Given
            const string someFilePath = @"c:\\folder\someFilePath.rtd";

            var mocks = new MockRepository();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(someFilePath))
                          .Return(null);

            var project = mocks.Stub<IProject>();

            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Expect(f => f.CreateNewProject())
                          .Return(project);
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Expect(po => po.SetProject(project, null));
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Failed, activity.State);

            // When
            Action call = () =>
            {
                activity.LogState();
                activity.Finish();
            };

            // Then
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Openen van project is mislukt.",
                                                                           LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(ExceptionCases))]
        public void GivenOpenProjectActivityFailedDueToException_WhenFinishingOpenProjectActivity_ThenProjectOwnerHasNewEmptyProjectWithLogMessage(Exception exceptionToThrow)
        {
            // Given
            const string someFilePath = @"c:\\folder\someFilePath.rtd";

            var mocks = new MockRepository();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(someFilePath))
                          .Throw(exceptionToThrow);

            var project = mocks.Stub<IProject>();

            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Expect(f => f.CreateNewProject())
                          .Return(project);
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Expect(po => po.SetProject(project, null));
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Failed, activity.State);

            // When
            Action call = () =>
            {
                activity.LogState();
                activity.Finish();
            };

            // Then
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Openen van project is mislukt.",
                                                                           LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void LogState_ActivityCancelled_ProjectOwnerNotUpdatedWithLogMessage()
        {
            // Setup
            const string someFilePath = @"c:\\folder\someFilePath.rtd";

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            project.Expect(p => p.NotifyObservers())
                   .Repeat.Never();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(someFilePath))
                          .Return(project);

            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Expect(po => po.SetProject(project, someFilePath))
                        .Repeat.Never();
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = someFilePath,
                ProjectOwner = projectOwner,
                ProjectFactory = projectFactory,
                ProjectStorage = projectStorage
            };

            var activity = new OpenProjectActivity(openProjectProperties);

            activity.Run();
            activity.Cancel();

            // Precondition
            Assert.AreEqual(ActivityState.Canceled, activity.State);

            // Call
            Action call = () => activity.LogState();

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create("Openen van project is geannuleerd.",
                                                                           LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Canceled, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Finish_ProjectMigratedAndOpened_ExpectedProgressText()
        {
            // Setup
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(project);
            var migrateProject = mocks.Stub<IMigrateProject>();
            migrateProject.Stub(pm => pm.Migrate(null, null))
                          .IgnoreArguments()
                          .Return(true);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectOwner = projectOwner,
                ProjectStorage = storeProject
            };
            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = "",
                Migrator = migrateProject
            };

            var activity = new OpenProjectActivity(openProjectProperties,
                                                   migrateProjectProperties);
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
            var expectedProgressMessages = new[]
            {
                "Stap 3 van 3 | Initialiseren van geopend project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);

            mocks.VerifyAll();
        }

        [Test]
        public void Finish_OnlyProjectOpened_ExpectedProgressText()
        {
            // Setup
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(sp => sp.LoadProject(null))
                        .IgnoreArguments()
                        .Return(project);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectOwner = projectOwner,
                ProjectStorage = storeProject
            };

            var activity = new OpenProjectActivity(openProjectProperties);
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
            var expectedProgressMessages = new[]
            {
                "Stap 2 van 2 | Initialiseren van geopend project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);

            mocks.VerifyAll();
        }

        [Test]
        public void Finish_ProjectMigrationFailed_ExpectedProgressText()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            var migrateProject = mocks.Stub<IMigrateProject>();
            migrateProject.Stub(pm => pm.Migrate(null, null))
                          .IgnoreArguments()
                          .Throw(new ArgumentException());
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectOwner = projectOwner,
                ProjectStorage = storeProject
            };
            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = "",
                Migrator = migrateProject
            };

            var activity = new OpenProjectActivity(openProjectProperties,
                                                   migrateProjectProperties);
            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Failed, activity.State);

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
            var expectedProgressMessages = new[]
            {
                "Stap 3 van 3 | Initialiseren van leeg project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);

            mocks.VerifyAll();
        }

        [Test]
        public void Finish_OnlyProjectOpenFailed_ExpectedProgressText()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(s => s.LoadProject(null))
                        .IgnoreArguments()
                        .Throw(new StorageException());
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectOwner = projectOwner,
                ProjectStorage = storeProject
            };

            var activity = new OpenProjectActivity(openProjectProperties);
            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Failed, activity.State);

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
            var expectedProgressMessages = new[]
            {
                "Stap 2 van 2 | Initialiseren van leeg project"
            };
            CollectionAssert.AreEqual(expectedProgressMessages, progressMessages);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivityMigratingAndOpeningProject_WhenCancellingDuringMigration_DoNotLoadProject(bool migrationSuccessful)
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            var migrateProject = mocks.Stub<IMigrateProject>();
            migrateProject.Stub(mp => mp.Migrate(null, null))
                          .IgnoreArguments()
                          .Return(migrationSuccessful);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectStorage = storeProject,
                ProjectOwner = projectOwner
            };
            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = "",
                Migrator = migrateProject
            };
            var activity = new OpenProjectActivity(openProjectProperties,
                                                   migrateProjectProperties);

            // When
            activity.ProgressChanged += (sender, args) => activity.Cancel();
            activity.Run();
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenActivityMigratinProject_WhenCancellingAndMigrationThrowsException_DoNotLoadProject()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            var storeProject = mocks.StrictMock<IStoreProject>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            var migrateProject = mocks.Stub<IMigrateProject>();
            migrateProject.Stub(mp => mp.Migrate(null, null))
                          .IgnoreArguments()
                          .Throw(new ArgumentException());
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectStorage = storeProject,
                ProjectOwner = projectOwner
            };
            var migrateProjectProperties = new OpenProjectActivity.ProjectMigrationConstructionProperties
            {
                MigrationFilePath = "",
                Migrator = migrateProject
            };
            var activity = new OpenProjectActivity(openProjectProperties,
                                                   migrateProjectProperties);

            // When
            activity.ProgressChanged += (sender, args) => activity.Cancel();
            activity.Run();
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivityOpeningProject_WhenCancellingDuringLoadProject_DoNotSetProject(bool loadProjectSuccessful)
        {
            // Setup
            var mocks = new MockRepository();
            IProject project = loadProjectSuccessful ? mocks.Stub<IProject>() : null;
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(s => s.LoadProject(null))
                        .IgnoreArguments()
                        .Return(project);
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectStorage = storeProject,
                ProjectOwner = projectOwner
            };
            var activity = new OpenProjectActivity(openProjectProperties);

            // When
            activity.ProgressChanged += (sender, args) => activity.Cancel();
            activity.Run();
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenActivityOpeningProject_WhenCancellingWhileLoadProjectThrowsStorageException_DoNotSetProject()
        {
            // Setup
            var mocks = new MockRepository();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            var storeProject = mocks.Stub<IStoreProject>();
            storeProject.Stub(s => s.LoadProject(null))
                        .IgnoreArguments()
                        .Throw(new StorageException());
            mocks.ReplayAll();

            var openProjectProperties = new OpenProjectActivity.OpenProjectConstructionProperties
            {
                FilePath = "",
                ProjectFactory = projectFactory,
                ProjectStorage = storeProject,
                ProjectOwner = projectOwner
            };
            var activity = new OpenProjectActivity(openProjectProperties);

            // When
            activity.ProgressChanged += (sender, args) => activity.Cancel();
            activity.Run();
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> ExceptionCases()
        {
            yield return new TestCaseData(new StorageException());
            yield return new TestCaseData(new ArgumentException());
        }
    }
}