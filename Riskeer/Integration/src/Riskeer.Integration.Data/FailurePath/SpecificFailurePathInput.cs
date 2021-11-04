// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using RiskeerCommonResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Integration.Data.FailurePath
{
    /// <summary>
    /// The input parameters that apply to a specific failure path.
    /// </summary>
    public class SpecificFailurePathInput
    {
        private const int numberOfDecimalPlacesN = 2;
        private static RoundedDouble n;

        private static readonly Range<RoundedDouble> validityRangeN = new Range<RoundedDouble>(new RoundedDouble(numberOfDecimalPlacesN, 1),
                                                                                               new RoundedDouble(numberOfDecimalPlacesN, 20));

        /// <summary>
        /// Creates a new instance of <see cref="SpecificFailurePathInput"/>.
        /// </summary>
        public SpecificFailurePathInput()
        {
            n = new RoundedDouble(numberOfDecimalPlacesN, 1);
        }

        /// <summary>
        /// Gets or sets the 'N' parameter used to factor in the 'length effect'.
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the 'N' value is set outside the range [1.00, 20.00].</exception>
        /// </summary>
        public RoundedDouble N
        {
            get
            {
                return n;
            }
            set
            {
                RoundedDouble newValue = value.ToPrecision(2);
                if (!validityRangeN.InRange(newValue))
                {
                    string message = string.Format(RiskeerCommonResources.N_Value_should_be_in_Range_0_,
                                                   validityRangeN);
                    throw new ArgumentOutOfRangeException(nameof(value), message);
                }

                n = newValue;
            }
        }
    }
}