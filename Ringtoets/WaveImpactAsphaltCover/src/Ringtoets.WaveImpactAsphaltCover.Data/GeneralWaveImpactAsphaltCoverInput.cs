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
using Ringtoets.WaveImpactAsphaltCover.Data.Properties;

namespace Ringtoets.WaveImpactAsphaltCover.Data
{
    /// <summary>
    /// The general input parameters that apply to each wave impact asphalt cover calculation.
    /// </summary>
    public class GeneralWaveImpactAsphaltCoverInput
    {
        private RoundedDouble deltaL;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralWaveImpactAsphaltCoverInput"/> class.
        /// </summary>
        public GeneralWaveImpactAsphaltCoverInput()
        {
            deltaL = new RoundedDouble(2, 1000.0);
            SectionLength = double.NaN;
        }

        /// <summary>
        /// Gets or sets the 'ΔL' parameter used to determine the length effect parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/>
        /// is <see cref="double.NaN"/> or is not larger than 0.</exception>
        public RoundedDouble DeltaL
        {
            get
            {
                return deltaL;
            }
            set
            {
                RoundedDouble newValue = value.ToPrecision(deltaL.NumberOfDecimalPlaces);
                if (newValue <= 0.0 || double.IsNaN(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.DeltaL_Value_should_be_larger_than_zero);
                }

                deltaL = newValue;
            }
        }

        /// <summary>
        /// Gets or sets the length of the assessment section.
        /// </summary>
        public double SectionLength { get; set; }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        public double N
        {
            get
            {
                return Math.Max(1, SectionLength / deltaL);
            }
        }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        /// <param name="sectionLength">The length of the assessment section.</param>
        /// <returns>The 'N' parameter.</returns>
        public double GetN(double sectionLength)
        {
            return Math.Max(1, sectionLength / deltaL);
        }
    }
}