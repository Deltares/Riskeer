using Core.Common.Base.Data;
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
            // SetUp
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();

            IGui guiMock = mocks.StrictMock<IGui>();
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            guiMock.Expect(x => x.Project).PropertyBehavior();
            guiMock.Expect(x => x.ProjectFilePath).PropertyBehavior();
            guiMock.Stub(x => x.RefreshGui());

            mocks.ReplayAll();

            StorageCommandHandler storageCommandHandler = new StorageCommandHandler(viewCommands, guiMock);

            // Call
            storageCommandHandler.CreateNewProject();

            // Assert
            Assert.IsInstanceOf<Project>(guiMock.Project);
            Assert.AreEqual("", guiMock.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateNewProject_SavedProjectThenNewProject_NewProjectAndPathAreSet()
        {
            // SetUp
            const string savedProjectPath = @"C:\savedProject.rtd";
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(null)).IgnoreArguments();

            Project projectMock = mocks.StrictMock<Project>();
            projectMock.Name = "test";
            projectMock.StorageId = 1234L;

            IGui guiMock = mocks.StrictMock<IGui>();
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            guiMock.Expect(x => x.Project).PropertyBehavior();
            guiMock.Expect(x => x.ProjectFilePath).PropertyBehavior();
            guiMock.Expect(x => x.Selection).PropertyBehavior();
            guiMock.Stub(x => x.RefreshGui());

            guiMock.Project = projectMock;
            guiMock.ProjectFilePath = savedProjectPath;

            mocks.ReplayAll();

            StorageCommandHandler storageCommandHandler = new StorageCommandHandler(viewCommands, guiMock);

            // Call
            storageCommandHandler.CreateNewProject();

            // Assert
            Assert.IsInstanceOf<Project>(guiMock.Project);
            Assert.AreNotEqual(projectMock, guiMock.Project);
            Assert.AreNotEqual(projectMock.StorageId, guiMock.Project.StorageId);
            Assert.AreNotEqual(savedProjectPath, guiMock.ProjectFilePath);
            Assert.AreEqual("", guiMock.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseProject_ProjectSet_NullsProjectSelectionAndPath()
        {
            // SetUp
            const string savedProjectPath = @"C:\savedProject.rtd";
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(null)).IgnoreArguments();

            Project projectMock = mocks.StrictMock<Project>();

            IGui guiMock = mocks.StrictMock<IGui>();
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            guiMock.Expect(x => x.Project).PropertyBehavior();
            guiMock.Expect(x => x.ProjectFilePath).PropertyBehavior();
            guiMock.Expect(x => x.Selection).PropertyBehavior();
            guiMock.Stub(x => x.RefreshGui());

            guiMock.Project = projectMock;
            guiMock.Selection = projectMock;
            guiMock.ProjectFilePath = savedProjectPath;

            mocks.ReplayAll();

            StorageCommandHandler storageCommandHandler = new StorageCommandHandler(viewCommands, guiMock);

            // Call
            storageCommandHandler.CloseProject();

            // Assert
            Assert.IsNull(guiMock.Project);
            Assert.IsNull(guiMock.Selection);
            Assert.AreEqual("", guiMock.ProjectFilePath);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseProject_EmptyProject_DoesNotThrowException()
        {
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(g => g.RemoveAllViewsForItem(null)).IgnoreArguments();

            Project projectMock = mocks.StrictMock<Project>();

            IGui guiMock = mocks.StrictMock<IGui>();
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectClosing += null).IgnoreArguments();
            guiMock.Expect(x => x.Project).PropertyBehavior();
            guiMock.Expect(x => x.ProjectFilePath).PropertyBehavior();
            guiMock.Expect(x => x.Selection).PropertyBehavior();
            guiMock.Stub(x => x.RefreshGui());

            guiMock.Project = projectMock;
            guiMock.Selection = guiMock.Project;

            mocks.ReplayAll();

            StorageCommandHandler storageCommandHandler = new StorageCommandHandler(viewCommands, guiMock);

            TestDelegate closeProject = () => storageCommandHandler.CloseProject();
            Assert.DoesNotThrow(closeProject);
        }
    }
}