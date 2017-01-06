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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Class that holds all the static grass cover erosion inwards calculation input parameters.
    /// </summary>
    public class GeneralGrassCoverErosionInwardsInput
    {
        private int n;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralGrassCoverErosionInwardsInput"/> class.
        /// </summary>
        public GeneralGrassCoverErosionInwardsInput()
        {
            n = 2;
            CriticalOvertoppingModelFactor = 1.0;
            FbFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 4.75,
                StandardDeviation = (RoundedDouble) 0.5,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };
            FnFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 2.6,
                StandardDeviation = (RoundedDouble) 0.35,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };
            OvertoppingModelFactor = 1.0;
            FrunupModelFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.07,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };
            FshallowModelFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 0.92,
                StandardDeviation = (RoundedDouble) 0.24,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };
        }

        #region Probability assessment

        /// <summary>
        /// Gets or sets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is not in
        /// the interval [1, 20].</exception>
        public int N
        {
            get
            {
                return n;
            }
            set
            {
                if (value < 1 || value > 20)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.N_Value_should_be_in_interval_1_20);
                }
                n = value;
            }
        }

        #endregion

        #region Factors

        /// <summary>
        /// Gets the factor fb variable.
        /// </summary>
        public TruncatedNormalDistribution FbFactor { get; private set; }

        /// <summary>
        /// Gets the factor fn variable.
        /// </summary>
        public TruncatedNormalDistribution FnFactor { get; private set; }

        #endregion

        #region Model factors

        /// <summary>
        /// Gets the model factor critical overtopping.
        /// </summary>
        public double CriticalOvertoppingModelFactor { get; private set; }

        /// <summary>
        /// Gets the model factor overtopping.
        /// </summary>
        public double OvertoppingModelFactor { get; private set; }

        /// <summary>
        /// Gets the model factor frunup variable.
        /// </summary>
        public TruncatedNormalDistribution FrunupModelFactor { get; private set; }

        /// <summary>
        /// Gets the model factor fshallow variable.
        /// </summary>
        public TruncatedNormalDistribution FshallowModelFactor { get; private set; }

        #endregion
    }
}