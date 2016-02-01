// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class representing a log-normal distribution.
    /// </summary>
    public class LognormalDistribution : IDistribution
    {
        private double standardDeviation;
        private double mean;

        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistribution"/> class,
        /// initialized as the standard log-normal distribution (mu=0, sigma=1).
        /// </summary>
        public LognormalDistribution()
        {
            // Simplified calculation mean and standard deviation given mu=0 and sigma=1.
            mean = Math.Exp(-0.5);
            StandardDeviation = Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1));
        }

        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected value is less then or equal to 0.</exception>
        public double Mean
        {
            get
            {
                return mean;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.LognormalDistribution_Mean_must_be_greater_equal_to_zero);
                }
                mean = value;
            }
        }

        public double StandardDeviation
        {
            get
            {
                return standardDeviation;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.StandardDeviation_Should_be_greater_than_zero);
                }
                standardDeviation = value;
            }
        }
    }
}