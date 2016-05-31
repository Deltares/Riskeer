using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class GrassCoverErosionOutwardsResultView : SimpleFailureMechanismResultView
    {
        protected override object CreateFailureMechanismSectionResultRow(SimpleFailureMechanismSectionResult sectionResult)
        {
            return new SimpleFailureMechanismSectionResultRow(sectionResult);
        }
    }
}