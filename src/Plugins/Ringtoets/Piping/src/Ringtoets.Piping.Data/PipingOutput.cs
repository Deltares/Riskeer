namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Simple class containing the results of a Piping calculation.
    /// </summary>
    public class PipingOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingOutput"/>. 
        /// </summary>
        /// <param name="upliftZValue">The calculated z-value for the uplift sub calculation.</param>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub calculation.</param>
        /// <param name="heaveZValue">The calculated z-value for the heave sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub calculation.</param>
        /// <param name="sellmeijerZValue">The calculated z-value for the Sellmeijer sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub calculation.</param>
        public PipingOutput(double upliftZValue, double upliftFactorOfSafety, double heaveZValue, double heaveFactorOfSafety, double sellmeijerZValue, double sellmeijerFactorOfSafety)
        {
            HeaveFactorOfSafety = heaveFactorOfSafety;
            HeaveZValue = heaveZValue;
            UpliftFactorOfSafety = upliftFactorOfSafety;
            UpliftZValue = upliftZValue;
            SellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            SellmeijerZValue = sellmeijerZValue;
        }

        /// <summary>
        /// The calculated z-value for the uplift sub calculation.
        /// </summary>
        public double UpliftZValue { get; private set; }

        /// <summary>
        /// The factor of safety for the uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety { get; private set; }

        /// <summary>
        /// The calculated z-value for the heave sub calculation.
        /// </summary>
        public double HeaveZValue { get; private set; }

        /// <summary>
        /// The factor of safety for the heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety { get; private set; }

        /// <summary>
        /// The calculated z-value for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerZValue { get; private set; }

        /// <summary>
        /// The factor of safety for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety { get; private set; }
    }
}