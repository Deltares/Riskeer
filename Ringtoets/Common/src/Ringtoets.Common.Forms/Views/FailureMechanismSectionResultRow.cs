using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.Views
{
    internal class FailureMechanismSectionResultRow
    {
        public FailureMechanismSectionResultRow(FailureMechanismSectionResult failureMechanismSectionResult)
        {
            FailureMechanismSectionResult = failureMechanismSectionResult;
        }

        public string Name
        {
            get
            {
                return FailureMechanismSectionResult.Section.Name;
            }
        }

        public FailureMechanismSectionResult FailureMechanismSectionResult { get; private set; }
    }
}