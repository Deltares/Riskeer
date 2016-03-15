using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;

namespace Demo.Ringtoets.Test.Commands
{
    [TestFixture]
    public class AddNewDemoDuneAssessmentSectionCommandTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            mocks.ReplayAll();

            // Call
            var command = new AddNewDemoDuneAssessmentSectionCommand(projectOwner);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_GuiIsProperlyInitialized_AddNewDuneAssessmentSectionWithDemoDataToRootProject()
        {
            // Setup
            var project = new Project();

            var mocks = new MockRepository();
            var projectOwnerStub = mocks.Stub<IProjectOwner>();
            projectOwnerStub.Project = project;

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var command = new AddNewDemoDuneAssessmentSectionCommand(projectOwnerStub);

            project.Attach(observerMock);

            // Call
            command.Execute();

            // Assert
            Assert.AreEqual(1, project.Items.Count);
            var demoAssessmentSection = (DuneAssessmentSection) project.Items[0];
            Assert.AreEqual("Demo duintraject", demoAssessmentSection.Name);

            Assert.AreEqual(2380, demoAssessmentSection.ReferenceLine.Points.Count());

            Assert.IsNotEmpty(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath);
            Assert.IsTrue(File.Exists(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath));
            Assert.IsTrue(File.Exists(Path.Combine(Path.GetDirectoryName(demoAssessmentSection.HydraulicBoundaryDatabase.FilePath), "HLCD.sqlite")));
            var hydraulicBoundaryLocations = demoAssessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(18, hydraulicBoundaryLocations.Length);

            foreach (var failureMechanism in demoAssessmentSection.GetFailureMechanisms())
            {
                Assert.AreEqual(283, failureMechanism.Sections.Count());
            }

            mocks.VerifyAll();
        }
    }
}