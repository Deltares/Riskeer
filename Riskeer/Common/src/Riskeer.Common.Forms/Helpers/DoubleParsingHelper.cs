﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class to parse <see cref="double"/>.
    /// </summary>
    public static class DoubleParsingHelper
    {
        /// <summary>
        /// Parses a string value to a <see cref="double"/>.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <returns>A <see cref="double"/>.</returns>
        /// <exception cref="DoubleParsingException">Thrown when <paramref name="value"/> could not be successfully parsed as a probability.</exception>
        public static double Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return double.NaN;
            }

            try
            {
                return Convert.ToDouble(value);
            }
            catch (FormatException exception)
            {
                throw new DoubleParsingException(Resources.Value_Could_not_parse_string_to_RoundedDouble,
                                                 exception);
            }
            catch (OverflowException exception)
            {
                throw new DoubleParsingException(Resources.ParsingHelper_Value_too_largeor_too_small,
                                                 exception);
            }
        }
    }
}