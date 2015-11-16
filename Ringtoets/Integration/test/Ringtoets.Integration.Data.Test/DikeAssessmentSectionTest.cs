using NUnit.Framework;

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
            Assert.AreEqual("Dijktraject", section.Name);
            Assert.AreEqual("Referentielijn", section.ReferenceLine.Name);
            Assert.AreEqual("Faalkansverdeling", section.FailureMechanismContribution.Name);
            Assert.AreEqual("HR locatiedatabase", section.HydraulicBoundaryDatabase.Name);
            Assert.IsInstanceOf<PipingFailureMechanism>(section.PipingFailureMechanism);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SoilProfiles);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SurfaceLines);
            Assert.AreEqual("Dijken - Graserosie kruin en binnentalud", section.GrassErosionFailureMechanism.Name);
            Assert.AreEqual("Dijken - Macrostabiliteit binnenwaarts", section.MacrostabilityInwardFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Overslag en overloop", section.OvertoppingFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Niet sluiten", section.ClosingFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Constructief falen", section.FailingOfConstructionFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Steenbekledingen", section.StoneRevetmentFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Asfaltbekledingen", section.AsphaltRevetmentFailureMechanism.Name);
            Assert.AreEqual("Kunstwerken - Grasbekledingen", section.GrassRevetmentFailureMechanism.Name);
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
    }
}