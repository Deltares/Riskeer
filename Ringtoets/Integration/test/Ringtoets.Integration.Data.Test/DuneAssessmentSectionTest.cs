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
            Assert.AreEqual("Duintraject", section.Name);
            Assert.AreEqual("Referentielijn", section.ReferenceLine.Name);
            Assert.AreEqual("Faalkansverdeling", section.FailureMechanismContribution.Name);
            Assert.AreEqual("HR locatiedatabase", section.HydraulicBoundaryDatabase.Name);
            Assert.AreEqual("Duinen - Erosie", section.DuneErosionFailureMechanism.Name);
        }    
    }
}