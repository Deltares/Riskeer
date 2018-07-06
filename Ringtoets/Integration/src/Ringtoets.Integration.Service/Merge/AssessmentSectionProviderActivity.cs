using Core.Common.Base.Service;
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Service.Merge
{
    public class AssessmentSectionProviderActivity : Activity
    {
        private readonly AssessmentSectionsOwner owner;
        private readonly IAssessmentSectionProvider assessmentSectionProvider;

        public AssessmentSectionProviderActivity(AssessmentSectionsOwner owner, IAssessmentSectionProvider assessmentSectionProvider)
        {
            this.owner = owner;
            this.assessmentSectionProvider = assessmentSectionProvider;
        }

        protected override void OnRun()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCancel()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnFinish()
        {
            throw new System.NotImplementedException();
        }
    }
}