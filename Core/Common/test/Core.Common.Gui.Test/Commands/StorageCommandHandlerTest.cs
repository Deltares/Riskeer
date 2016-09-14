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
using Core.Common.Gui.Properties;
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
        public void CreateNewProject_NoProjectSet_NewProjectIsSet()
        {
            // Setup
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Expect(po => po.CreateNewProject());
            var mainWindowController = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            Action call = () => storageCommandHandler.CreateNewProject();

            // Assert
            var expectedMessages = new[]
            {
                "Openen van nieuw Ringtoetsproject.",
                "Nieuw Ringtoetsproject succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.AreEqual("", projectOwner.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateNewProject_SavedProjectThenNewProject_NewProjectAndPathAreSet()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";

            var projectMock = mocks.StrictMock<IProject>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = savedProjectPath;
            projectOwner.Expect(po => po.CreateNewProject());
            projectOwner.Expect(po => po.IsCurrentNew()).Return(true);
            projectOwner.Expect(po => po.CloseProject());

            var mainWindowController = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            Action call = () => storageCommandHandler.CreateNewProject();

            // Assert
            var expectedMessages = new[]
            {
                "Openen van nieuw Ringtoetsproject.",
                "Nieuw Ringtoetsproject succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);

            Assert.IsInstanceOf<IProject>(projectOwner.Project);
            Assert.AreEqual("", projectOwner.ProjectFilePath);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_ProjectIsNull_DoNothingAndReturnFalse()
        {
            // Setup
            var projectStorage = mocks.StrictMock<IStoreProject>();
            var mainWindowController = mocks.StrictMock<IWin32Window>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Expect(po => po.Project).Return(null);
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            var result = storageCommandHandler.SaveProject();

            // Assert
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse()
        {
            // Setup
            const string someValidFilePath = "<some valid file path>";
            var projectMock = mocks.Stub<IProject>();

            const string exceptionMessage = "<some descriptive exception message>";

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.HasStagedProject).Return(false);
            projectStorage.Expect(ps => ps.StageProject(projectMock));
            projectStorage.Expect(ps => ps.SaveProjectAs(someValidFilePath)).
                           Throw(new StorageException(exceptionMessage, new Exception("l33t h4xor!")));

            var mainWindowController = mocks.StrictMock<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = someValidFilePath;
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
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
            var projectMock = mocks.Stub<IProject>();
            const string someValidFilePath = "<some valid filepath>";

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.StageProject(projectMock));
            projectStorage.Expect(ps => ps.HasStagedProject).Return(false);
            projectStorage.Expect(ps => ps.SaveProjectAs(someValidFilePath));

            var mainWindowController = mocks.StrictMock<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = someValidFilePath;
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            bool result = false;
            Action call = () => result = storageCommandHandler.SaveProject();

            // Assert
            var expectedMessage = string.Format("Het Ringtoetsproject '{0}' is succesvol opgeslagen.",
                                                projectMock.Name);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_LoadingProjectThrowsStorageException_LogFailureAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";
            const string goodErrorMessageText = "<some informative error message>";

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Throw(new StorageException(goodErrorMessageText, new Exception("H@X!")));
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            bool result = true;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject.",
                goodErrorMessageText,
                "Het is niet gelukt om het Ringtoetsproject te laden."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 3);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_LoadingNull_LogFailureAndReturnFalse()
        {
            // Setup
            const string pathToSomeInvalidFile = "<path to some invalid file>";

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Return(null);
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            bool result = true;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject.",
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
            string pathToSomeInvalidFile = string.Format("C://folder/directory/{0}.rtd",
                                                         fileName);
            var loadedProject = mocks.Stub<IProject>();

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Return(loadedProject);

            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            bool result = false;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeInvalidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject.",
                "Bestaand Ringtoetsproject succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            Assert.IsInstanceOf<IProject>(projectOwner.Project);
            Assert.AreEqual(pathToSomeInvalidFile, projectOwner.ProjectFilePath);
            Assert.AreEqual(fileName, projectOwner.Project.Name);
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

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeValidFile))
                          .Return(loadedProject);
            projectStorage.Stub(ps => ps.CloseProject());
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = originalProject;

            var mainWindowController = mocks.Stub<IWin32Window>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = originalProject;
            projectOwner.ProjectFilePath = "<original file path>";
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            bool result = false;
            Action call = () => result = storageCommandHandler.OpenExistingProject(pathToSomeValidFile);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoetsproject.",
                "Bestaand Ringtoetsproject succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.IsTrue(result);

            Assert.IsInstanceOf<IProject>(projectOwner.Project);
            Assert.AreEqual(pathToSomeValidFile, projectOwner.ProjectFilePath);
            Assert.AreEqual(fileName, projectOwner.Project.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void ContinueIfHasChanges_NoProjectSet_ReturnsTrue()
        {
            // Setup
            var projectStorage = mocks.Stub<IStoreProject>();
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorage,
                projectOwner,
                mainWindowController);

            // Call
            bool result = storageCommandHandler.ContinueIfHasChanges();

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void ContinueIfHasChanges_ProjectSetNoChange_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectMock = mocks.StrictMock<IProject>();
            var projectStorageMock = mocks.Stub<IStoreProject>();
            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectOwnerMock,
                mainWindowController);

            // Call
            bool actionMaycontinue = storageCommandHandler.ContinueIfHasChanges();

            // Assert
            Assert.IsTrue(actionMaycontinue);

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ContinueIfHasChanges_ProjectSetWithChangeCancelPressed_ReturnsFalse()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectMock = mocks.Stub<IProject>();
            const string projectName = "Project";
            projectMock.Name = projectName;
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(projectMock));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges()).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectOwnerMock,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.ClickCancel();
            };

            // Call
            bool actionMaycontinue = storageCommandHandler.ContinueIfHasChanges();

            // Assert
            Assert.IsFalse(actionMaycontinue);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void ContinueIfHasChanges_ProjectSetWithChangeNoPressed_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectMock = mocks.Stub<IProject>();
            const string projectName = "Project";
            projectMock.Name = projectName;
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(projectMock));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges()).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectOwnerMock,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.SendCommand(MessageBoxTester.Command.No);
            };

            // Call
            bool actionMaycontinue = storageCommandHandler.ContinueIfHasChanges();

            // Assert
            Assert.IsTrue(actionMaycontinue);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void ContinueIfHasChanges_ProjectSetWithChangeYesPressed_ReturnsTrue()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectMock = mocks.Stub<IProject>();
            const string projectName = "Project";
            projectMock.Name = projectName;
            var projectFilePath = "some path";

            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(projectMock));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true).Repeat.Twice();
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges()).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            projectOwnerMock.ProjectFilePath = projectFilePath;

            projectStorageMock.Expect(p => p.SaveProjectAs(projectFilePath));
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectOwnerMock,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.SendCommand(MessageBoxTester.Command.Yes);
            };

            // Call
            bool actionMaycontinue = storageCommandHandler.ContinueIfHasChanges();

            // Assert
            Assert.IsTrue(actionMaycontinue);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void CreateNewProject_ProjectSetWithChangeCancelPressed_CancelAndLog()
        {
            // Setup
            var mainWindowController = mocks.Stub<IWin32Window>();
            var projectMock = mocks.Stub<IProject>();
            const string projectName = "Project";
            projectMock.Name = projectName;
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(ps => ps.StageProject(projectMock));
            projectStorageMock.Expect(ps => ps.HasStagedProject).Return(true);
            projectStorageMock.Expect(ps => ps.HasStagedProjectChanges()).IgnoreArguments().Return(true);
            projectStorageMock.Expect(ps => ps.UnstageProject());

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = string.Format("Sla wijzigingen in het project op: {0}?", projectName);

            var storageCommandHandler = new StorageCommandHandler(
                projectStorageMock,
                projectOwnerMock,
                mainWindowController);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.SendCommand(MessageBoxTester.Command.Cancel);
            };

            // Call
            Action call = () => storageCommandHandler.CreateNewProject();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.StorageCommandHandler_NewProject_Creating_new_project_cancelled, 1);

            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }
    }
}