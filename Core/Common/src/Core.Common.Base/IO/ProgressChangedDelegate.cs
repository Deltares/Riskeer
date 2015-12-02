namespace Core.Common.Base.IO
{
    /// <summary>
    /// Action to perform when progress has changed.
    /// </summary>
    /// <param name="currentStepDescription">The description of the current step.</param>
    /// <param name="currentStep">The number of the current progress step.</param>
    /// <param name="totalSteps">The total number of progress steps.</param>
    public delegate void ProgressChangedDelegate(string currentStepDescription, int currentStep, int totalSteps);
}