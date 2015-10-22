namespace Wti.Service
{
    /// <summary>
    /// Status result from a calculation or validation performed by the <see cref="PipingCalculationService"/>.
    /// </summary>
    public enum PipingCalculationResult
    {
        /// <summary>
        /// Value which is returned when no calculation or validation errors occurred.
        /// </summary>
        Successful,

        /// <summary>
        /// Value which is returned when the validation routine resulted in validation errors.
        /// </summary>
        ValidationErrors,

        /// <summary>
        /// Value which is returned when the calculation routine encountered problems.
        /// </summary>
        CalculationErrors
    }
}