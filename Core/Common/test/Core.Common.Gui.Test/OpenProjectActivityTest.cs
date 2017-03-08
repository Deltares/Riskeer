﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
            CollectionAssert.IsEmpty(activity.LogMessages);
            Assert.AreEqual("Openen van bestaand project", activity.Name);
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
        public void Constructor_MigrateProjecthNull_ThrowArgumentException()
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
        public void Run_StoreProjectLoadProjectDoesNotThrow_ActivityExecutedWithoutLogMessages()
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
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(ActivityState.Executed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectReturnsNull_ActivityFailedWithoutLogMessages()
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
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectThrowsStorageException_ActivityFailedWithLogMessage()
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
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_StoreProjectLoadProjectThrowsArgumentException_ActivityFailedWithoutLogMessages()
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
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }




        [Test]
        public void Run_SuccessfulMigrateAndLoadProject_ActivityExecutedWithoutLogMessages()
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
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(ActivityState.Executed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_FailedToMigrate_ActivityFailedWithoutLogMessages()
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
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_MigrateThrowsArgumentException_ActivityFailedWithLogMessage()
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
            TestHelper.AssertLogMessageIsGenerated(call, exceptionMessage, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }




        [Test]
        public void Finish_ActivityExecutedSuccessfully_ProjectOwnerAndNewProjectUpdatedWithLogMessage()
        {
            // Setup
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

            // Call
            Action call = () => activity.Finish();

            // Assert
            const string expectedMessage = "Uitvoeren van 'Openen van bestaand project' is gelukt.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Finished, activity.State);

            Assert.AreEqual(Path.GetFileNameWithoutExtension(someFilePath), project.Name);

            mocks.VerifyAll();
        }

        [Test]
        public void Finish_ActivityFailedDueToNoProject_ProjectOwnerHasNewEmptyProjectWithLogMessage()
        {
            // Setup
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

            // Call
            Action call = () => activity.Finish();

            // Assert
            const string expectedMessage = "Uitvoeren van 'Openen van bestaand project' is mislukt.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Finish_ActivityFailedDueToException_ProjectOwnerHasNewEmptyProjectWithLogMessage(bool trueForStorageExceptionFalseForArgumentException)
        {
            // Setup
            const string someFilePath = @"c:\\folder\someFilePath.rtd";

            Exception exception = trueForStorageExceptionFalseForArgumentException ?
                                      (Exception) new StorageException() :
                                      new ArgumentException();

            var mocks = new MockRepository();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(someFilePath))
                          .Throw(exception);

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

            // Call
            Action call = () => activity.Finish();

            // Assert
            const string expectedMessage = "Uitvoeren van 'Openen van bestaand project' is mislukt.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Failed, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Finish_ActivityCancelled_ProjectOwnerNotUpdatedWithLogMessage()
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
            Action call = () => activity.Finish();

            // Assert
            const string expectedMessage = "Uitvoeren van 'Openen van bestaand project' is geannuleerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(ActivityState.Canceled, activity.State);

            mocks.VerifyAll();
        }
    }
}