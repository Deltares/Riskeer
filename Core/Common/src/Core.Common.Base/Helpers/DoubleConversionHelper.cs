// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Globalization;
using System.Xml;

namespace Core.Common.Base.Helpers
{
    /// <summary>
    /// Helper class for parsing and converting values to a <see cref="double"/>.
    /// </summary>
    public static class DoubleConversionHelper
    {
        /// <summary>
        /// Parses a <see cref="string"/> to a <see cref="double"/> using the specified culture-specific format. 
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="culture">The culture-specific formatting information to use.</param>
        /// <returns>The parsed <see cref="double"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="value"/> does not represent a number in a valid format.</exception>
        /// <exception cref="OverflowException">Thrown when the parsed value represents a number less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static double Parse(string value, CultureInfo culture = null)
        {
            double parsedDouble = double.Parse(value, culture);

            return double.IsInfinity(parsedDouble) && !IsInfinityLiteral(value, culture)
                       ? throw new OverflowException()
                       : parsedDouble;
        }

        /// <summary>
        /// Converts an object to a <see cref="double"/> using the specified culture-specific format. 
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="culture">The culture-specific formatting information to use.</param>
        /// <returns>The converted <see cref="double"/>.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not in an appropriate format for a double type.</exception>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="value"/> does not implement the <see cref="IConvertible"/> interface.</exception>
        /// <exception cref="OverflowException">Thrown when the converted value represents a number less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static double ConvertToDouble(object? value, CultureInfo culture = null)
        {
            var convertedDouble = Convert.ToDouble(value, culture);

            return double.IsInfinity(convertedDouble) && !IsInfinityLiteral(value?.ToString(), culture)
                       ? throw new OverflowException()
                       : convertedDouble;
        }

        /// <summary>
        /// Converts an Xml <see cref="string"/> to a <see cref="double"/>. 
        /// </summary>
        /// <param name="value">The Xml <see cref="string"/> to convert.</param>
        /// <returns>The converted <see cref="double"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="value"/> does not represent a number in a valid format.</exception>
        /// <exception cref="OverflowException">Thrown when the converted value represents a number less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static double XmlConvertToDouble(string value)
        {
            var convertedDouble = XmlConvert.ToDouble(value);

            return double.IsInfinity(convertedDouble) && !IsXmlInfinityLiteral(value)
                       ? throw new OverflowException()
                       : convertedDouble;
        }

        private static bool IsInfinityLiteral(string value, CultureInfo culture)
        {
            string trimmedValue = value.Trim();

            return string.Equals(trimmedValue, Convert.ToString(double.PositiveInfinity, culture))
                   || string.Equals(trimmedValue, Convert.ToString(double.NegativeInfinity, culture));
        }

        private static bool IsXmlInfinityLiteral(string value)
        {
            string trimmedValue = value.Trim();

            return string.Equals(trimmedValue, XmlConvert.ToString(double.PositiveInfinity))
                   || string.Equals(trimmedValue, XmlConvert.ToString(double.NegativeInfinity));
        }
    }
}