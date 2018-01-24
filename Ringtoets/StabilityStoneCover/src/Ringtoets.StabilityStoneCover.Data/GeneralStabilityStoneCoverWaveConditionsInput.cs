﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Properties;
using Ringtoets.Revetment.Data;

namespace Ringtoets.StabilityStoneCover.Data
{
    /// <summary>
    /// Class that holds all the static stability stone cover wave conditions input parameters.
    /// </summary>
    public class GeneralStabilityStoneCoverWaveConditionsInput
    {
        private static readonly Range<double> validityRangeN = new Range<double>(1.0, 20.0);
        private RoundedDouble n;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralStabilityStoneCoverWaveConditionsInput"/>.
        /// </summary>
        public GeneralStabilityStoneCoverWaveConditionsInput()
        {
            GeneralBlocksWaveConditionsInput = new GeneralWaveConditionsInput(1.0, 1.0, 1.0);
            GeneralColumnsWaveConditionsInput = new GeneralWaveConditionsInput(1.0, 0.4, 0.8);
            n = new RoundedDouble(2, 4.0);
        }

        /// <summary>
        /// Gets the general input parameter used in wave conditions calculations for blocks.
        /// </summary>
        public GeneralWaveConditionsInput GeneralBlocksWaveConditionsInput { get; }

        /// <summary>
        /// Gets the general input parameter used in wave conditions calculations for columns.
        /// </summary>
        public GeneralWaveConditionsInput GeneralColumnsWaveConditionsInput { get; }

        /// <summary>
        /// Gets or sets the general input parameter N used in wave conditions calculations.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of <see cref="N"/>
        /// is not in the range [1, 20].</exception>
        public RoundedDouble N
        {
            get
            {
                return n;
            }
            set
            {
                if (!validityRangeN.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(Resources.N_Value_should_be_in_Range_0_,
                                                                                       validityRangeN));
                }

                n = value.ToPrecision(n.NumberOfDecimalPlaces);
            }
        }
    }
}