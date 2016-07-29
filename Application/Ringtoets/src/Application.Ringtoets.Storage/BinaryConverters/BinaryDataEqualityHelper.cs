// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.BinaryConverters
{
    /// <summary>
    /// Class that helps comparing byte-arrays in a performance optimized manner.
    /// </summary>
    public static class BinaryDataEqualityHelper
    {
        /// <summary>
        /// Determines if two byte arrays are equal to each other.
        /// </summary>
        /// <param name="array1">The first array, cannot be <c>null</c>.</param>
        /// <param name="array2">The second array, cannot be <c>null</c>.</param>
        /// <returns><c>True</c> if the two arrays are equals, <c>false</c> otherwise.</returns>
        public static bool AreEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }
            // Note: Do not turn this into a linq query, as that is less performance optimal!
            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}