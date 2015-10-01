namespace Wti.Calculation.Piping
{
    /// <summary>
    /// This class contains all the results of a complete piping calculation.
    /// </summary>
    public class PipingCalculationResult {
        private readonly double upliftZValue;
        private readonly double upliftFactorOfSafety;
        private readonly double heaveZValue;
        private readonly double heaveFactorOfSafety;
        private readonly double sellmeijerZValue;
        private readonly double sellmeijerFactorOfSafety;

        #region properties

        /// <summary>
        /// Gets the z-value of the Uplift sub calculation.
        /// </summary>
        public double UpliftZValue { get
            {
                return upliftZValue;
            } 
        }

        /// <summary>
        /// Gets the factory of safety of the Uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety
        {
            get
            {
                return upliftFactorOfSafety;
            }
        }

        /// <summary>
        /// Gets the z-value of the Heave sub calculation.
        /// </summary>
        public double HeaveZValue
        {
            get
            {
                return heaveZValue;
            }
        }

        /// <summary>
        /// Gets the factory of safety of the Heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety
        {
            get
            {
                return heaveFactorOfSafety;
            }
        }

        /// <summary>
        /// Gets the z-value of the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerZValue
        {
            get
            {
                return sellmeijerZValue;
            }
        }

        /// <summary>
        /// Gets the factory of safety of the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety
        {
            get
            {
                return sellmeijerFactorOfSafety;
            }
        }

        #endregion

        /// <summary>
        /// Constructs a new <see cref="PipingCalculationResult"/>. The result will hold all the values which were given.
        /// </summary>
        /// <param name="upliftZValue">The z-value of the Uplift sub calculation.</param>
        /// <param name="upliftFactorOfSafety">The factory of safety of the Uplift sub calculation.</param>
        /// <param name="heaveZValue">The z-value of the Heave sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factory of safety of the Heave sub calculation.</param>
        /// <param name="sellmeijerZValue">The z-value of the Sellmeijer sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factory of safety of the Sellmeijer sub calculation.</param>
        public PipingCalculationResult(double upliftZValue, double upliftFactorOfSafety, double heaveZValue, double heaveFactorOfSafety, double sellmeijerZValue, double sellmeijerFactorOfSafety)
        {
            this.upliftZValue = upliftZValue;
            this.upliftFactorOfSafety = upliftFactorOfSafety;
            this.heaveZValue = heaveZValue;
            this.heaveFactorOfSafety = heaveFactorOfSafety;
            this.sellmeijerZValue = sellmeijerZValue;
            this.sellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
        }
    }
}