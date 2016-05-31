using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class ClosingStructureResultView : ArbitraryProbabilityFailureMechanismResultView
    {
        protected override object CreateFailureMechanismSectionResultRow(ArbitraryProbabilityFailureMechanismSectionResult sectionResult)
        {
            return new ArbitraryProbabilityFailureMechanismSectionResultRow(sectionResult);
        }
    }
}