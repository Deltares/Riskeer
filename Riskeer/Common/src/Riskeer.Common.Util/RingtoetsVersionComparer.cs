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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// This class can be used to compare Ringtoets database files.
    /// </summary>
    public class RingtoetsVersionComparer : IComparer, IComparer<string>
    {
        private const string versionSeparator = ".";

        public int Compare(object x, object y)
        {
            return Compare(GetFormattableString(x), GetFormattableString(y));
        }

        public int Compare(string x, string y)
        {
            if (x == null || y == null)
            {
                return string.Compare(x, y, StringComparison.InvariantCulture);
            }

            char[] separatorArray = versionSeparator.ToCharArray();
            string[] firstVersionArray = x.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
            string[] secondVersionArray = y.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);

            if (firstVersionArray.Length < 1)
            {
                if (secondVersionArray.Length < 1)
                {
                    return 0;
                }

                return -1;
            }

            if (secondVersionArray.Length < 1)
            {
                return 1;
            }

            int first;
            int.TryParse(firstVersionArray[0], out first);

            int second;
            int.TryParse(secondVersionArray[0], out second);

            int compareTo = first.CompareTo(second);
            if (compareTo > 0)
            {
                return compareTo;
            }

            if (compareTo == 0 && (firstVersionArray.Length > 1 || secondVersionArray.Length > 1))
            {
                string newVersionString = string.Join(versionSeparator, firstVersionArray.Skip(1).ToArray());
                string newCurrentVersionString = string.Join(versionSeparator, secondVersionArray.Skip(1).ToArray());
                return Compare(newVersionString, newCurrentVersionString);
            }

            return compareTo;
        }

        private static string GetFormattableString(object formattableObject)
        {
            var formattable = formattableObject as IFormattable;
            return formattable?.ToString(null, CultureInfo.InvariantCulture) ?? formattableObject.ToString();
        }
    }
}