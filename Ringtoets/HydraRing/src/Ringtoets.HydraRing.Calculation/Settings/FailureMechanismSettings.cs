namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for failure mechanims settings.
    /// </summary>
    public class FailureMechanismSettings
    {
        /// <summary>
        /// Gets or sets the minimum value to use while iterating to a target probability.
        /// </summary>
        public double ValueMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum value to use while iterating to a target probability.
        /// </summary>
        public double ValueMax { get; set; }
    }
}