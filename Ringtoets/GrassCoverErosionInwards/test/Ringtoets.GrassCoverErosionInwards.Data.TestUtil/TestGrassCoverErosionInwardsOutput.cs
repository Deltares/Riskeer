using Ringtoets.Common.Data.Probability;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Simple implementation of a <see cref="GrassCoverErosionInwardsOutput"/>, which can be
    /// used in tests where actual output values are not important.
    /// </summary>
    public class TestGrassCoverErosionInwardsOutput : GrassCoverErosionInwardsOutput
    {
        public TestGrassCoverErosionInwardsOutput() : base(0.0, true, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                           new TestDikeHeightAssessmentOutput(0)) {}
    }
}