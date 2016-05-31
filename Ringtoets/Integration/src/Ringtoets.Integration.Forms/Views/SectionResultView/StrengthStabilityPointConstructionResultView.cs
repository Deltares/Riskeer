using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class StrengthStabilityPointConstructionResultView : ArbitraryProbabilityFailureMechanismResultView
    {
        protected override object CreateFailureMechanismSectionResultRow(ArbitraryProbabilityFailureMechanismSectionResult sectionResult)
        {
            return new ArbitraryProbabilityFailureMechanismSectionResultRow(sectionResult);
        }
    }
}