using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class StorageCommandHandlerTest
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
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();

            var projectStorage = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands);

            // Call
            storageCommandHandler.CreateNewProject();

            // Assert
            Assert.IsInstanceOf<Project>(projectOwner.Project);
            Assert.AreEqual("", projectOwner.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateNewProject_SavedProjectThenNewProject_NewProjectAndPathAreSet()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(null)).IgnoreArguments();

            Project projectMock = mocks.StrictMock<Project>();
            projectMock.Name = "test";
            projectMock.StorageId = 1234L;

            var projectStorage = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = savedProjectPath;
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands);

            // Call
            storageCommandHandler.CreateNewProject();

            // Assert
            Assert.IsInstanceOf<Project>(projectOwner.Project);
            Assert.AreNotEqual(projectMock, projectOwner.Project);
            Assert.AreNotEqual(projectMock.StorageId, projectOwner.Project.StorageId);
            Assert.AreNotEqual(savedProjectPath, projectOwner.ProjectFilePath);
            Assert.AreEqual("", projectOwner.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseProject_ProjectSet_NullsProjectSelectionAndPath()
        {
            // Setup
            const string savedProjectPath = @"C:\savedProject.rtd";
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(null)).IgnoreArguments();

            Project projectMock = mocks.StrictMock<Project>();

            var projectStorage = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.ProjectFilePath = savedProjectPath;
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = projectMock;
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands);

            // Call
            storageCommandHandler.CloseProject();

            // Assert
            Assert.IsNull(projectOwner.Project);
            Assert.IsNull(applicationSelection.Selection);
            Assert.AreEqual("", projectOwner.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseProject_EmptyProject_DoesNotThrowException()
        {
            // Setup
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(null)).IgnoreArguments();

            Project projectMock = mocks.StrictMock<Project>();

            var projectStorage = mocks.Stub<IStoreProject>();
            var projectOwner = mocks.Stub<IProjectOwner>();
            projectOwner.Project = projectMock;
            projectOwner.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            projectOwner.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            var applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Selection = projectMock;
            var mainWindowController = mocks.Stub<IMainWindowController>();
            var toolViewController = mocks.Stub<IToolViewController>();

            mocks.ReplayAll();

            var storageCommandHandler = new StorageCommandHandler(projectStorage, projectOwner, applicationSelection,
                                                                  mainWindowController, toolViewController, viewCommands);

            // Call
            TestDelegate closeProject = () => storageCommandHandler.CloseProject();

            // Assert
            Assert.DoesNotThrow(closeProject);
        }
    }
}