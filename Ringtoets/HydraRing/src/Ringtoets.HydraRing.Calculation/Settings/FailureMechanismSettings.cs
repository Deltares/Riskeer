namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for failure mechanims settings.
    /// </summary>
    public class FailureMechanismSettings
    {
        private readonly double valueMin;
        private readonly double valueMax;
        private readonly double faultTreeModelId;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismSettings"/> class.
        /// </summary>
        /// <param name="valueMin">The minimum value to use while iterating to a target probability.</param>
        /// <param name="valueMax">The maximum value to use while iterating to a target probability.</param>
        /// <param name="faultTreeModelId">The fault tree model id.</param>
        public FailureMechanismSettings(double valueMin, double valueMax, double faultTreeModelId)
        {
            this.valueMin = valueMin;
            this.valueMax = valueMax;
            this.faultTreeModelId = faultTreeModelId;
        }

        /// <summary>
        /// Gets the minimum value to use while iterating to a target probability.
        /// </summary>
        /// <remarks>This property is only applicable in case of type-2 computations.</remarks>
        public double ValueMin
        {
            get
            {
                return valueMin;
            }
        }

        /// <summary>
        /// Gets the maximum value to use while iterating to a target probability.
        /// </summary>
        /// <remarks>This property is only applicable in case of type-2 computations.</remarks>
        public double ValueMax
        {
            get
            {
                return valueMax;
            }
        }

        /// <summary>
        /// Gets the fault tree model id.
        /// </summary>
        public double FaultTreeModelId
        {
            get
            {
                return faultTreeModelId;
            }
        }
    }
}