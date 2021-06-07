using System;
using System.Collections.Generic;
using Application.Riskeer.API.Interfaces;
using Core.Common.Base.Data;
using Riskeer.Integration.Data;

namespace Application.Riskeer.API.Implementation {
    public class ProjectApi : IProjectApi
    {
        internal readonly RiskeerProject RiskeerProject;
        internal readonly Dictionary<AssessmentSection,IAssessmentSectionApi> AssessmentSectionsDictionary = new Dictionary<AssessmentSection, IAssessmentSectionApi>();

        public ProjectApi(RiskeerProject openedProject)
        {
            RiskeerProject = openedProject;
            foreach (var assessmentSection in RiskeerProject.AssessmentSections)
            {
                AssessmentSectionsDictionary[assessmentSection] = new AssessmentSectionApi(assessmentSection);
            }
        }

    }
}