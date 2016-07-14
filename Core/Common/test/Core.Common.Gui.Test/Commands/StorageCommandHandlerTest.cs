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
using Core.Common.Base;
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
            var viewCommands = mocks.StrictMock<IViewCommands>();

            var projectStorage = mocks.Stub<IStoreProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing -= null).IgnoreArguments();

            var applicationSelection = mocks.Stub<IApplicationSelection>();

            var mainWindowController = mocks.Stub<IMainWindowController>();
            mainWindowController.Expect(c => c.RefreshGui());

            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                         mainWindowController, viewCommands))
            {
                // Call
                Action call = () => storageCommandHandler.CreateNewProject();

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van nieuw Ringtoetsproject.",
                    "Nieuw Ringtoetsproject succesvol geopend."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);

                Assert.IsInstanceOf<Project>(projectOwner.Project);
                Assert.AreEqual("", projectOwner.ProjectFilePath);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CreateNewProject_SavedProjectThenNewProject_NewProjectAndPathAreSet()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";

            var projectMock = mocks.StrictMock<Project>();
            projectMock.Name = "test";
            projectMock.StorageId = 1234L;

            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(projectMock)).IgnoreArguments();

            var projectStorage = mocks.Stub<IStoreProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = savedProjectPath;
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing -= null).IgnoreArguments();

            var applicationSelection = mocks.Stub<IApplicationSelection>();

            var mainWindowController = mocks.Stub<IMainWindowController>();
            mainWindowController.Expect(c => c.RefreshGui()).Repeat.AtLeastOnce();

            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                         mainWindowController, viewCommands))
            {
                // Call
                Action call = () => storageCommandHandler.CreateNewProject();

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van nieuw Ringtoetsproject.",
                    "Nieuw Ringtoetsproject succesvol geopend."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);

                Assert.IsInstanceOf<Project>(projectOwner.Project);
                Assert.AreNotEqual(projectMock, projectOwner.Project);
                Assert.AreNotEqual(projectMock.StorageId, projectOwner.Project.StorageId);
                Assert.AreNotEqual(savedProjectPath, projectOwner.ProjectFilePath);
                Assert.AreEqual("", projectOwner.ProjectFilePath);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_ProjectIsNull_DoNothingAndReturnFalse()
        {
            // Setup
            var projectStorage = mocks.StrictMock<IStoreProject>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var mainWindowController = mocks.StrictMock<IMainWindowController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Expect(po => po.Project).Return(null);
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                var result = commandHandler.SaveProject();

                // Assert
                Assert.IsFalse(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectThrowsStorageException_AbortSaveAndReturnFalse()
        {
            // Setup
            const string someValidFilePath = "<some valid file path>";
            var project = new Project();

            const string exceptionMessage = "<some descriptive exception message>";

            var projectStorage = mocks.StrictMock<IStoreProject>();
            projectStorage.Expect(ps => ps.SaveProject(someValidFilePath, project)).
                           Throw(new StorageException(exceptionMessage, new Exception("l33t h4xor!")));
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var mainWindowController = mocks.StrictMock<IMainWindowController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = project;
            projectOwner.ProjectFilePath = someValidFilePath;
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = true;
                Action call = () => result = commandHandler.SaveProject();

                // Assert
                var expectedMessages = new[]
                {
                    exceptionMessage,
                    "Het is niet gelukt om het Ringtoetsproject op te slaan."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
                Assert.IsFalse(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void SaveProject_SavingProjectIsSuccessful_LogSuccessAndReturnTrue()
        {
            // Setup
            var project = new Project("<some cool name>");
            const string someValidFilePath = "<some valid filepath>";

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Expect(ps => ps.SaveProject(someValidFilePath, project))
                          .Return(42);
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var mainWindowController = mocks.StrictMock<IMainWindowController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = project;
            projectOwner.ProjectFilePath = someValidFilePath;
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = false;
                Action call = () => result = commandHandler.SaveProject();

                // Assert
                var expectedMessage = string.Format("Het Ringtoetsproject '{0}' is succesvol opgeslagen.",
                                                    project.Name);
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
                Assert.IsTrue(result);
            }
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
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = true;
                Action call = () => result = commandHandler.OpenExistingProject(pathToSomeInvalidFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    goodErrorMessageText,
                    "Het is niet gelukt om het Ringtoetsproject te laden."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 3);
                Assert.IsFalse(result);
            }
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
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = true;
                Action call = () => result = commandHandler.OpenExistingProject(pathToSomeInvalidFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Het is niet gelukt om het Ringtoetsproject te laden."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
                Assert.IsFalse(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void OpenExistingProject_OpeningProjectWhenNoProjectHasBeenLoaded_SetNewlyLoadedProjectAndReturnTrue()
        {
            // Setup
            const string fileName = "newProject";
            string pathToSomeInvalidFile = string.Format("C://folder/directory/{0}.rtd",
                                                         fileName);
            var loadedProject = new Project();

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Return(loadedProject);
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();

            var mainWindowController = mocks.Stub<IMainWindowController>();
            mainWindowController.Expect(c => c.RefreshGui());

            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            loadedProject.Attach(observer);

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = false;
                Action call = () => result = commandHandler.OpenExistingProject(pathToSomeInvalidFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Bestaand Ringtoetsproject succesvol geopend."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
                Assert.IsTrue(result);
            }
            Assert.IsInstanceOf<Project>(projectOwner.Project);
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
            var loadedProject = new Project();
            var originalProject = new Project("Original");

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeValidFile))
                          .Return(loadedProject);
            projectStorage.Stub(ps => ps.CloseProject());
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = originalProject;

            var mainWindowController = mocks.Stub<IMainWindowController>();
            mainWindowController.Expect(c => c.RefreshGui()).Repeat.Twice();

            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(originalProject));

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = originalProject;
            projectOwner.ProjectFilePath = "<original file path>";
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            loadedProject.Attach(observer);

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = false;
                Action call = () => result = commandHandler.OpenExistingProject(pathToSomeValidFile);

                // Assert
                var expectedMessages = new[]
                {
                    "Openen van bestaand Ringtoetsproject.",
                    "Bestaand Ringtoetsproject succesvol geopend."
                };
                TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
                Assert.IsTrue(result);
            }
            Assert.IsInstanceOf<Project>(projectOwner.Project);
            Assert.AreEqual(pathToSomeValidFile, projectOwner.ProjectFilePath);
            Assert.AreEqual(fileName, projectOwner.Project.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void ContinueIfHasChanges_NoProjectSet_ReturnsTrue()
        {
            // Setup
            var projectStorage = mocks.Stub<IStoreProject>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, viewCommands))
            {
                // Call
                bool result = commandHandler.ContinueIfHasChanges();

                // Assert
                Assert.IsTrue(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContinueIfHasChanges_ProjectSetNoChange_ReturnsTrue()
        {
            // Setup
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var projectMock = mocks.StrictMock<Project>();
            var projectStorageMock = mocks.Stub<IStoreProject>();
            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, viewCommandsMock))
            {
                // Call
                bool actionMaycontinue = storageCommandHandler.ContinueIfHasChanges();

                // Assert
                Assert.IsTrue(actionMaycontinue);
            }
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ContinueIfHasChanges_ProjectSetWithChangeCancelPressed_ReturnsFalse()
        {
            // Setup
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var projectMock = mocks.StrictMock<Project>();
            projectMock.StorageId = 1234L;
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(true);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = "Sla wijzigingen in het project op: Project?";

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, viewCommandsMock))
            {
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
            }
            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void ContinueIfHasChanges_ProjectSetWithChangeNoPressed_ReturnsTrue()
        {
            // Setup
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var projectMock = mocks.StrictMock<Project>();
            projectMock.StorageId = 1234L;
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(true);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = "Sla wijzigingen in het project op: Project?";

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, viewCommandsMock))
            {
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
            }
            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void ContinueIfHasChanges_ProjectSetWithChangeYesPressed_ReturnsTrue()
        {
            // Setup
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var projectMock = mocks.StrictMock<Project>();
            projectMock.StorageId = 1234L;
            var projectFilePath = "some path";

            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(true);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            projectOwnerMock.ProjectFilePath = projectFilePath;

            projectStorageMock.Expect(p => p.SaveProject(projectFilePath, projectMock)).Return(1);
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = "Sla wijzigingen in het project op: Project?";

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, viewCommandsMock))
            {
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
            }
            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }

        [Test]
        [RequiresSTA]
        public void CreateNewProject_ProjectSetWithChangeCancelPressed_CancelAndLog()
        {
            // Setup
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var projectMock = mocks.StrictMock<Project>();
            projectMock.StorageId = 1234L;
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(true);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = "Sla wijzigingen in het project op: Project?";

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, viewCommandsMock))
            {
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
            }
            mocks.VerifyAll();
            Assert.AreEqual(expectedMessage, messageBoxText);
        }
    }
}