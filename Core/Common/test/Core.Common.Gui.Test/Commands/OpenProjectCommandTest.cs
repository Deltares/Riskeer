using System;

using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Gui.Commands;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class OpenProjectCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var command = new OpenProjectCommand();

            // Assert
            Assert.IsInstanceOf<IGuiCommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            Assert.IsFalse(command.LoadWasSuccesful);
            Assert.IsNull(command.Gui);
        }

        [Test]
        public void Execute_ValidFilePath_SetProjectAndReturnLoadWasSuccesfulTrue()
        {
            // Setup
            var expectedProject = new Project();
            const string path = "cool file.bro";

            var mocks = new MockRepository();
            var storageMock = mocks.StrictMock<IStoreProject>();
            storageMock.Expect(s => s.LoadProject(path)).Return(expectedProject);

            var guiMock = mocks.Stub<IGui>();
            guiMock.Project = null;
            guiMock.Stub(g => g.Storage).Return(storageMock);
            guiMock.Expect(g => g.UpdateTitle());
            mocks.ReplayAll();

            var command = new OpenProjectCommand
            {
                Gui = guiMock
            };

            // Call
            Action call = () => command.Execute(path);
            
            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoets project.",
                "Bestaand Ringtoets project succesvol geopend."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);
            Assert.AreSame(expectedProject, guiMock.Project);
            Assert.AreEqual("cool file", expectedProject.Name);
            Assert.AreEqual(path, guiMock.ProjectFilePath);
            Assert.AreSame(expectedProject, guiMock.Selection);
            Assert.True(command.LoadWasSuccesful);
        }

        [Test]
        public void Execute_ValidFilePathAndAlreadyHasProject_CloseViewsOldProjectAndSetProjectAndReturnLoadWasSuccesfulTrue()
        {
            // Setup
            var originalProject = new Project();
            var expectedProject = new Project();
            const string path = "cool file.bro";

            var mocks = new MockRepository();
            var storageMock = mocks.StrictMock<IStoreProject>();
            storageMock.Expect(s => s.LoadProject(path)).Return(expectedProject);

            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.RemoveAllViewsForItem(originalProject));

            var guiMock = mocks.Stub<IGui>();
            guiMock.Project = originalProject;
            guiMock.Stub(g => g.Storage).Return(storageMock);
            guiMock.Expect(g => g.UpdateTitle());
            guiMock.CommandHandler = commandHandlerMock;
            mocks.ReplayAll();

            var command = new OpenProjectCommand
            {
                Gui = guiMock
            };

            // Call
            command.Execute(path);

            // Assert
            Assert.AreSame(expectedProject, guiMock.Project);
            Assert.AreEqual("cool file", expectedProject.Name);
            Assert.AreEqual(path, guiMock.ProjectFilePath);
            Assert.AreSame(expectedProject, guiMock.Selection);
            Assert.True(command.LoadWasSuccesful);
        }

        [Test]
        public void Execute_StorageThrowsArgumentException_DoNotAffectProjectAndViewAndLogError()
        {
            ExecuteOpenProjectCommandAndThrowExceptionDuringLoad(new ArgumentException("<some error message>"));
        }

        [Test]
        public void Execute_StorageThrowsCouldNotConnectException_DoNotAffectProjectAndViewAndLogError()
        {
            ExecuteOpenProjectCommandAndThrowExceptionDuringLoad(new CouldNotConnectException("<some error message>"));
        }

        [Test]
        public void Execute_StorageThrowsStorageValidationException_DoNotAffectProjectAndViewAndLogError()
        {
            ExecuteOpenProjectCommandAndThrowExceptionDuringLoad(new StorageValidationException("<some error message>"));
        }

        private static void ExecuteOpenProjectCommandAndThrowExceptionDuringLoad(Exception exception)
        {
            // Setup
            const string originalName = "original name";
            const string originalPath = originalName + ".tof";

            var originalProject = new Project
            {
                Name = originalName
            };
            const string path = "cool file.bro";
            string errorMessage = exception.Message;

            var mocks = new MockRepository();
            var storageMock = mocks.StrictMock<IStoreProject>();
            storageMock.Expect(s => s.LoadProject(path)).Throw(exception);

            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.RemoveAllViewsForItem(originalProject));

            var guiMock = mocks.Stub<IGui>();
            guiMock.Project = originalProject;
            guiMock.Stub(g => g.Storage).Return(storageMock);
            guiMock.Expect(g => g.UpdateTitle());
            guiMock.CommandHandler = commandHandlerMock;
            guiMock.ProjectFilePath = originalPath;
            mocks.ReplayAll();

            var command = new OpenProjectCommand
            {
                Gui = guiMock
            };

            // Call
            Action call = () => command.Execute(path);

            // Assert
            var expectedMessages = new[]
            {
                "Openen van bestaand Ringtoets project.",
                errorMessage,
                "Het is niet gelukt om het Ringtoets project te laden."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 3);
            Assert.AreSame(originalProject, guiMock.Project,
                           "Original project should still exist.");
            Assert.AreEqual(originalName, originalProject.Name,
                            "Name should not be updated");
            Assert.AreEqual(originalPath, guiMock.ProjectFilePath,
                            "Project file path should remain unchanged.");
            Assert.False(command.LoadWasSuccesful);
        }
    }
}