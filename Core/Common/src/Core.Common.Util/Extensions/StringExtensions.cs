// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Globalization;

namespace Core.Common.Util.Extensions
{
    /// <summary>
    /// This class defines extension methods for <see cref="string"/> objects.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a deep clone of a string.
        /// </summary>
        public static string DeepClone(this string original)
        {
            if (original == null)
            {
                return null;
            }

            return string.Format(CultureInfo.CurrentCulture,
                                 "{0}",
                                 original);
        }

        /// <summary>
        /// Sets the first letter of a string to upper case.
        /// </summary>
        /// <param name="str">The string to set the first letter to upper case for.</param>
        /// <returns>A string with the first letter set to upper case.</returns>
        public static string FirstToUpper(this string str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToUpper(str[0], CultureInfo.CurrentCulture) + str.Substring(1);
            }

            return str.ToUpper(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Sets the first letter of a string to lower case.
        /// </summary>
        /// <param name="str">The string to set the first letter to lower case for.</param>
        /// <returns>A string with the first letter set to lower case.</returns>
        public static string FirstToLower(this string str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToLower(str[0], CultureInfo.CurrentCulture) + str.Substring(1);
            }

            return str.ToLower(CultureInfo.CurrentCulture);
        }
    }
}