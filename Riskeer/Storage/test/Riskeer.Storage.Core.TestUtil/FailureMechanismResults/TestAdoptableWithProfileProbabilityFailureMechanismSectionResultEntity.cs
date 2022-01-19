using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil.FailureMechanismResults
{
    /// <summary>
    /// Simple implementation of <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> that can be used in tests.
    /// </summary>
    public class TestAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity : IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity
    {
        public byte IsRelevant { get; set; }
        public byte AdoptableInitialFailureMechanismResultType { get; set; }
        public double? ManualInitialFailureMechanismResultSectionProbability { get; set; }
        public byte FurtherAnalysisNeeded { get; set; }
        public double? RefinedSectionProbability { get; set; }
        public double? ManualInitialFailureMechanismResultProfileProbability { get; set; }
        public byte ProbabilityRefinementType { get; set; }
        public double? RefinedProfileProbability { get; set; }
    }
}