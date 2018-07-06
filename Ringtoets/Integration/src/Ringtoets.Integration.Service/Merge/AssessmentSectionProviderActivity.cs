using Core.Common.Base.Service;
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Service.Merge
{
    public class AssessmentSectionProviderActivity : Activity
    {
        private readonly AssessmentSectionsOwner owner;
        private readonly IAssessmentSectionProvider assessmentSectionProvider;
        private readonly string filePath;

        private bool canceled;

        public AssessmentSectionProviderActivity(AssessmentSectionsOwner owner,
                                                 IAssessmentSectionProvider assessmentSectionProvider,
                                                 string filePath)
        {
            this.owner = owner;
            this.assessmentSectionProvider = assessmentSectionProvider;
            this.filePath = filePath;
        }

        protected override void OnRun()
        {
            owner.AssessmentSections = assessmentSectionProvider.GetAssessmentSections(filePath);
        }

        protected override void OnCancel()
        {
            canceled = true;
        }

        protected override void OnFinish()
        {
            if (canceled)
            {
                owner.AssessmentSections = null;
            }
        }
    }
}