using System.Linq;

using Core.Common.Base;

using NUnit.Framework;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Properties;

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

            Assert.AreEqual(70, section.DuneErosionFailureMechanism.Contribution);
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

        [Test]
        public void FailureMechanismContribution_Always_ReturnInitializedFailureMechanismContribution()
        {
            // Setup
            var assessmentSection = new DuneAssessmentSection();

            // Call
            var contribution = assessmentSection.FailureMechanismContribution.Distribution.ToArray();

            // Assert
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            Assert.AreEqual(2, contribution.Length);

            Assert.AreEqual(failureMechanisms[0].Name, contribution[0].Assessment);
            Assert.AreEqual(failureMechanisms[0].Contribution, contribution[0].Contribution);
            Assert.AreEqual((30000 / contribution[0].Contribution) * 100, contribution[0].ProbabilitySpace);

            Assert.AreEqual(Resources.OtherFailureMechanism_DisplayName, contribution[1].Assessment);
            Assert.AreEqual(30, contribution[1].Contribution);
            Assert.AreEqual((30000 / contribution[1].Contribution) * 100, 100000);
        }
    }
}