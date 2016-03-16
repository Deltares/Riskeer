using System;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Piping.Data
{
    public class SemiProbabilisticPipingInput
    {
        private double contribution;

        public SemiProbabilisticPipingInput()
        {
            A = 1.0;
            B = 350.0;
            SectionLength = double.NaN;
            Norm = 0;
            Contribution = double.NaN;
        }

        /// <summary>
        /// Gets 'a' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double A { get; private set; }

        /// <summary>
        /// Gets 'b' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// Gets or sets the length of the assessment section.
        /// </summary>
        public double SectionLength { get; set; }

        /// <summary>
        /// Gets or sets the contribution of piping as a percentage (0-100) to the total of the failure 
        /// probability of the assessment section.
        /// </summary>
        public double Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.Contribution_Value_should_be_in_interval_0_100);
                }
                contribution = value;
            }
        }

        /// <summary>
        /// Gets or sets the return period to assess for.
        /// </summary>
        public int Norm { get; set; }

    }
}