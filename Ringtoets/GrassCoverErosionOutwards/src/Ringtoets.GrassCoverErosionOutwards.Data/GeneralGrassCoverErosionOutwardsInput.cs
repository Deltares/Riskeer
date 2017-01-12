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
using Ringtoets.Common.Data.Properties;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Class that holds all the input parameters which are equal for every grass cover erosion outwards calculation.
    /// </summary>
    public class GeneralGrassCoverErosionOutwardsInput
    {
        private int n;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralGrassCoverErosionOutwardsInput"/> class.
        /// </summary>
        public GeneralGrassCoverErosionOutwardsInput()
        {
            n = 2;
            GeneralWaveConditionsInput = new GeneralWaveConditionsInput(1.0, 0.67, 0.0);
        }

        /// <summary>
        /// Gets the general input parameter used in wave conditions calculations.
        /// </summary>
        public GeneralWaveConditionsInput GeneralWaveConditionsInput { get; private set; }

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
    }
}