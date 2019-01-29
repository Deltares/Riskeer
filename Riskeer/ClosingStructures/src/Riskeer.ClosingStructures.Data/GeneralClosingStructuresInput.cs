// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.ClosingStructures.Data.Properties;
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.ClosingStructures.Data
{
    /// <summary>
    /// The general input parameters that apply to each closing structures calculation.
    /// </summary>
    public class GeneralClosingStructuresInput
    {
        private static readonly Range<int> validityRangeN2A = new Range<int>(0, 40);
        private int n2A;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralClosingStructuresInput"/> class.
        /// </summary>
        public GeneralClosingStructuresInput()
        {
            C = new RoundedDouble(2, 0.5);
            N2A = 1;

            GravitationalAcceleration = new RoundedDouble(2, 9.81);

            ModelFactorOvertoppingFlow = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.09,
                StandardDeviation = (RoundedDouble) 0.06
            };

            ModelFactorStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };

            ModelFactorLongThreshold = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 0.9,
                StandardDeviation = (RoundedDouble) 0.05
            };

            ModelFactorInflowVolume = new RoundedDouble(2, 1);
        }

        #region Constants

        /// <summary>
        /// Gets the gravitational acceleration.
        /// [m/s^2]
        /// </summary>
        public RoundedDouble GravitationalAcceleration { get; }

        #endregion

        #region Length effect parameters

        /// <summary>
        /// Gets or sets the 'C' parameter used to factor in the 'length effect'.
        /// </summary>
        public RoundedDouble C { get; }

        /// <summary>
        /// Gets or sets the 'N2A' parameter used to factor in the 'length effect'.
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the 'N2A' value is set outside the range [0, 40].</exception>
        /// </summary>
        public int N2A
        {
            get
            {
                return n2A;
            }
            set
            {
                if (!validityRangeN2A.InRange(value))
                {
                    string message = string.Format(Resources.N2A_Value_should_be_in_Range_0_,
                                                   validityRangeN2A);
                    throw new ArgumentOutOfRangeException(nameof(value), message);
                }

                n2A = value;
            }
        }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        public double N
        {
            get
            {
                return Math.Max(1, C * N2A);
            }
        }

        #endregion

        #region Model factors

        /// <summary>
        /// Gets the model factor overtopping flow.
        /// </summary>
        public LogNormalDistribution ModelFactorOvertoppingFlow { get; }

        /// <summary>
        /// Gets the model factor for storage volume.
        /// </summary>
        public LogNormalDistribution ModelFactorStorageVolume { get; }

        /// <summary>
        /// Gets the model factor for long threshold.
        /// </summary>
        public NormalDistribution ModelFactorLongThreshold { get; }

        /// <summary>
        /// Gets the model factor for incoming flow volume.
        /// </summary>
        public RoundedDouble ModelFactorInflowVolume { get; }

        #endregion
    }
}