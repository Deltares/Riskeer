using System;

using Core.Common.Base.Data;
using Core.Common.Base.Storage;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class contains the results of a semi-probabilistic assessment of the piping
    /// failure mechanism.
    /// </summary>
    public class PipingSemiProbabilisticOutput : IStorable
    {
        private double requiredProbability;
        private double pipingProbability;
        private double upliftProbability;
        private double heaveProbability;
        private double sellmeijerProbability;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub-mechanism.</param>
        /// <param name="upliftProbability">The probability of failure due to the uplift sub-mechanism.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub-mechanism.</param>
        /// <param name="heaveReliability">The reliability of the heave sub-mechanism.</param>
        /// <param name="heaveProbability">The probability of failure due to the heave sub-mechanism.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub-mechanism.</param>
        /// <param name="sellmeijerReliability">The reliability of the Sellmeijer sub-mechanism.</param>
        /// <param name="sellmeijerProbability">The probability of failure due to the Sellmeijer sub-mechanism.</param>
        /// <param name="requiredProbability">The required (maximum allowed) probability of failure due to piping.</param>
        /// <param name="requiredReliability">The required (maximum allowed) reliability of the piping failure mechanism</param>
        /// <param name="pipingProbability">The calculated probability of failing due to piping.</param>
        /// <param name="pipingReliability">The calculated reliability of the piping failure mechanism.</param>
        /// <param name="pipingFactorOfSafety">The factor of safety for the piping failure mechanism.</param>
        /// <exception cref="ArgumentOutOfRangeException">When setting a probability that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public PipingSemiProbabilisticOutput(double upliftFactorOfSafety, double upliftProbability, double heaveFactorOfSafety, double heaveReliability, double heaveProbability, double sellmeijerFactorOfSafety, double sellmeijerReliability, double sellmeijerProbability, double requiredProbability, double requiredReliability, double pipingProbability, double pipingReliability, double pipingFactorOfSafety)
        {
            UpliftFactorOfSafety = new RoundedDouble(3, upliftFactorOfSafety);
            UpliftProbability = upliftProbability;
            HeaveFactorOfSafety = new RoundedDouble(3, heaveFactorOfSafety);
            HeaveReliability = new RoundedDouble(3, heaveReliability);
            HeaveProbability = heaveProbability;
            SellmeijerFactorOfSafety = new RoundedDouble(3, sellmeijerFactorOfSafety);
            SellmeijerReliability = new RoundedDouble(3, sellmeijerReliability);
            SellmeijerProbability = sellmeijerProbability;

            RequiredProbability = requiredProbability;
            RequiredReliability = new RoundedDouble(3, requiredReliability);
            PipingProbability = pipingProbability;
            PipingReliability = new RoundedDouble(3, pipingReliability);
            PipingFactorOfSafety = new RoundedDouble(3, pipingFactorOfSafety);
        }

        /// <summary>
        /// Gets the required probability of the piping failure mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double RequiredProbability
        {
            get
            {
                return requiredProbability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    requiredProbability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        /// <summary>
        /// Get the required reliability of the piping failure mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble RequiredReliability { get; private set; }

        /// <summary>
        /// Gets the factor of safety of the piping failure mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble PipingFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability of the piping failure mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble PipingReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the piping failure mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double PipingProbability
        {
            get
            {
                return pipingProbability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    pipingProbability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        /// <summary>
        /// Gets the factor of safety for the uplift sub-mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble UpliftFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability for the uplift sub-mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble UpliftReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the uplift failure sub-mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double UpliftProbability
        {
            get
            {
                return upliftProbability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    upliftProbability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        /// <summary>
        /// Gets the factor of safety for the heave sub-mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble HeaveFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability for the heave sub-mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble HeaveReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the heave failure sub-mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double HeaveProbability
        {
            get
            {
                return heaveProbability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    heaveProbability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        /// <summary>
        /// Gets the factor of safety for the Sellmeijer sub-mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble SellmeijerFactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability for the Sellmeijer sub-mechanism,
        /// which is a value greater than 0.
        /// </summary>
        public RoundedDouble SellmeijerReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing due to the Sellmeijer failure sub-mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double SellmeijerProbability
        {
            get
            {
                return sellmeijerProbability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    sellmeijerProbability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        public long StorageId { get; set; }
    }
}