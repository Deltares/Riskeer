using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for failure mechanims settings, uniquely identified by the <see cref="FailureMechanismType"/>.
    /// </summary>
    public class FailureMechanismSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public HydraRingFailureMechanismType FailureMechanismType { get; set; }

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