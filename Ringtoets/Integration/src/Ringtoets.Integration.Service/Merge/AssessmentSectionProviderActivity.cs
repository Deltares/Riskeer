using Core.Common.Base.Service;

namespace Ringtoets.Integration.Service.Merge
{
    public class AssessmentSectionProviderActivity : Activity
    {
        private readonly IAssessmentSectionProvider assessmentSectionProvider;

        public AssessmentSectionProviderActivity(IAssessmentSectionProvider assessmentSectionProvider)
        {
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