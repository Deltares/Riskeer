// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class to parse <see cref="RoundedDouble"/>.
    /// </summary>
    public static class RoundedDoubleParsingHelper
    {
        /// <summary>
        /// Parses a string value to a <see cref="RoundedDouble"/>.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <param name="nrOfDecimals">The number of decimals.</param>
        /// <returns>A <see cref="RoundedDouble"/>.</returns>
        /// <exception cref="RoundedDoubleParsingException">Thrown when <paramref name="value"/> could not be successfully parsed as a probability.</exception>
        public static RoundedDouble Parse(string value, int nrOfDecimals)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return RoundedDouble.NaN;
            }

            try
            {
                return new RoundedDouble(nrOfDecimals, Convert.ToDouble(value));
            }
            catch (FormatException exception)
            {
                throw new RoundedDoubleParsingException(Resources.Value_Could_not_parse_string_to_RoundedDouble,
                                                 exception);
            }
            catch (OverflowException exception)
            {
                throw new RoundedDoubleParsingException(Resources.ParsingHelper_Value_too_large,
                                                 exception);
            }
        }
    }
}