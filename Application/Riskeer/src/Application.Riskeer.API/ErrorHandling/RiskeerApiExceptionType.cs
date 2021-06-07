namespace Application.Riskeer.API.ErrorHandling {
    public enum RiskeerApiExceptionType
    {
        InvalidRiskeerFile,
        InvalidFilePath,
        CouldNotConnectToFile,
        EmptyRiskeerProject,
        FileAlreadyExists,
        ReferenceLineNotFound,
        NoProjectSpecified,
        AssessmentSectionNotFound
    }
}