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
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Class that holds all static height structures calculation specific input parameters.
    /// </summary>
    public class GeneralHeightStructuresInput
    {
        private int n;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralHeightStructuresInput"/> class.
        /// </summary>
        public GeneralHeightStructuresInput()
        {
            n = 2;

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

            ModelFactorInflowVolume = new RoundedDouble(2, 1);
        }

        /// <summary>
        /// Gets or sets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is not in interval 
        /// [1, 20].</exception>
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
                    throw new ArgumentOutOfRangeException("value", Resources.N_Value_should_be_in_interval_1_20);
                }
                n = value;
            }
        }

        /// <summary>
        /// Gets the gravitational acceleration.
        /// [m/s^2]
        /// </summary>
        public RoundedDouble GravitationalAcceleration { get; private set; }

        /// <summary>
        /// Gets the model factor overtopping flow.
        /// </summary>
        public LogNormalDistribution ModelFactorOvertoppingFlow { get; private set; }

        /// <summary>
        /// Gets the model factor for storage volume.
        /// </summary>
        public LogNormalDistribution ModelFactorStorageVolume { get; private set; }

        /// <summary>
        /// Gets the model factor for incoming flow volume.
        /// </summary>
        public RoundedDouble ModelFactorInflowVolume { get; private set; }
    }
}