using MathNet.Numerics.Distributions;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test
{
    [TestFixture]
    public class TestTargetProbabilityCalculationInputTest
    {
        [Test]
        [TestCase(2, 10000)]
        [TestCase(-50, 1)]
        [TestCase(0, -90)]
        [TestCase(200000, double.NaN)]
        public void Constructed_UsingDifferentNormAndLocationId_ReturnDifferentBetaAndDefaultValues(int locationId, double norm)
        {
            // Setup
            var expectedBeta = -Normal.InvCDF(0.0, 1.0, 1.0 / norm);

            // Call
            var testInput = new TestTargetProbabilityCalculationInput(locationId, norm);

            // Assert
            Assert.IsInstanceOf<TargetProbabilityCalculationInput>(testInput);
            Assert.AreEqual(locationId, testInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesPiping, testInput.FailureMechanismType);
            Assert.AreEqual(2, testInput.CalculationTypeId);
            Assert.AreEqual(5, testInput.VariableId);
            Assert.AreEqual(1, testInput.Section.SectionId);
            CollectionAssert.IsEmpty(testInput.Variables);
            CollectionAssert.IsEmpty(testInput.ProfilePoints);
            CollectionAssert.IsEmpty(testInput.ForelandsPoints);
            Assert.IsNull(testInput.BreakWater);
            Assert.AreEqual(expectedBeta, testInput.Beta);
        }
    }
}