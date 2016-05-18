using Core.Common.Base.Data;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    public class GrassCoverErosionInwardsFailureMechanismSectionResultRow
    {
        public GrassCoverErosionInwardsFailureMechanismSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            SectionResult = sectionResult;
        }

        public string Name
        {
            get
            {
                return SectionResult.Section.Name;
            }
        }

        public bool AssessmentLayerOne
        {
            get
            {
                return SectionResult.AssessmentLayerOne;
            }
            set
            {
                SectionResult.AssessmentLayerOne = value;
                SectionResult.NotifyObservers();
            }
        }

        public RoundedDouble AssessmentLayerTwoA
        {
            get
            {
                return SectionResult.AssessmentLayerTwoA;
            }
        }

        public RoundedDouble AssessmentLayerTwoB
        {
            get
            {
                return SectionResult.AssessmentLayerTwoB;
            }
            set
            {
                SectionResult.AssessmentLayerTwoB = value;
            }
        }

        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                SectionResult.AssessmentLayerThree = value;
            }
        }

        public GrassCoverErosionInwardsFailureMechanismSectionResult SectionResult { get; private set; }
    }
}