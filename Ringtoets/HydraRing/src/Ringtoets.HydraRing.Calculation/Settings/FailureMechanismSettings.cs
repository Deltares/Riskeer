namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for failure mechanims settings.
    /// </summary>
    public class FailureMechanismSettings
    {
        private readonly double valueMin;
        private readonly double valueMax;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismSettings"/> class.
        /// </summary>
        /// <param name="valueMin">The minimum value to use while iterating to a target probability.</param>
        /// <param name="valueMax">The maximum value to use while iterating to a target probability.</param>
        public FailureMechanismSettings(double valueMin, double valueMax)
        {
            this.valueMin = valueMin;
            this.valueMax = valueMax;
        }

        /// <summary>
        /// Gets or sets the minimum value to use while iterating to a target probability.
        /// </summary>
        public double ValueMin
        {
            get
            {
                return valueMin;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value to use while iterating to a target probability.
        /// </summary>
        public double ValueMax
        {
            get
            {
                return valueMax;
            }
        }
    }
}