// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test
{
    [TestFixture]
    public class OpenProjectActivityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var projectOwner = Substitute.For<IProjectOwner>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectStorage = Substitute.For<IStoreProject>();
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
            var projectOwner = Substitute.For<IProjectOwner>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectStorage = Substitute.For<IStoreProject>();
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
            const string expectedMessage = "File path should be set.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("requiredOpenProjectProperties", paramName);
        }

        [Test]
        public void Constructor_ProjectOwnerNull_ThrowArgumentException()
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectStorage = Substitute.For<IStoreProject>();
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
        }

        [Test]
        public void Constructor_ProjectFactoryNull_ThrowArgumentException()
        {
            // Setup
            var projectOwner = Substitute.For<IProjectOwner>();
            var projectStorage = Substitute.For<IStoreProject>();
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
        }

        [Test]
        public void Constructor_StoreProjectNull_ThrowArgumentException()
        {
            // Setup
            var projectOwner = Substitute.For<IProjectOwner>();
            var projectFactory = Substitute.For<IProjectFactory>();
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
        }

        [Test]
        public void Constructor_MigrationFilePathNull_ThrowArgumentException()
        {
            // Setup
            var projectOwner = Substitute.For<IProjectOwner>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
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
        }

        [Test]
        public void Constructor_MigrateProjectNull_ThrowArgumentException()
        {
            // Setup
            var projectOwner = Substitute.For<IProjectOwner>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectStorage = Substitute.For<IStoreProject>();
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
        }

        [Test]
        public void Run_StoreProjectLoadProjectDoesNotThrow_ActivityExecutedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            var project = Substitute.For<IProject>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns(project);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
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
        }

        [Test]
        public void Run_StoreProjectLoadProjectReturnsNull_ActivityFailedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns((IProject) null);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
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
        }

        [Test]
        public void Run_StoreProjectLoadProjectThrowsStorageException_ActivityFailedWithAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";

            const string message = "<some exception message>";
            var innerException = new Exception("A");
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns(x =>
                          {
                              throw new StorageException(message, innerException);
                          });

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
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
        }

        [Test]
        public void Run_StoreProjectLoadProjectThrowsArgumentException_ActivityFailedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns(x =>
                          {
                              throw new ArgumentException();
                          });

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
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
        }

        [Test]
        public void Run_SuccessfulMigrateAndLoadProject_ActivityExecutedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            const string someMigrationFilePath = "<path to some migrated file>";
            var project = Substitute.For<IProject>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someMigrationFilePath)
                          .Returns(project);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.Migrate(someFilePath, someMigrationFilePath)
                           .Returns(true);
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
        }

        [Test]
        public void Run_FailedToMigrate_ActivityFailedWithoutAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            const string someMigrationFilePath = "<path to some migrated file>";
            var project = Substitute.For<IProject>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someMigrationFilePath)
                          .Returns(project);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.Migrate(someFilePath, someMigrationFilePath)
                           .Returns(false);
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
        }

        [Test]
        public void Run_MigrateThrowsArgumentException_ActivityFailedWithAdditionalLogMessages()
        {
            // Setup
            const string someFilePath = "<path to some file>";
            const string someMigrationFilePath = "<path to some migrated file>";
            const string exceptionMessage = "<some exception message>";
            var project = Substitute.For<IProject>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someMigrationFilePath)
                          .Returns(project);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.Migrate(someFilePath, someMigrationFilePath)
                           .Returns(x =>
                           {
                               throw new ArgumentException(exceptionMessage);
                           });
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
        }

        [Test]
        public void Run_WithMigration_ExpectedProgressNotifications()
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
            var migrateProject = Substitute.For<IMigrateProject>();
            migrateProject.Migrate(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(true);
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
        }

        [Test]
        public void Run_WithoutMigration_ExpectedProgressNotifications()
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
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
        }

        [Test]
        public void GivenSuccessfullyExecutedOpenProjectActivity_WhenFinishingOpenProjectActivity_ThenProjectOwnerAndNewProjectUpdatedWithLogMessage()
        {
            // Given
            const string someFilePath = @"c:\\folder\someFilePath.rtd";
            var project = Substitute.For<IProject>();
            project.NotifyObservers();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns(project);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.SetProject(project, someFilePath);
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
        }

        [Test]
        public void GivenOpenProjectActivityAndFailedDueToNoProject_WhenFinishingOpenProjectActivity_ThenProjectSetToNullAndMessageLogged()
        {
            // Given
            const string someFilePath = @"c:\\folder\someFilePath.rtd";
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns((IProject) null);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.SetProject(null, null);
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
        }

        [Test]
        [TestCaseSource(nameof(ExceptionCases))]
        public void GivenOpenProjectActivityFailedDueToException_WhenFinishingOpenProjectActivity_ThenProjectOwnerHasNullProjectWithLogMessage(Exception exceptionToThrow)
        {
            // Given
            const string someFilePath = @"c:\\folder\someFilePath.rtd";
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns(x =>
                          {
                              throw exceptionToThrow;
                          });

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.SetProject(null, null);
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
            var expectedMessage = Tuple.Create("Openen van project is mislukt.",
                                               LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void LogState_ActivityCancelled_ProjectOwnerNotUpdatedWithLogMessage()
        {
            // Setup
            const string someFilePath = @"c:\\folder\someFilePath.rtd";
            var project = Substitute.For<IProject>();
            project.DidNotReceive().NotifyObservers();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(someFilePath)
                          .Returns(project);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.DidNotReceive().SetProject(project, someFilePath);

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
        }

        [Test]
        public void Finish_ProjectMigratedAndOpened_ExpectedProgressText()
        {
            // Setup
            var project = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
            storeProject.LoadProject(Arg.Any<string>()).Returns(project);
            var migrateProject = Substitute.For<IMigrateProject>();
            migrateProject.Migrate(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
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
        }

        [Test]
        public void Finish_OnlyProjectOpened_ExpectedProgressText()
        {
            // Setup
            var project = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
            storeProject.LoadProject(Arg.Any<string>()).Returns(project);
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
        }

        [Test]
        public void Finish_ProjectMigrationFailed_ProjectSetToNull()
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
            var migrateProject = Substitute.For<IMigrateProject>();
            migrateProject.Migrate(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
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

            activity.ProgressChanged += (sender, args) =>
            {
                Assert.AreSame(activity, sender);
                Assert.AreEqual(EventArgs.Empty, args);
            };

            // Call
            activity.Finish();

            // Assert
            projectOwner.Received(1).SetProject(null, null);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivityMigratingAndOpeningProject_WhenCancellingDuringMigration_ThenDoNotLoadProject(bool migrationSuccessful)
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var storeProject = Substitute.For<IStoreProject>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var migrateProject = Substitute.For<IMigrateProject>();
            migrateProject.Migrate(Arg.Any<string>(), Arg.Any<string>()).Returns(migrationSuccessful);
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
        }

        [Test]
        public void GivenActivityMigrationProject_WhenCancellingAndMigrationThrowsException_ThenDoNotLoadProject()
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var storeProject = Substitute.For<IStoreProject>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var migrateProject = Substitute.For<IMigrateProject>();
            migrateProject.When(x => x.Migrate(Arg.Any<string>(), Arg.Any<string>())).Do(x =>
            {
                throw new ArgumentException();
            });
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenActivityOpeningProject_WhenCancellingDuringLoadProject_ThenDoNotSetProject(bool loadProjectSuccessful)
        {
            // Setup
            IProject project = loadProjectSuccessful ? Substitute.For<IProject>() : null;
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
            storeProject.LoadProject(Arg.Any<string>()).Returns(project);
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
        }

        [Test]
        public void GivenActivityOpeningProject_WhenCancellingWhileLoadProjectThrowsStorageException_ThenDoNotSetProject()
        {
            // Setup
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var storeProject = Substitute.For<IStoreProject>();
            storeProject.When(x => x.LoadProject(Arg.Any<string>())).Do(x =>
            {
                throw new StorageException();
            });
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
        }

        private static IEnumerable<TestCaseData> ExceptionCases()
        {
            yield return new TestCaseData(new StorageException());
            yield return new TestCaseData(new ArgumentException());
        }
    }
}