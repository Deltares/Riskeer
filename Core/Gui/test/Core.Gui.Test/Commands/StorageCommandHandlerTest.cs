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
using System.Drawing;
using System.Threading;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using Core.Gui.Commands;
using Core.Gui.Forms.Main;
using Core.Gui.Helpers;
using Core.Gui.Selection;
using Core.Gui.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Arg = NSubstitute.Arg;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    public class StorageCommandHandlerTest : NUnitFormTest
    {
        [Test]
        public void CreateNewProject_SavedProjectThenNewProject_NewProjectAndPathAreSet()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";

            var oldProject = Substitute.For<IProject>();
            var newProject = Substitute.For<IProject>();

            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(oldProject);
            projectOwner.ProjectFilePath.Returns(savedProjectPath);

            var projectFactory = Substitute.For<IProjectFactory>();
            projectFactory.CreateNewProject().Returns(newProject);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            void Call() => storageCommandHandler.CreateNewProject();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Nieuw project aanmaken is gestart.", LogLevelConstant.Info),
                Tuple.Create("Nieuw project aanmaken is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 2);
        }

        [Test]
        public void CreateNewProject_ProjectFactoryReturnsNull_LogsMessageAndProjectSetToNull()
        {
            // Setup
            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(Substitute.For<IProject>());
            projectOwner.ProjectFilePath.Returns((string) null);

            var projectFactory = Substitute.For<IProjectFactory>();
            projectFactory.CreateNewProject().Returns((IProject) null);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            void Call() => storageCommandHandler.CreateNewProject();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Nieuw project aanmaken is gestart.", LogLevelConstant.Info),
                Tuple.Create("Nieuw project aanmaken is geannuleerd.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 2);
        }

        [Test]
        public void CreateNewProject_ProjectFactoryThrowsProjectFactoryException_LogsMessageAndProjectSetToNull()
        {
            // Setup
            const string expectedExceptionMessage = "Error message";

            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(Substitute.For<IProject>());
            projectOwner.ProjectFilePath.Returns((string) null);
            projectOwner.When(po => po.SetProject(null, null));

            var projectFactory = Substitute.For<IProjectFactory>();
            projectFactory.When(x => x.CreateNewProject()).Do(x =>
            {
                throw new ProjectFactoryException(expectedExceptionMessage);
            });

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            void Call() => storageCommandHandler.CreateNewProject();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Nieuw project aanmaken is gestart.", LogLevelConstant.Info),
                Tuple.Create(expectedExceptionMessage, LogLevelConstant.Error),
                Tuple.Create("Nieuw project aanmaken is mislukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 3);
        }

        [Test]
        public void SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse()
        {
            // Setup
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse));
            using (new FileDisposeHelper(someValidFilePath))
            {
                var project = Substitute.For<IProject>();
                var projectFactory = Substitute.For<IProjectFactory>();

                const string exceptionMessage = "<some descriptive exception message>";

                var projectStorage = Substitute.For<IStoreProject>();
                projectStorage.HasStagedProject.Returns(false);
                projectStorage.StageProject(project);
                projectStorage.When(x => x.SaveProjectAs(someValidFilePath))
                              .Do(x =>
                              {
                                  throw new StorageException(exceptionMessage, new Exception("l33t h4xor!"));
                              });

                var projectMigrator = Substitute.For<IMigrateProject>();

                var projectOwner = Substitute.For<IProjectOwner>();
                projectOwner.Project.Returns(project);
                projectOwner.ProjectFilePath.Returns(someValidFilePath);

                var inquiryHelper = Substitute.For<IInquiryHelper>();

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
                var mainWindowController = Substitute.For<IMainWindowController>();
                mainWindowController.MainWindow.Returns(mainWindow);
                var storageCommandHandler = new StorageCommandHandler(
                    projectStorage,
                    projectMigrator,
                    projectFactory,
                    projectOwner,
                    inquiryHelper,
                    mainWindowController);

                DialogBoxHandler = (s, hWnd) =>
                {
                    // Expect progress dialog, which will close automatically.
                };

                // Call
                var result = true;
                void Call() => result = storageCommandHandler.SaveProject();

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Opslaan van bestaand project is gestart.", LogLevelConstant.Info),
                    Tuple.Create(exceptionMessage, LogLevelConstant.Error),
                    Tuple.Create("Opslaan van bestaand project is mislukt.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 3);
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void SaveProject_SavingProjectIsSuccessful_LogSuccessAndReturnTrue()
        {
            // Setup
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(SaveProject_SavingProjectIsSuccessful_LogSuccessAndReturnTrue));
            using (new FileDisposeHelper(someValidFilePath))
            {
                var project = Substitute.For<IProject>();
                var projectFactory = Substitute.For<IProjectFactory>();

                var projectStorage = Substitute.For<IStoreProject>();
                projectStorage.HasStagedProject.Returns(false);

                var projectMigrator = Substitute.For<IMigrateProject>();

                var projectOwner = Substitute.For<IProjectOwner>();
                projectOwner.Project.Returns(project);
                projectOwner.ProjectFilePath.Returns(someValidFilePath);

                var inquiryHelper = Substitute.For<IInquiryHelper>();

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
                var mainWindowController = Substitute.For<IMainWindowController>();
                mainWindowController.MainWindow.Returns(mainWindow);
                var storageCommandHandler = new StorageCommandHandler(
                    projectStorage,
                    projectMigrator,
                    projectFactory,
                    projectOwner,
                    inquiryHelper,
                    mainWindowController);

                DialogBoxHandler = (s, hWnd) =>
                {
                    // Expect progress dialog, which will close automatically.
                };

                // Call
                var result = false;
                void Call() => result = storageCommandHandler.SaveProject();

                // Assert
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create("Opslaan van bestaand project is gelukt.", LogLevelConstant.Info));
                Assert.IsTrue(result);

                projectStorage.Received().StageProject(project);
                projectStorage.Received().SaveProjectAs(someValidFilePath);
            }
        }

        [Test]
        public void OpenExistingProject_MigrationNeeded_MigratesFileAndSetNewlyLoadedProjectAtMigratedFileAndReturnTrue()
        {
            // Arrange
            const string fileName = "newProject";

            var pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            var pathToMigratedFile = $"C://folder/directory/{fileName}-newerVersion.rtd";

            var loadedProject = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(pathToMigratedFile).Returns(loadedProject);

            var mainWindow = Substitute.For<IMainWindow>();
            var mainWindowController = Substitute.For<IMainWindowController>();

            var projectMigrator = Substitute.For<IMigrateProject>();

            projectMigrator.ShouldMigrate(pathToSomeValidFile)
                           .Returns(MigrationRequired.Yes);

            projectMigrator.DetermineMigrationLocation(pathToSomeValidFile)
                           .Returns(pathToMigratedFile);

            mainWindowController.MainWindow.Returns(mainWindow);
            mainWindow.ApplicationIcon.Returns(SystemIcons.Application);
            mainWindow.Handle.Returns(IntPtr.Zero);

            projectMigrator.Migrate(pathToSomeValidFile, pathToMigratedFile)
                           .Returns(true);

            var projectOwner = Substitute.For<IProjectOwner>();

            var inquiryHelper = Substitute.For<IInquiryHelper>();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity dialog opened and will be closed automatically once done.
            };

            // Act
            var result = false;

            void Call() =>
                result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
            };

            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 2);

            Assert.IsTrue(result);

            projectOwner.Received(1)
                        .SetProject(loadedProject, pathToMigratedFile);

            Received.InOrder(() =>
            {
                projectMigrator.ShouldMigrate(pathToSomeValidFile);
                projectMigrator.DetermineMigrationLocation(pathToSomeValidFile);
                projectMigrator.Migrate(pathToSomeValidFile, pathToMigratedFile);
                projectStorage.LoadProject(pathToMigratedFile);
                projectOwner.SetProject(loadedProject, pathToMigratedFile);
            });
        }

        [Test]
        public void OpenExistingProject_ShouldMigrateCancelled_LeaveCurrentProjectUnaffectedAndReturnsFalse()
        {
            // Setup
            const string fileName = "newProject";
            var pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";

            var projectStorage = Substitute.For<IStoreProject>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.Aborted);

            var project = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            projectFactory.CreateNewProject().Returns(project);

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.Project.Returns(project);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);
            // Call
            bool result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Assert.IsFalse(result);
            projectOwner.DidNotReceive().SetProject(Arg.Any<IProject>(), Arg.Any<string>());
            projectFactory.DidNotReceive().CreateNewProject();
        }

        [Test]
        public void OpenExistingProject_MigrationNotSupported_SetProjectNullAndReturnsFalse()
        {
            // Setup
            const string fileName = "newProject";
            var pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";

            var projectStorage = Substitute.For<IStoreProject>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.NotSupported);

            var project = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            projectFactory.CreateNewProject().Returns(project);

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.SetProject(Arg.Any<IProject>(), Arg.Any<string>());

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Assert.IsFalse(result);
            projectFactory.DidNotReceive().CreateNewProject();
        }

        [Test]
        public void OpenExistingProject_DetermineMigrationLocationButCancelled_LeaveCurrentProjectUnaffectedAndReturnsFalse()
        {
            // Setup
            const string fileName = "newProject";
            var pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";

            var projectStorage = Substitute.For<IStoreProject>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.Yes);
            projectMigrator.DetermineMigrationLocation(pathToSomeValidFile).Returns("");

            var project = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.SetProject(Arg.Any<IProject>(), Arg.Any<string>());

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Assert.IsFalse(result);
            Received.InOrder(() =>
            {
                projectMigrator.Received().ShouldMigrate(pathToSomeValidFile);
                projectMigrator.DetermineMigrationLocation(pathToSomeValidFile);
            });
        }

        [Test]
        [TestCaseSource(nameof(GetExceptions))]
        public void OpenExistingProject_ShouldMigrateThrowsException_LogFailureAndSetNullProjectAndReturnsFalse(Exception exception, string errorMessage)
        {
            // Setup
            const string pathToSomeValidFile = " ";

            var projectStorage = Substitute.For<IStoreProject>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(_ => throw (exception));

            var project = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            var result = true;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(errorMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(result);
            projectOwner.Received().SetProject(Arg.Any<IProject>(), Arg.Any<string>());
        }

        [Test]
        public void OpenExistingProject_ShouldMigrateYesAndDetermineMigrationLocationThrowsArgumentException_LogFailureAndSetProjectNullAndReturnsFalse()
        {
            // Setup
            const string errorMessage = "I am an error message.";
            const string pathToSomeValidFile = "C://folder/directory/newProject.rtd";

            var projectStorage = Substitute.For<IStoreProject>();

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.Yes);
            projectMigrator.DetermineMigrationLocation(pathToSomeValidFile).Returns(_ => throw (new ArgumentException(errorMessage)));

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            var result = true;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(errorMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(result);
            Received.InOrder(() =>
            {
                projectMigrator.ShouldMigrate(pathToSomeValidFile);
                projectMigrator.DetermineMigrationLocation(pathToSomeValidFile);
            });
            projectOwner.Received().SetProject(Arg.Any<IProject>(), Arg.Any<string>());
        }

        [Test]
        public void OpenExistingProject_ShouldMigrateTrueAndMigrateThrowsArgumentException_LogFailureAndSetProjectNullAndReturnsFalse()
        {
            // Setup
            const string errorMessage = "I am an error message.";
            const string fileName = "newProject";
            var pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            var pathToMigratedFile = $"C://folder/directory/{fileName}-newerVersion.rtd";

            var projectStorage = Substitute.For<IStoreProject>();

            var mainWindow = Substitute.For<IMainWindow>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.Yes);
            projectMigrator.DetermineMigrationLocation(pathToSomeValidFile).Returns(pathToMigratedFile);
            mainWindowController.MainWindow.Returns(mainWindow);
            mainWindow.ApplicationIcon.Returns(SystemIcons.Application);
            mainWindow.Handle.Returns(IntPtr.Zero);
            projectMigrator.Migrate(pathToSomeValidFile, pathToMigratedFile).Returns(_ => throw (new ArgumentException(errorMessage)));

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity dialog opened and will be closed automatically once done.
            };

            // Call
            var result = true;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(errorMessage, LogLevelConstant.Error), 3);
            Assert.IsFalse(result);
            projectOwner.Received().SetProject(Arg.Any<IProject>(), Arg.Any<string>());
            Received.InOrder(() =>
            {
                projectMigrator.ShouldMigrate(pathToSomeValidFile);
                projectMigrator.DetermineMigrationLocation(pathToSomeValidFile);
                _ = mainWindowController.MainWindow;
                _ = mainWindow.ApplicationIcon;
                _ = mainWindow.Handle;
                projectMigrator.Migrate(pathToSomeValidFile, pathToMigratedFile);
            });
        }

        [Test]
        public void OpenExistingProject_LoadingProjectThrowsStorageException_LogFailureSetNullProjectAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";
            const string goodErrorMessageText = "<some informative error message>";

            var project = Substitute.For<IProject>();
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(pathToSomeInvalidFile)
                          .Returns(_ => throw (new StorageException(goodErrorMessageText, new Exception("H@X!"))));

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeInvalidFile).Returns(MigrationRequired.No);

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
            var mainWindowController = Substitute.For<IMainWindowController>();
            mainWindowController.MainWindow.Returns(mainWindow);
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity dialog opened and will be closed automatically once done.
            };

            // Call
            var result = true;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create(goodErrorMessageText, LogLevelConstant.Error),
                Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 3);
            Assert.IsFalse(result);
            projectOwner.Received().SetProject(null,null);
        }

        [Test]
        public void OpenExistingProject_LoadingNull_LogFailureSetNullProjectAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";

            var project = Substitute.For<IProject>();
            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(pathToSomeInvalidFile).Returns((IProject)null);

            var projectMigrator = Substitute.For<IMigrateProject>();

            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
            var mainWindowController = Substitute.For<IMainWindowController>();
            mainWindowController.MainWindow.Returns(mainWindow);
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity dialog opened and will be closed automatically once done.
            };

            // Call
            var result = true;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 2);
            Assert.IsFalse(result);
            projectOwner.Received().SetProject(Arg.Any<IProject>(), Arg.Any<string>());
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWhenNoProjectHasBeenLoaded_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            var pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            var loadedProject = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(pathToSomeValidFile).Returns(loadedProject);

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.No);

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.SetProject(loadedProject, pathToSomeValidFile);

            var inquiryHelper = Substitute.For<IInquiryHelper>();

            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

            var mainWindowController = Substitute.For<IMainWindowController>();
            mainWindowController.MainWindow.Returns(mainWindow);
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity dialog opened and will be closed automatically once done.
            };

            // Call
            var result = false;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 2);
            Assert.IsTrue(result);
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWithAlreadyLoadedProject_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            var loadedProject = Substitute.For<IProject>();
            var originalProject = Substitute.For<IProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.LoadProject(pathToSomeValidFile).Returns(loadedProject);

            var projectMigrator = Substitute.For<IMigrateProject>();
            projectMigrator.ShouldMigrate(pathToSomeValidFile).Returns(MigrationRequired.No);

            var applicationSelection = Substitute.For<IApplicationSelection>();
            applicationSelection.Selection = originalProject;

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(originalProject);
            projectOwner.ProjectFilePath.Returns("<original file path>");
            projectOwner.SetProject(loadedProject, pathToSomeValidFile);

            var inquiryHelper = Substitute.For<IInquiryHelper>();

            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

            var mainWindowController = Substitute.For<IMainWindowController>();
            mainWindowController.MainWindow.Returns(mainWindow);
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity dialog opened and will be closed automatically once done.
            };

            // Call
            var result = false;
            void Call() => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages, 2);
            Assert.IsTrue(result);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetExistingProjectFilePath_FilePathSelectedAndOkClicked_ReturnsSelectedFilePath()
        {
            // Setup
            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns((IProject)null);
            projectStorage.HasStagedProjectChanges(Arg.Any<string>()).Returns(false);
            projectStorage.OpenProjectFileFilter.Returns(string.Empty);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            string projectPath = TestHelper.GetScratchPadPath(
                nameof(GetExistingProjectFilePath_FilePathSelectedAndOkClicked_ReturnsSelectedFilePath));
            using (new FileDisposeHelper(projectPath))
            {
                var storageCommandHandler = new StorageCommandHandler(
                    projectStorage,
                    projectMigrator,
                    projectFactory,
                    projectOwner,
                    inquiryHelper,
                    mainWindowController);

                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new OpenFileDialogTester(wnd);
                    helper.OpenFile(projectPath);
                };

                // Call
                string returnedPath = storageCommandHandler.GetExistingProjectFilePath();

                // Assert
                Assert.AreEqual(projectPath, returnedPath);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetExistingProjectFilePath_NoFilePathSelectedAndCancelClicked_ReturnsFilePathNull()
        {
            // Setup
            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new OpenFileDialogTester(wnd);
                helper.ClickCancel();
            };

            // Call
            string returnedPath = storageCommandHandler.GetExistingProjectFilePath();

            // Assert
            Assert.IsNull(returnedPath);
        }

        [Test]
        public void HandleUnsavedChanges_NoProjectSet_ReturnsTrue()
        {
            // Setup
            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns((IProject)null);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);
        }

        [Test]
        public void HandleUnsavedChanges_ProjectSetNoChange_ReturnsTrue()
        {
            // Setup
            var project = Substitute.For<IProject>();
            var projectStorage = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.ProjectFilePath.Returns("");
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void HandleUnsavedChanges_ProjectSetWithChangeCancelPressed_ReturnsFalse()
        {
            // Setup
            var project = Substitute.For<IProject>();
            const string projectName = "Project";
            project.Name = projectName;

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.StageProject(project);
            projectStorage.HasStagedProject.Returns(true);
            projectStorage.HasStagedProjectChanges(Arg.Any<string>()).Returns(true);
            projectStorage.UnstageProject();

            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.ProjectFilePath.Returns("");

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquirePerformOptionalStep("Project afsluiten",
                                                      $"Sla wijzigingen in het project op: {projectName}?")
                         .Returns(OptionalStepResult.Cancel);
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsFalse(changesHandled);
            projectStorage.Received().StageProject(project);
            projectStorage.Received().HasStagedProjectChanges(Arg.Any<string>());
            projectStorage.Received().UnstageProject();
            inquiryHelper.Received().InquirePerformOptionalStep("Project afsluiten",
                                                                 $"Sla wijzigingen in het project op: {projectName}?");
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void HandleUnsavedChangesProjectSetWithChangeNoPressed_ReturnsTrue()
        {
            // Setup
            var project = Substitute.For<IProject>();
            const string projectName = "Project";
            project.Name = projectName;

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.StageProject(project);
            projectStorage.HasStagedProject.Returns(true);
            projectStorage.HasStagedProjectChanges(Arg.Any<string>()).Returns(true);

            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.ProjectFilePath.Returns("");

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquirePerformOptionalStep("Project afsluiten",
                                                                   $"Sla wijzigingen in het project op: {projectName}?")
                                       .Returns(OptionalStepResult.SkipOptionalStep);
            var mainWindowController = Substitute.For<IMainWindowController>();
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);
            projectStorage.Received().StageProject(project);
            projectStorage.Received().HasStagedProjectChanges(Arg.Any<string>());
            projectStorage.Received().UnstageProject();
            inquiryHelper.Received().InquirePerformOptionalStep("Project afsluiten",
                                                                $"Sla wijzigingen in het project op: {projectName}?");
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void HandleUnsavedChanges_ProjectSetWithChangeYesPressed_ReturnsTrue()
        {
            // Setup
            const string projectName = "Project";
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(HandleUnsavedChanges_ProjectSetWithChangeYesPressed_ReturnsTrue));
            using (new FileDisposeHelper(someValidFilePath))
            {
                var project = Substitute.For<IProject>();
                project.Name = projectName;

                var projectStorage = Substitute.For<IStoreProject>();
                projectStorage.StageProject(project);
                
                projectStorage.UnstageProject();
                projectStorage.StageProject(project);
                projectStorage.HasStagedProject.Returns(true);
                projectStorage.HasStagedProjectChanges(Arg.Any<string>()).Returns(true);
                
                projectStorage.SaveProjectAs(someValidFilePath);

                var projectMigrator = Substitute.For<IMigrateProject>();
                var projectFactory = Substitute.For<IProjectFactory>();

                var projectOwner = Substitute.For<IProjectOwner>();
                projectOwner.Project.Returns(project);
                projectOwner.ProjectFilePath.Returns(someValidFilePath);

                var inquiryHelper = Substitute.For<IInquiryHelper>();
                inquiryHelper.InquirePerformOptionalStep("Project afsluiten",
                                                                       $"Sla wijzigingen in het project op: {projectName}?")
                             .Returns(OptionalStepResult.PerformOptionalStep);

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
                var mainWindowController = Substitute.For<IMainWindowController>();
                mainWindowController.MainWindow.Returns(mainWindow);
                var storageCommandHandler = new StorageCommandHandler(
                    projectStorage,
                    projectMigrator,
                    projectFactory,
                    projectOwner,
                    inquiryHelper,
                    mainWindowController);

                DialogBoxHandler = (s, hWnd) =>
                {
                    // Expect progress dialog, which will close automatically.
                };

                // Call
                bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

                // Assert
                Assert.IsTrue(changesHandled);
                projectStorage.Received().StageProject(project);
                projectStorage.Received().HasStagedProjectChanges(Arg.Any<string>());
                projectStorage.Received().UnstageProject();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void HandleUnsavedChanges_ProjectSetWithChangeYesFileDoesNotExist_ReturnsTrue()
        {
            // Setup
            const string fileFilter = "<Some text> | *.rtd";
            const string projectName = "Project";
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(HandleUnsavedChanges_ProjectSetWithChangeYesFileDoesNotExist_ReturnsTrue));

            DialogBoxHandler = (s, hWnd) =>
            {
                // Expect progress dialog, which will close automatically.
            };

            var project = Substitute.For<IProject>();
            project.Name = projectName;

            var projectStorage = Substitute.For<IStoreProject>();
            projectStorage.StageProject(project);
            projectStorage.HasStagedProject.Returns(true);
            projectStorage.HasStagedProjectChanges(someValidFilePath).Returns(true);
            projectStorage.SaveProjectFileFilter.Returns(fileFilter);
            projectStorage.SaveProjectAs(someValidFilePath);

            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();

            var projectOwner = Substitute.For<IProjectOwner>();
            projectOwner.Project.Returns(project);
            projectOwner.ProjectFilePath.Returns(someValidFilePath);
            projectOwner.SetProject(project, someValidFilePath);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquirePerformOptionalStep("Project afsluiten",
                                                                   $"Sla wijzigingen in het project op: {projectName}?")
                                       .Returns(OptionalStepResult.PerformOptionalStep);
            inquiryHelper.GetTargetFileLocation(fileFilter, projectName)
                                       .Returns(someValidFilePath);

            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
            var mainWindowController = Substitute.For<IMainWindowController>();
            mainWindowController.MainWindow.Returns(mainWindow);
            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);
            projectStorage.Received().StageProject(project);
            projectStorage.Received().HasStagedProjectChanges(someValidFilePath);
            projectStorage.Received().UnstageProject();
            projectOwner.Received().SetProject(project, someValidFilePath);

            inquiryHelper.Received().InquirePerformOptionalStep("Project afsluiten",
                                                                $"Sla wijzigingen in het project op: {projectName}?");
            inquiryHelper.Received().GetTargetFileLocation(fileFilter, projectName);
        }

        private static IEnumerable<TestCaseData> GetExceptions()
        {
            const string exceptionMessage = "I am an error message";

            yield return new TestCaseData(new ArgumentException(exceptionMessage), exceptionMessage)
                .SetName("ArgumentException");
            yield return new TestCaseData(new CriticalFileReadException(exceptionMessage), exceptionMessage)
                .SetName("CriticalFileReadException");
            yield return new TestCaseData(new StorageValidationException(exceptionMessage), exceptionMessage)
                .SetName("StorageValidationException");
        }
    }
}