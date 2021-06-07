using Application.Riskeer.API.Interfaces;
using Riskeer.Integration.Data;

namespace Application.Riskeer.API.Implementation {
    public class AssessmentSectionApi : IAssessmentSectionApi
    {
        internal readonly AssessmentSection AssessmentSection;

        public AssessmentSectionApi(AssessmentSection assessmentSection)
        {
            this.AssessmentSection = assessmentSection;
        }

        public string Name
        {
            get => AssessmentSection.Name;
            set
            {
                AssessmentSection.Name = value;
                AssessmentSection.NotifyObservers();
            }
        }
    }
}