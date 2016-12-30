using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestGrassCoverErosionInwardsOutputTest
    {

        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var output = new TestGrassCoverErosionInwardsOutput();

            // Assert
            Assert.AreEqual(0.0, output.WaveHeight.Value);
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.IsInstanceOf<ProbabilityAssessmentOutput>(output.ProbabilityAssessmentOutput);
            Assert.IsInstanceOf<TestDikeHeightAssessmentOutput>(output.DikeHeightAssessmentOutput);
        }
    }
}