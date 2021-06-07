using Application.Riskeer.API.Implementation;

namespace Application.Riskeer.API.Interfaces
{
    public interface IProjectHandler
    {
        ProjectApi OpenProject(string fileLocation);

        void SaveProject(ProjectApi project, string filePath, bool overwrite = false);

        // Lateron
        IAssessmentSectionApi AddAssessmentSection(ProjectApi project, string sectionId, bool useLowerBoundaryNorm);
    }
}
