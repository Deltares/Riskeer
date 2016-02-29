using System;
using Core.Common.Base;
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

            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommands))
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

            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommands))
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
        public void CloseProject_ProjectSet_NullsProjectSelectionAndPath()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";

            var projectMock = mocks.StrictMock<Project>();

            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(projectMock));

            var projectStorage = mocks.Stub<IStoreProject>();

            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = savedProjectPath;
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing -= null).IgnoreArguments();

            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = projectMock;

            var mainWindowController = mocks.Stub<IMainWindowController>();
            mainWindowController.Expect(c => c.RefreshGui());

            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommands))
            {
                // Call
                storageCommandHandler.CloseProject();

                // Assert
                Assert.IsNull(projectOwner.Project);
                Assert.IsNull(applicationSelection.Selection);
                Assert.AreEqual("", projectOwner.ProjectFilePath);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseProject_ProjectNotYetSet_DoesNotThrowException()
        {
            // Setup
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectStorage = mocks.Stub<IStoreProject>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var toolViewController = mocks.Stub<IToolViewController>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing -= null).IgnoreArguments();

            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommands))
            {
                // Call
                TestDelegate closeProject = () => storageCommandHandler.CloseProject();

                // Assert
                Assert.DoesNotThrow(closeProject);
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
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Expect(po => po.Project).Return(null);
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands))
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
            var toolViewController = mocks.StrictMock<IToolViewController>();
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
                                                                  mainWindowController, toolViewController, viewCommands))
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
            var toolViewController = mocks.StrictMock<IToolViewController>();
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
                                                                  mainWindowController, toolViewController, viewCommands))
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
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var mainWindowController = mocks.StrictMock<IMainWindowController>();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands))
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
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var mainWindowController = mocks.StrictMock<IMainWindowController>();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.StrictMock<IProjectOwner>();
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands))
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

            var toolViewController = mocks.StrictMock<IToolViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing += null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectOpened -= null).IgnoreArguments();
            projectOwner.Stub(po => po.ProjectClosing -= null).IgnoreArguments();
            mocks.ReplayAll();

            loadedProject.Attach(observer);

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands))
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
            string pathToSomeInvalidFile = string.Format("C://folder/directory/{0}.rtd",
                                                         fileName);
            var loadedProject = new Project();
            var originalProject = new Project("Original");

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var projectStorage = mocks.Stub<IStoreProject>();
            projectStorage.Stub(ps => ps.LoadProject(pathToSomeInvalidFile))
                          .Return(loadedProject);
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = originalProject;

            var mainWindowController = mocks.Stub<IMainWindowController>();
            mainWindowController.Expect(c => c.RefreshGui()).Repeat.Twice();

            var toolViewController = mocks.StrictMock<IToolViewController>();
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
                                                                  mainWindowController, toolViewController, viewCommands))
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
            Assert.IsNull(applicationSelection.Selection);
            mocks.VerifyAll();
        }

        [Test]
        public void ContinueIfHasChanges_NoProjectSet_ReturnsTrue()
        {
            // Setup
            var projectStorage = mocks.Stub<IStoreProject>();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            using (var commandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands))
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
            var toolViewController = mocks.Stub<IToolViewController>();
            var projectMock = mocks.StrictMock<Project>();
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(false);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommandsMock))
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
            var toolViewController = mocks.Stub<IToolViewController>();
            var projectMock = mocks.StrictMock<Project>();
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(true);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = "Sla wijzigingen in het project op: Project?";

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommandsMock))
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
            var toolViewController = mocks.Stub<IToolViewController>();
            var projectMock = mocks.StrictMock<Project>();
            var projectStorageMock = mocks.Stub<IStoreProject>();
            projectStorageMock.Expect(p => p.HasChanges(null)).IgnoreArguments().Return(true);

            var projectOwnerMock = mocks.Stub<IProjectOwner>();
            projectOwnerMock.Project = projectMock;
            mocks.ReplayAll();

            string messageBoxText = null;
            string expectedMessage = "Sla wijzigingen in het project op: Project?";

            using (var storageCommandHandler = new StorageCommandHandler(projectStorageMock, projectOwnerMock, applicationSelection,
                                                                         mainWindowController, toolViewController, viewCommandsMock))
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
            var toolViewController = mocks.Stub<IToolViewController>();
            var projectMock = mocks.StrictMock<Project>();

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
                                                                         mainWindowController, toolViewController, viewCommandsMock))
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
    }
}