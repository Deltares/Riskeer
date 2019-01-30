// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Properties;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// The general input parameters that apply to each grass cover erosion outwards calculation.
    /// </summary>
    public class GeneralGrassCoverErosionOutwardsInput
    {
        private const int numberOfDecimalPlacesN = 2;

        private static readonly Range<RoundedDouble> validityRangeN = new Range<RoundedDouble>(new RoundedDouble(numberOfDecimalPlacesN, 1),
                                                                                               new RoundedDouble(numberOfDecimalPlacesN, 20));

        private RoundedDouble n;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralGrassCoverErosionOutwardsInput"/> class.
        /// </summary>
        public GeneralGrassCoverErosionOutwardsInput()
        {
            n = new RoundedDouble(numberOfDecimalPlacesN, 2.0);
            GeneralWaveConditionsInput = new GeneralWaveConditionsInput(1.0, 0.67, 0.0);
        }

        /// <summary>
        /// Gets the general input parameter used in wave conditions calculations.
        /// </summary>
        public GeneralWaveConditionsInput GeneralWaveConditionsInput { get; }

        #region Probability assessment

        /// <summary>
        /// Gets or sets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/>
        /// is not in the [1.0, 20.0] interval.</exception>
        public RoundedDouble N
        {
            get
            {
                return n;
            }
            set
            {
                RoundedDouble newValue = value.ToPrecision(n.NumberOfDecimalPlaces);
                if (!validityRangeN.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(Resources.N_Value_should_be_in_Range_0_,
                                                                                       validityRangeN));
                }

                n = newValue;
            }
        }

        #endregion
    }
}