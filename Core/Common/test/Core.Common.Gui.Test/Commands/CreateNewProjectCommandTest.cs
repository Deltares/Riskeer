using Core.Common.Base.Data;
using Core.Common.Gui.Commands;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class CreateNewProjectCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup

            // Call
            var command = new CreateNewProjectCommand();

            // Assert
            Assert.IsInstanceOf<IGuiCommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            Assert.IsNull(command.Gui);
        }

        [Test]
        public void Execute_GuiHasNoProject_SetProjectAndSelectionAndTitle()
        {
            // Setup
            var mocks = new MockRepository();
            var guiMock = mocks.Stub<IGui>();
            guiMock.Project = null;
            guiMock.Expect(g => g.UpdateTitle());
            mocks.ReplayAll();

            var command = new CreateNewProjectCommand
            {
                Gui = guiMock
            };

            // Call
            command.Execute();

            // Assert
            Assert.IsInstanceOf<Project>(guiMock.Project);
            Assert.AreSame(guiMock.Project, guiMock.Selection);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_GuiHasProject_ReplaceProjectAndSelectionAndTitleAndCloseAllViews()
        {
            // Setup
            var originalProject = new Project();

            var mocks = new MockRepository();
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            guiCommandHandlerMock.Expect(ch => ch.RemoveAllViewsForItem(originalProject));

            var guiMock = mocks.Stub<IGui>();
            guiMock.CommandHandler = guiCommandHandlerMock;
            guiMock.Project = originalProject;
            guiMock.Expect(g => g.UpdateTitle());
            mocks.ReplayAll();

            var command = new CreateNewProjectCommand
            {
                Gui = guiMock
            };

            // Call
            command.Execute();

            // Assert
            Assert.IsInstanceOf<Project>(guiMock.Project);
            Assert.AreNotSame(originalProject, guiMock.Project);
            Assert.AreSame(guiMock.Project, guiMock.Selection);
            mocks.VerifyAll();
        }
    }
}