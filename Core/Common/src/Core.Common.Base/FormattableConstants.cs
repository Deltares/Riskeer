// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;

namespace Core.Common.Base
{
    /// <summary>
    /// This class defines 'format' strings for <see cref="IFormattable"/> objects.
    /// </summary>
    public static class FormattableConstants
    {
        /// <summary>
        /// Numeric format string that ensures at least 1 decimal number is shown.
        /// </summary>
        /// <example>
        /// When using this format string in <see cref="IFormattable.ToString(string,IFormatProvider)"/>:
        /// <para><c>123.456</c> becomes <c>123.456</c></para>
        /// <para><c>0</c> becomes <c>0.0</c></para>
        /// <para><c>.700</c> becomes <c>0.7</c></para>
        /// </example>
        public static string ShowAtLeastOneDecimal
        {
            get
            {
                return "0.0###############";
            }
        }
    }
}