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
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub-mechanism.</param>
        /// <param name="upliftReliability">The reliability of uplift the sub-mechanism.</param>
        /// <param name="upliftProbability">The probability of failure due to the uplift sub-mechanism.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub-mechanism.</param>
        /// <param name="heaveReliability">The reliability of the heave sub-mechanism.</param>
        /// <param name="heaveProbability">The probability of failure due to the heave sub-mechanism.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub-mechanism.</param>
        /// <param name="sellmeijerReliability">The reliability of the Sellmeijer sub-mechanism.</param>
        /// <param name="sellmeijerProbability">The probability of failure due to the Sellmeijer sub-mechanism.</param>
        /// <param name="requiredProbability">The required (maximum allowed) probability of failure due to piping.</param>
        /// <param name="requiredReliability">The required (maximum allowed) reliabiality of the piping failure mechanism</param>
        /// <param name="pipingProbability">The calculated probability of failing due to piping.</param>
        /// <param name="pipingReliability">The calculated reliability of the piping failure mechanism.</param>
        /// <param name="pipingFactorOfSafety">The factor of safety for the piping failure mechanims.</param>
        public PipingSemiProbabilisticOutput(double upliftFactorOfSafety, double upliftReliability, double upliftProbability, double heaveFactorOfSafety, double heaveReliability, double heaveProbability, double sellmeijerFactorOfSafety, double sellmeijerReliability, double sellmeijerProbability, double requiredProbability, double requiredReliability, double pipingProbability, double pipingReliability, double pipingFactorOfSafety)
        {
            UpliftFactorOfSafety = upliftFactorOfSafety;
            UpliftReliability = upliftReliability;
            UpliftProbability = upliftProbability;
            HeaveFactorOfSafety = heaveFactorOfSafety;
            HeaveReliability = heaveReliability;
            HeaveProbability = heaveProbability;
            SellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            SellmeijerReliability = sellmeijerReliability;
            SellmeijerProbability = sellmeijerProbability;

            RequiredProbability = requiredProbability;
            RequiredReliability = requiredReliability;
            PipingProbability = pipingProbability;
            PipingReliability = pipingReliability;
            PipingFactorOfSafety = pipingFactorOfSafety;
        }

        /// <summary>
        /// Gets the required probability of the piping failure mechanism.
        /// </summary>
        public double RequiredProbability { get; private set; }

        /// <summary>
        /// Get the required reliability of the piping failure mechanism.
        /// </summary>
        public double RequiredReliability { get; private set; }

        /// <summary>
        /// Gets the factor of safety of the piping failure mechanism.
        /// </summary>
        public double PipingFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability of the piping failure mechanism.
        /// </summary>
        public double PipingReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the piping failure mechanism.
        /// </summary>
        public double PipingProbability { get; private set; }

        /// <summary>
        /// Gets the factor of safety for the uplift sub-mechanism.
        /// </summary>
        public double UpliftFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability for the uplift sub-mechanism.
        /// </summary>
        public double UpliftReliability{ get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the uplift failure sub-mechanism.
        /// </summary>
        public double UpliftProbability{ get; private set; }

        /// <summary>
        /// Gets the factor of safety for the heave sub-mechanism.
        /// </summary>
        public double HeaveFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability for the heave sub-mechanism.
        /// </summary>
        public double HeaveReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the heave failure sub-mechanism.
        /// </summary>
        public double HeaveProbability { get; private set; }

        /// <summary>
        /// Gets the factor of safety for the Sellmeijer sub-mechanism.
        /// </summary>
        public double SellmeijerFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability for the Sellmeijer sub-mechanism.
        /// </summary>
        public double SellmeijerReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the Sellmeijer failure sub-mechanism.
        /// </summary>
        public double SellmeijerProbability { get; private set; }
    }
}