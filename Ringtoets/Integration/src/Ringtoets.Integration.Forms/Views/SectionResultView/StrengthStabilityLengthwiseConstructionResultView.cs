using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class StrengthStabilityLengthwiseConstructionResultView : NumericFailureMechanismResultView
    {
        protected override object CreateFailureMechanismSectionResultRow(NumericFailureMechanismSectionResult sectionResult)
        {
            return new NumericFailureMechanismSectionResultRow(sectionResult);
        }
    }
}