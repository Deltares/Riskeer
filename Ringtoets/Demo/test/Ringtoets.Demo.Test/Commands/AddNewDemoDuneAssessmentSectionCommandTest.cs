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
        public void Execute_GuiIsProperlyInitialized_AddNewDuneAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var mocks = new MockRepository();

            var guiMock = mocks.Stub<IGui>();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var project = new Project();
            var applicationCore = new ApplicationCore();

            guiMock.Project = project;
            guiMock.ApplicationCore = applicationCore;

            var command = new AddNewDemoDuneAssessmentSectionCommand
            {
                Gui = guiMock
            };

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoAssessmentSection = (DuneAssessmentSection) project.Items[0];
            Assert.AreEqual("Demo duintraject", demoAssessmentSection.Name);
            mocks.VerifyAll();
        }
    }
}