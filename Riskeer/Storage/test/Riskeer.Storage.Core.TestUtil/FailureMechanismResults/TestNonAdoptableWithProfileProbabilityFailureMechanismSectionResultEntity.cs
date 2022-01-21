using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil.FailureMechanismResults
{
    /// <summary>
    /// Simple implementation of <see cref="INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> that can be used in tests.
    /// </summary>
    public class TestNonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity : INonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity
    {
        public byte IsRelevant { get; set; }
        public byte InitialFailureMechanismResultType { get; set; }
        public double? ManualInitialFailureMechanismResultSectionProbability { get; set; }
        public byte FurtherAnalysisNeeded { get; set; }
        public double? RefinedSectionProbability { get; set; }
        public double? ManualInitialFailureMechanismResultProfileProbability { get; set; }
        public double? RefinedProfileProbability { get; set; }
    }
}