﻿using System;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This class is a representation of a variable derived from a probabilistic distribution,
    /// based on a percentile.
    /// </summary>
    public class DesignVariable
    {
        private double percentile;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignVariable"/> class with 
        /// <see cref="Percentile"/> equal to 0.5.
        /// </summary>
        public DesignVariable()
        {
            percentile = 0.5;
        }

        /// <summary>
        /// Gets or sets the probabilistic distribution of the parameter being modeled.
        /// </summary>
        public IDistribution Distribution { get; set; }

        /// <summary>
        /// Gets or sets the percentile used to derive a deterministic value based on <see cref="Distribution"/>.
        /// </summary>
        public double Percentile
        {
            get
            {
                return percentile;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.DesignVariable_Percentile_must_be_in_range);
                }
                percentile = value;
            }
        }

        /// <summary>
        /// Gets the design value based on the <see cref="Distribution"/> and <see cref="Percentile"/>.
        /// </summary>
        /// <returns>A design value.</returns>
        /// <exception cref="System.InvalidOperationException"><see cref="Distribution"/> is null.</exception>
        public double GetDesignValue()
        {
            if (Distribution == null)
            {
                throw new InvalidOperationException(Resources.DesignVariable_GetDesignValue_Distribution_must_be_set);
            }
            return Distribution.InverseCDF(Percentile);
        }
    }
}