namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class contains the results of a semi-probabilistic assessment of the piping
    /// failure mechanism.
    /// </summary>
    public class PipingSemiProbabilisticOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub calculation.</param>
        /// <param name="pipingFactorOfSafety">The factor of safety for the piping failure mechanims.</param>
        public PipingSemiProbabilisticOutput(double upliftFactorOfSafety, double heaveFactorOfSafety, double sellmeijerFactorOfSafety, double pipingFactorOfSafety)
        {
            UpliftFactorOfSafety = upliftFactorOfSafety;
            HeaveFactorOfSafety = heaveFactorOfSafety;
            SellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            PipingFactorOfSafety = pipingFactorOfSafety;
        }

        /// <summary>
        /// Gets or sets the factor of safety of the piping failure mechanism.
        /// </summary>
        public double PipingFactorOfSafety { get; private set; }

        /// <summary>
        /// The factor of safety for the uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety { get; private set; }

        /// <summary>
        /// The factor of safety for the heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety { get; private set; }

        /// <summary>
        /// The factor of safety for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety { get; private set; }
    }
}