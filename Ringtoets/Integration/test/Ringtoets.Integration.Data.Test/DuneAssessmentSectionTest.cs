using System.Linq;

using Core.Common.Base;

using NUnit.Framework;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class DuneAssessmentSectionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var section = new DuneAssessmentSection();

            // Assert
            Assert.IsInstanceOf<Observable>(section);
            Assert.IsInstanceOf<AssessmentSectionBase>(section);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);

            Assert.AreEqual("Duintraject", section.Name);
            Assert.AreEqual("Referentielijn", section.ReferenceLine.Name);
            Assert.AreEqual("HR locatiedatabase", section.HydraulicBoundaryDatabase.Name);
            Assert.AreEqual("Duinen - Erosie", section.DuneErosionFailureMechanism.Name);
        }

        [Test]
        public void GetFailureMechanisms_Always_ReturnAllFailureMechanisms()
        {
            // Setup
            var assessmentSection = new DuneAssessmentSection();

            // Call
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            // Assert
            Assert.AreEqual(1, failureMechanisms.Length);
            Assert.AreSame(assessmentSection.DuneErosionFailureMechanism, failureMechanisms[0]);
        }
    }
}