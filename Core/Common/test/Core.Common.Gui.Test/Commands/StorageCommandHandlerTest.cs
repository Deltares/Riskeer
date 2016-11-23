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
using System.Windows.Forms;
using Core.Common.Base.Data;
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

        [SetUp]
        public void SetUp()
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
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(oldProject);
            projectOwner.Stub(po => po.ProjectFilePath).Return(savedProjectPath);
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(newProject);
            projectOwner.Expect(po => po.SetProject(newProject, null));

            var mainWindowController = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            Action call = () => storageCommandHandler.CreateNewProject();

            // Assert
            var expectedMessages = new[]
            {
                "Nieuw Ringtoetsproject aanmaken...",
                "Nieuw Ringtoetsproject succesvol aangemaakt."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse()
        {
            // Setup
            const string someValidFilePath = "<some valid file path>";
            var projectStub = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            const string exceptionMessage = "<some descriptive exception message>";

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.HasStagedProject).Return(false);
            projectStorage.Expect(ps => ps.StageProject(projectStub));
            projectStorage.Expect(ps => ps.SaveProjectAs(someValidFilePath)).
                           Throw(new StorageException(exceptionMessage, new Exception("l33t h4xor!")));

            var mainWindowController = mocks.StrictMock<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(projectStub);
            projectOwner.Stub(po => po.ProjectFilePath).Return(someValidFilePath);
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool result = true;
            Action call = () => result = storageCommandHandler.SaveProject();

            // Assert
            var expectedMessages = new[]
            {
                exceptionMessage,
                "Het is niet gelukt om het Ringtoetsproject op te slaan."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectIsSuccessful_LogSuccessAndReturnTrue()
        {
            // Setup
            var projectStub = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            const string someValidFilePath = "<some valid filepath>";

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.StageProject(projectStub));
            projectStorage.Expect(ps => ps.HasStagedProject).Return(false);
            projectStorage.Expect(ps => ps.SaveProjectAs(someValidFilePath));

            var mainWindowController = mocks.StrictMock<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(projectStub);
            projectOwner.Stub(po => po.ProjectFilePath).Return(someValidFilePath);
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool result = false;
            Action call = () => result = storageCommandHandler.SaveProject();

            // Assert
            var expectedMessage = string.Format("Het Ringtoetsproject '{0}' is succesvol opgeslagen.",
                                                projectStub.Name);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(result);

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
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Throw(new StorageException(goodErrorMessageText, new Exception("H@X!")));
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.SetProject(project, null));
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool result = true;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject...",
                goodErrorMessageText,
                "Het is niet gelukt om het Ringtoetsproject te laden."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 3);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_LoadingNull_LogFailureCreateNewProjectAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";

            IProject project = mocks.Stub<IProject>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project);
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Return(null);
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.SetProject(project, null));
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool result = true;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject...",
                "Het is niet gelukt om het Ringtoetsproject te laden."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWhenNoProjectHasBeenLoaded_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = string.Format("C://folder/directory/{0}.rtd",
                                                         fileName);
            var loadedProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeValidFile))
                          .Return(loadedProject);

            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.SetProject(loadedProject, pathToSomeValidFile));
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool result = false;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject...",
                "Bestaand Ringtoetsproject succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWithAlreadyLoadedProject_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeValidFile = string.Format("C://folder/directory/{0}.rtd",
                                                       fileName);
            var loadedProject = mocks.Stub<IProject>();
            var originalProject = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeValidFile))
                          .Return(loadedProject);
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = originalProject;

            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(originalProject);
            projectOwner.Stub(po => po.ProjectFilePath).Return("<original file path>");
            projectOwner.Stub(po => po.SetProject(loadedProject, pathToSomeValidFile));

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool result = false;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject...",
                "Bestaand Ringtoetsproject succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void AskConfirmationUnsavedChanges_ProjectSetNoChange_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var projectStorageMock = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            projectOwner.Stub(po => po.ProjectFilePath).Return("");
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectFactory,
                projectOwner,
                mainWindowController);

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeCancelPressed_ReturnsFalse()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            const string projectName = "Project";
            project.Name = projectName;
            var projectStorageMock = mocks.StrictMock<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(project));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerStub = mocks.Stub<IProjectOwner>();
            projectOwnerStub.Stub(po => po.Project).Return(project);
            projectOwnerStub.Stub(po => po.ProjectFilePath).Return("");
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectFactory,
                projectOwnerStub,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.ClickCancel();
            };

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsFalse(changesHandled);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeNoPressed_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            const string projectName = "Project";
            project.Name = projectName;
            var projectStorageMock = mocks.StrictMock<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(project));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerStub = mocks.Stub<IProjectOwner>();
            projectOwnerStub.Stub(po => po.Project).Return(project);
            projectOwnerStub.Stub(po => po.ProjectFilePath).Return("");
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectFactory,
                projectOwnerStub,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.SendCommand(MessageBoxTester.Command.No);
            };

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void AskConfirmationUnsavedChanges_ProjectSetWithChangeYesPressed_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var project = mocks.Stub<IProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            const string projectName = "Project";
            project.Name = projectName;
            var projectFilePath = "some path";

            var projectStorageMock = mocks.StrictMock<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(project));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true).Repeat.Twice();
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges(null)).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerStub = mocks.Stub<IProjectOwner>();
            projectOwnerStub.Stub(po => po.Project).Return(project);
            projectOwnerStub.Stub(po => po.ProjectFilePath).Return(projectFilePath);

            projectStorageMock.Expect(p => p.SaveProjectAs(projectFilePath));
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectFactory,
                projectOwnerStub,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.SendCommand(MessageBoxTester.Command.Yes);
            };

            // Call
            bool changesHandled = storageCommandHandler.HandleUnsavedChanges();

            // Assert
            Assert.IsTrue(changesHandled);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }
    }
}