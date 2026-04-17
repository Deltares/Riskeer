// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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

using Core.Common.Base.Data;
using System;
using Riskeer.WaveImpactAsphaltCover.Data.Properties;

namespace Riskeer.WaveImpactAsphaltCover.Data
{
    /// <summary>
    /// The general input parameters that apply to each wave impact asphalt cover wave conditions calculation.
    /// </summary>
    public class GeneralWaveImpactAsphaltCoverWaveConditionsInput
    {
        private const int waveImpactAphaltNumberOfDecimalPlaces = 2;
        private RoundedDouble c;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralWaveImpactAsphaltCoverWaveConditionsInput"/>.
        /// </summary>
        public GeneralWaveImpactAsphaltCoverWaveConditionsInput()
        {
            A = new RoundedDouble(waveImpactAphaltNumberOfDecimalPlaces, 1.0);
            B = new RoundedDouble(waveImpactAphaltNumberOfDecimalPlaces);
            c = new RoundedDouble(waveImpactAphaltNumberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets the 'a' parameter used in wave impact asphalt cover wave conditions calculations.
        /// </summary>
        public RoundedDouble A { get; }

        /// <summary>
        /// Gets the 'b' parameter used in wave impact asphalt cover wave conditions calculations.
        /// </summary>
        public RoundedDouble B { get; }

        private static readonly Range<RoundedDouble> waveImpactAphaltCValidityRange =
            new Range<RoundedDouble>(new RoundedDouble(waveImpactAphaltNumberOfDecimalPlaces),
                                     new RoundedDouble(waveImpactAphaltNumberOfDecimalPlaces, 2.0));

        /// <summary>
        /// Gets or sets the 'c' parameter used in wave impact asphalt cover wave conditions calculations.
        /// </summary>
        public RoundedDouble C
        {
            get
            {
                return c;
            }
            set
            {
                RoundedDouble newValue = value.ToPrecision(waveImpactAphaltNumberOfDecimalPlaces);

                if (!waveImpactAphaltCValidityRange.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(Resources.WaveImpactAsphaltParamC_must_be_in_Range_0_,
                                                                                       waveImpactAphaltCValidityRange));
                }

                c = newValue;
            }
        }
    }
}