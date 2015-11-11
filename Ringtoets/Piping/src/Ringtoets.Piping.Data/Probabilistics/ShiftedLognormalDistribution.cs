namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class represents a specialized case of <see cref="LognormalDistribution"/> that has
    /// been shifted along the X-axis.
    /// </summary>
    public class ShiftedLognormalDistribution : LognormalDistribution
    {
        /// <summary>
        /// Gets or sets the shift applied to the log-normal distribution.
        /// </summary>
        public double Shift { get; set; }
    }
}