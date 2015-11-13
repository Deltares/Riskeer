using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Demo.Commands;
using Ringtoets.Piping.Data;

namespace Ringtoets.Demo.Test.Commands
{
    [TestFixture]
    public class AddNewDemoProjectCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var command = new AddNewDemoProjectCommand();

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsInstanceOf<IGuiCommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            Assert.IsNull(command.Gui);
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewDemoProjectToRootProject()
        {
            // Setup
            var project = new Project();

            var mocks = new MockRepository();
            var applicationMock = mocks.Stub<IApplication>();
            applicationMock.Stub(a => a.Project).Return(project);
            var guiMock = mocks.Stub<IGui>();
            guiMock.Application = applicationMock;

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var command = new AddNewDemoProjectCommand();
            command.Gui = guiMock;

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoProject = (AssessmentSection) project.Items[0];
            Assert.AreEqual("Demo traject", demoProject.Name);

            Assert.AreEqual(1, demoProject.PipingFailureMechanism.SoilProfiles);
            Assert.AreEqual(1, demoProject.PipingFailureMechanism.SurfaceLines);

            Assert.AreEqual(1, demoProject.PipingFailureMechanism.Calculations);
            mocks.VerifyAll();
        }
    }
}