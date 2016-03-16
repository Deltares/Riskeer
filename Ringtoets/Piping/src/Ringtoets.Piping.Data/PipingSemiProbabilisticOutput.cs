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
        public PipingSemiProbabilisticOutput()
        {
            PipingFactorOfSafety = double.NaN;
        }

        /// <summary>
        /// Gets or sets the factor of safety of the piping failure mechanism.
        /// </summary>
        public double PipingFactorOfSafety { get; set; }
    }
}