using System.Linq;

using Core.Common.Base;

using NUnit.Framework;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class DikeAssessmentSectionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var section = new DikeAssessmentSection();

            // Assert
            Assert.IsInstanceOf<Observable>(section);
            Assert.IsInstanceOf<AssessmentSectionBase>(section);

            Assert.AreEqual("Dijktraject", section.Name);
            Assert.AreEqual("Referentielijn", section.ReferenceLine.Name);
            Assert.AreEqual("HR locatiedatabase", section.HydraulicBoundaryDatabase.Name);
            Assert.IsInstanceOf<PipingFailureMechanism>(section.PipingFailureMechanism);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SoilProfiles);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SurfaceLines);
            Assert.AreEqual("Dijken - Graserosie kruin en binnentalud", section.GrassErosionFailureMechanism.Name);
            Assert.AreEqual("Dijken - Macrostabiliteit binnenwaarts", section.MacrostabilityInwardFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Overslag en overloop", section.OvertoppingFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Niet sluiten", section.ClosingFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Constructief falen", section.FailingOfConstructionFailureMechanism.Name);
            Assert.AreEqual("Dijken - Steenbekledingen", section.StoneRevetmentFailureMechanism.Name);
            Assert.AreEqual("Dijken - Asfaltbekledingen", section.AsphaltRevetmentFailureMechanism.Name);
            Assert.AreEqual("Dijken - Grasbekledingen", section.GrassRevetmentFailureMechanism.Name);
        }

        [Test]
        public void Name_SetingNewValue_GetNewValue()
        {
            // Setup
            var section = new DikeAssessmentSection();

            const string newValue = "new value";

            // Call
            section.Name = newValue;

            // Assert
            Assert.AreEqual(newValue, section.Name);
        }

        [Test]
        public void GetFailureMechanisms_Always_ReturnAllFailureMechanisms()
        {
            // Setup
            var assessmentSection = new DikeAssessmentSection();

            // Call
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            // Assert
            Assert.AreEqual(9, failureMechanisms.Length);
            Assert.AreSame(assessmentSection.PipingFailureMechanism, failureMechanisms[0]);
            Assert.AreSame(assessmentSection.GrassErosionFailureMechanism, failureMechanisms[1]);
            Assert.AreSame(assessmentSection.MacrostabilityInwardFailureMechanism, failureMechanisms[2]);
            Assert.AreSame(assessmentSection.OvertoppingFailureMechanism, failureMechanisms[3]);
            Assert.AreSame(assessmentSection.ClosingFailureMechanism, failureMechanisms[4]);
            Assert.AreSame(assessmentSection.FailingOfConstructionFailureMechanism, failureMechanisms[5]);
            Assert.AreSame(assessmentSection.StoneRevetmentFailureMechanism, failureMechanisms[6]);
            Assert.AreSame(assessmentSection.AsphaltRevetmentFailureMechanism, failureMechanisms[7]);
            Assert.AreSame(assessmentSection.GrassRevetmentFailureMechanism, failureMechanisms[8]);
        }
    }
}