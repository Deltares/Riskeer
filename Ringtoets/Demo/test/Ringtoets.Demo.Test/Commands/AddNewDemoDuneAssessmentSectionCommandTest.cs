using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
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

            var applicationCore = new ApplicationCore();
            Expect.Call(guiMock.ApplicationCore).Return(applicationCore).Repeat.Any();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var project = new Project();

            guiMock.Project = project;

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