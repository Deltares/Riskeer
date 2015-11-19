using System.Linq;

using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Demo.Commands;
using Ringtoets.Integration.Data;

namespace Ringtoets.Demo.Test.Commands
{
    [TestFixture]
    public class AddNewDemoDuneAssessmentSectionCommandTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var command = new AddNewDemoDuneAssessmentSectionCommand();

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

            var command = new AddNewDemoDuneAssessmentSectionCommand();
            command.Gui = guiMock;

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoAssessmentSection = (DuneAssessmentSection)project.Items[0];
            Assert.AreEqual("Demo duintraject", demoAssessmentSection.Name);
            mocks.VerifyAll();
        }
    }
}