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
            var result = new TestGrassCoverErosionInwardsOutput();

            // Assert
            Assert.AreEqual(0.0, result.WaveHeight.Value);
            Assert.IsTrue(result.IsOvertoppingDominant);
            Assert.IsInstanceOf<ProbabilityAssessmentOutput>(result.ProbabilityAssessmentOutput);
            Assert.IsInstanceOf<TestDikeHeightAssessmentOutput>(result.DikeHeightAssessmentOutput);
        }
    }
}