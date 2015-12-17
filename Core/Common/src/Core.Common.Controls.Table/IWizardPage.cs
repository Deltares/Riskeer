namespace Core.Common.Controls.Table
{
    /// <summary>
    /// </summary>
    public interface IWizardPage
    {
        /// <summary>
        /// Determines whether a finish button should is visible
        /// </summary>
        bool CanFinish();

        /// <summary>
        /// Determines whether a next button should is visible
        /// </summary>
        bool CanDoNext();

        /// <summary>
        /// Determines whether a previous button should is visible
        /// </summary>
        bool CanDoPrevious();
    }
}