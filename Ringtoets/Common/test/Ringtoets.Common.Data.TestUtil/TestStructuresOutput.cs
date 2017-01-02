using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Simple <see cref="ProbabilityAssessmentOutput"/> that can be used for tests where actual output
    /// values are not important.
    /// </summary>
    public class TestStructuresOutput : ProbabilityAssessmentOutput
    {
        /// <summary>
        /// Creates new instance of <see cref="TestStructuresOutput"/>.
        /// </summary>
        public TestStructuresOutput() : base(0, 0, 0, 0, 0) {}
    }
}