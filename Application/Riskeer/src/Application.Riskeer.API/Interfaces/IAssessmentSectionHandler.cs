namespace Application.Riskeer.API.Interfaces {
    public interface IAssessmentSectionHandler
    {
        IAssessmentSectionApi FindAssessmentSection(IProjectApi project, string name);

        void ImportReferenceLine(IAssessmentSectionApi assessmentSectionApi, string shapeFileLocation);

        void ExportReferenceLine(IAssessmentSectionApi assessmentSectionApi, string destinationFilePath);

        void CoupleToHydraulicDatabase(IAssessmentSectionApi assessmentSectionApi, string databaseFileLocation);

        void ExportAssemblyResults(IAssessmentSectionApi assessmentSectionApi, string filePath);

        void ExportHydraulicBoundaryConditions(IAssessmentSectionApi assessmentSectionApi, string filePath);
    }
}