﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// Class that holds all the static closing structures calculation input parameters.
    /// </summary>
    public class GeneralClosingStructuresInput
    {
        private RoundedDouble c;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralClosingStructuresInput"/> class.
        /// </summary>
        public GeneralClosingStructuresInput()
        {
            c = new RoundedDouble(2, 0.5);
            N2A = 1;

            GravitationalAcceleration = new RoundedDouble(2, 9.81);

            ModelFactorOvertoppingFlow = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.09,
                StandardDeviation = (RoundedDouble) 0.06
            };
            ModelFactorForStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.2
            };
            ModelFactorForSubCriticalFlow = new NormalDistribution(1)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.1
            };
            ModelFactorForIncomingFlowVolume = new RoundedDouble(2, 1);
        }

        #region Constants

        /// <summary>
        /// Gets the gravitational acceleration.
        /// </summary>
        public RoundedDouble GravitationalAcceleration { get; private set; }

        #endregion

        #region Length effect parameters

        /// <summary>
        /// Gets or sets the 'C' parameter used to factor in the 'length effect'.
        /// </summary>
        public RoundedDouble C
        {
            get
            {
                return c;
            }
            set
            {
                c = value.ToPrecision(c.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the 'N2A' parameter used to factor in the 'length effect'.
        /// </summary>
        public int N2A { get; set; }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        public RoundedDouble N
        {
            get
            {
                return new RoundedDouble(2, Math.Max(1, c*N2A));
            }
        }

        #endregion

        #region Model factors

        /// <summary>
        /// Gets the model factor overtopping flow.
        /// </summary>
        public LogNormalDistribution ModelFactorOvertoppingFlow { get; private set; }

        /// <summary>
        /// Gets the model factor for storage volume.
        /// </summary>
        public LogNormalDistribution ModelFactorForStorageVolume { get; private set; }

        /// <summary>
        /// Gets the model factor for sub critical flow.
        /// </summary>
        public NormalDistribution ModelFactorForSubCriticalFlow { get; private set; }

        /// <summary>
        /// Get the model factor for incoming flow volume.
        /// </summary>
        public RoundedDouble ModelFactorForIncomingFlowVolume { get; private set; }

        #endregion
    }
}