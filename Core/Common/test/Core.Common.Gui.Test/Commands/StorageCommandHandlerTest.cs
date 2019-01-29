// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Storage;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class StorageCommandHandlerTest : NUnitFormTest
    {
        private MockRepository mocks;

        public override void Setup()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void CreateNewProject_SavedProjectThenNewProject_NewProjectAndPathAreSet()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";

            var oldProject = mocks.Stub<IProject>();
            var newProject = mocks.Stub<IProject>();

            var projectStorage = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(oldProject);
            projectOwner.Stub(po => po.ProjectFilePath).Return(savedProjectPath);
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(newProject);
            projectOwner.Expect(po => po.SetProject(newProject, null));

            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var mainWindowController = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            Action call = () => storageCommandHandler.CreateNewProject();

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Nieuw project aanmaken is gestart.", LogLevelConstant.Info),
                Tuple.Create("Nieuw project aanmaken is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 2);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse()
        {
            // Setup
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse));
            using (new FileDisposeHelper(someValidFilePath))
            {
                var project = mocks.Stub<IProject>();
                var projectFactory = mocks.Stub<IProjectFactory>();

                const string exceptionMessage = "<some descriptive exception message>";

                var projectStorage = mocks.StrictMock<IStoreProject>();
                projectStorage.Expect(ps => ps.HasStagedProject).Return(false);
                projectStorage.Expect(ps => ps.StageProject(project));
                projectStorage.Expect(ps => ps.SaveProjectAs(someValidFilePath))
                              .Throw(new StorageException(exceptionMessage, new Exception("l33t h4xor!")));

                var projectMigrator = mocks.Stub<IMigrateProject>();

                var mainWindowController = mocks.Stub<IWin32Window>();
                var projectOwner = mocks.Stub<IProjectOwner>();
                projectOwner.Stub(po => po.Project).Return(project);
                projectOwner.Stub(po => po.ProjectFilePath).Return(someValidFilePath);

                var inquiryHelper = mocks.Stub<IInquiryHelper>();
                mocks.ReplayAll();

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
                Action call = () => result = storageCommandHandler.SaveProject();

                // Assert
                Tuple<string, LogLevelConstant>[] expectedMessages =
                {
                    Tuple.Create("Opslaan van bestaand project is gestart.", LogLevelConstant.Info),
                    Tuple.Create(exceptionMessage, LogLevelConstant.Error),
                    Tuple.Create("Opslaan van bestaand project is mislukt.", LogLevelConstant.Error)
                };
                TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 3);
                Assert.IsFalse(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectIsSuccessful_LogSuccessAndReturnTrue()
        {
            // Setup
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(SaveProject_SavingProjectIsSuccessful_LogSuccessAndReturnTrue));
            using (new FileDisposeHelper(someValidFilePath))
            {
                var project = mocks.Stub<IProject>();
                var projectFactory = mocks.Stub<IProjectFactory>();

                var projectStorage = mocks.Stub<IStoreProject>();
                projectStorage.Expect(ps => ps.StageProject(project));
                projectStorage.Expect(ps => ps.HasStagedProject).Return(false);
                projectStorage.Expect(ps => ps.SaveProjectAs(someValidFilePath));

                var projectMigrator = mocks.Stub<IMigrateProject>();

                var mainWindowController = mocks.Stub<IWin32Window>();
                var projectOwner = mocks.Stub<IProjectOwner>();
                projectOwner.Stub(po => po.Project).Return(project);
                projectOwner.Stub(po => po.ProjectFilePath).Return(someValidFilePath);

                var inquiryHelper = mocks.Stub<IInquiryHelper>();
                mocks.ReplayAll();

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
                Action call = () => result = storageCommandHandler.SaveProject();

                // Assert
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create("Opslaan van bestaand project is gelukt.", LogLevelConstant.Info));
                Assert.IsTrue(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_MigrationNeeded_MigratesFileAndSetNewlyLoadedProjectAtMigratedFileAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            string pathToMigratedFile = $"C://folder/directory/{fileName}-newerVersion.rtd";
            var loadedProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToMigratedFile))
                          .Return(loadedProject);

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            using (mocks.Ordered())
            {
                projectMigrator.Expect(pm => pm.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.Yes);
                projectMigrator.Expect(pm => pm.DetermineMigrationLocation(pathToSomeValidFile)).Return(pathToMigratedFile);
                projectMigrator.Expect(pm => pm.Migrate(pathToSomeValidFile, pathToMigratedFile)).Return(true);
            }

            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.SetProject(loadedProject, pathToMigratedFile));

            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_ShouldMigrateCancelled_LeaveCurrentProjectUnaffectedAndReturnsFalse()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";

            var projectStorage = mocks.StrictMock<IStoreProject>();

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            projectMigrator.Expect(pm => pm.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.Aborted);

            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            projectFactory.Expect(pf => pf.CreateNewProject()).Return(project)
                          .Repeat.Never();

            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Expect(po => po.SetProject(project, null))
                        .Repeat.Never();

            var mainWindowController = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_DetermineMigrationLocationButCancelled_LeaveCurrentProjectUnaffectedAndReturnsFalse()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";

            var projectStorage = mocks.StrictMock<IStoreProject>();

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            using (mocks.Ordered())
            {
                projectMigrator.Expect(pm => pm.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.Yes);
                projectMigrator.Expect(pm => pm.DetermineMigrationLocation(pathToSomeValidFile)).Return(null);
            }

            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            projectFactory.Expect(pf => pf.CreateNewProject()).Return(project)
                          .Repeat.Never();

            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Expect(po => po.SetProject(project, null))
                        .Repeat.Never();

            var mainWindowController = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetExceptions))]
        public void OpenExistingProject_ShouldMigrateThrowsException_LogFailureAndCreateNewProjectAndReturnsFalse(Exception exception, string errorMessage)
        {
            // Setup
            const string pathToSomeValidFile = " ";

            var projectStorage = mocks.StrictMock<IStoreProject>();

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            projectMigrator.Expect(pm => pm.ShouldMigrate(pathToSomeValidFile))
                           .Throw(exception);

            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.SetProject(project, null));

            var mainWindowController = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            var result = true;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(errorMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_ShouldMigrateYesAndDetermineMigrationLocationThrowsArgumentException_LogFailureAndCreateNewProjectAndReturnsFalse()
        {
            // Setup
            const string errorMessage = "I am an error message.";
            const string pathToSomeValidFile = "C://folder/directory/newProject.rtd";

            var projectStorage = mocks.StrictMock<IStoreProject>();

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            using (mocks.Ordered())
            {
                projectMigrator.Expect(pm => pm.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.Yes);
                projectMigrator.Expect(pm => pm.DetermineMigrationLocation(pathToSomeValidFile))
                               .Throw(new ArgumentException(errorMessage));
            }

            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            projectFactory.Expect(pf => pf.CreateNewProject()).Return(project);

            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Expect(po => po.SetProject(project, null));

            var mainWindowController = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectMigrator,
                projectFactory,
                projectOwner,
                inquiryHelper,
                mainWindowController);

            // Call
            var result = true;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(errorMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_ShouldMigrateTrueAndMigrateThrowsArgumentException_LogFailureAndCreateNewProjectAndReturnsFalse()
        {
            // Setup
            const string errorMessage = "I am an error message.";
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            string pathToMigratedFile = $"C://folder/directory/{fileName}-newerVersion.rtd";

            var projectStorage = mocks.StrictMock<IStoreProject>();

            var projectMigrator = mocks.StrictMock<IMigrateProject>();
            using (mocks.Ordered())
            {
                projectMigrator.Expect(pm => pm.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.Yes);
                projectMigrator.Expect(pm => pm.DetermineMigrationLocation(pathToSomeValidFile)).Return(pathToMigratedFile);
                projectMigrator.Expect(pm => pm.Migrate(pathToSomeValidFile, pathToMigratedFile))
                               .Throw(new ArgumentException(errorMessage));
            }

            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            projectFactory.Expect(pf => pf.CreateNewProject()).Return(project);

            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Expect(po => po.SetProject(project, null));

            var mainWindowController = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(errorMessage, LogLevelConstant.Error), 3);
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_LoadingProjectThrowsStorageException_LogFailureCreateNewProjectAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";
            const string goodErrorMessageText = "<some informative error message>";

            var project = mocks.Stub<IProject>();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Throw(new StorageException(goodErrorMessageText, new Exception("H@X!")));
            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(pathToSomeInvalidFile)).Return(MigrationRequired.No);
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.SetProject(project, null));
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create(goodErrorMessageText, LogLevelConstant.Error),
                Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 3);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_LoadingNull_LogFailureCreateNewProjectAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";

            var project = mocks.Stub<IProject>();
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Return(null);

            var projectMigrator = mocks.Stub<IMigrateProject>();

            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.SetProject(project, null));

            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is mislukt.", LogLevelConstant.Error)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 2);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWhenNoProjectHasBeenLoaded_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            var loadedProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeValidFile))
                          .Return(loadedProject);

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.No);
            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.SetProject(loadedProject, pathToSomeValidFile));

            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWithAlreadyLoadedProject_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = $"C://folder/directory/{fileName}.rtd";
            var loadedProject = mocks.Stub<IProject>();
            var originalProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeValidFile))
                          .Return(loadedProject);

            var projectMigrator = mocks.Stub<IMigrateProject>();
            projectMigrator.Stub(m => m.ShouldMigrate(pathToSomeValidFile)).Return(MigrationRequired.No);

            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = originalProject;

            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(originalProject);
            projectOwner.Stub(po => po.ProjectFilePath).Return("<original file path>");
            projectOwner.Stub(po => po.SetProject(loadedProject, pathToSomeValidFile));

            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create("Openen van project is gestart.", LogLevelConstant.Info),
                Tuple.Create("Openen van project is gelukt.", LogLevelConstant.Info)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetExistingProjectFilePath_FilePathSelectedAndOkClicked_ReturnsSelectedFilePath()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var project = mocks.Stub<IProject>();

            projectOwner.Stub(po => po.Project).Return(project);
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);
            projectStorage.Stub(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(false);
            projectStorage.Stub(ps => ps.FileFilter).Return(string.Empty);

            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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
        public void AskConfirmationUnsavedChanges_ProjectSetNoChange_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.ProjectFilePath).Return("");
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeCancelPressed_ReturnsFalse()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var project = mocks.Stub<IProject>();
            const string projectName = "Project";
            project.Name = projectName;

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.StageProject(project));
            projectStorage.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorage.Expect(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(true);
            projectStorage.Expect(ps => ps.UnstageProject());

            var projectMigrator = mocks.Stub<IMigrateProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.ProjectFilePath).Return("");

            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquirePerformOptionalStep("Project afsluiten",
                                                                   $"Sla wijzigingen in het project op: {projectName}?"))
                         .Return(OptionalStepResult.Cancel);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeNoPressed_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var project = mocks.Stub<IProject>();
            const string projectName = "Project";
            project.Name = projectName;

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.StageProject(project));
            projectStorage.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorage.Expect(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(true);
            projectStorage.Expect(ps => ps.UnstageProject());

            var projectMigrator = mocks.Stub<IMigrateProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.ProjectFilePath).Return("");

            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquirePerformOptionalStep("Project afsluiten",
                                                                   $"Sla wijzigingen in het project op: {projectName}?"))
                         .Return(OptionalStepResult.SkipOptionalStep);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeYesPressed_ReturnsTrue()
        {
            // Setup
            const string projectName = "Project";
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(AskConfirmationUnsavedChanges_ProjectSetWithChangeYesPressed_ReturnsTrue));
            using (new FileDisposeHelper(someValidFilePath))
            {
                var mainWindowController = mocks.Stub<IWin32Window>();
                var projectFactory = mocks.Stub<IProjectFactory>();

                var project = mocks.Stub<IProject>();
                project.Name = projectName;

                var projectStorage = mocks.StrictMock<IStoreProject>();
                projectStorage.Expect(ps => ps.StageProject(project));
                projectStorage.Expect(ps => ps.HasStagedProject).Return(true).Repeat.Twice();
                projectStorage.Expect(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(true);
                projectStorage.Expect(ps => ps.UnstageProject());
                projectStorage.Expect(p => p.SaveProjectAs(someValidFilePath));

                var projectMigrator = mocks.Stub<IMigrateProject>();

                var projectOwner = mocks.Stub<IProjectOwner>();
                projectOwner.Stub(po => po.Project).Return(project);
                projectOwner.Stub(po => po.ProjectFilePath).Return(someValidFilePath);

                var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
                inquiryHelper.Expect(h => h.InquirePerformOptionalStep("Project afsluiten",
                                                                       $"Sla wijzigingen in het project op: {projectName}?"))
                             .Return(OptionalStepResult.PerformOptionalStep);
                mocks.ReplayAll();

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
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeYesFileDoesNotExist_ReturnsTrue()
        {
            // Setup
            const string fileFilter = "<Some text> | *.rtd";
            const string projectName = "Project";
            string someValidFilePath = TestHelper.GetScratchPadPath(nameof(AskConfirmationUnsavedChanges_ProjectSetWithChangeYesFileDoesNotExist_ReturnsTrue));

            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var project = mocks.Stub<IProject>();
            project.Name = projectName;

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.StageProject(project));
            projectStorage.Stub(ps => ps.HasStagedProject).Return(true);
            projectStorage.Expect(ps => ps.HasStagedProjectChanges(someValidFilePath)).Return(true);
            projectStorage.Expect(ps => ps.UnstageProject());
            projectStorage.Stub(ps => ps.FileFilter).Return(fileFilter);
            projectStorage.Expect(p => p.SaveProjectAs(someValidFilePath));

            var projectMigrator = mocks.Stub<IMigrateProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.ProjectFilePath).Return(someValidFilePath);
            projectOwner.Expect(po => po.SetProject(project, someValidFilePath));

            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquirePerformOptionalStep("Project afsluiten",
                                                                   $"Sla wijzigingen in het project op: {projectName}?"))
                         .Return(OptionalStepResult.PerformOptionalStep);
            inquiryHelper.Expect(h => h.GetTargetFileLocation(fileFilter, projectName))
                         .Return(someValidFilePath);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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