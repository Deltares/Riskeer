// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;

namespace Ringtoets.Common.Data.TestUtil.Comparers
{
    /// <summary>
    /// Base class for determining whether two objects are clones.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public abstract class CloneComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return false;
            }

            if (ReferenceEquals(null, x) && ReferenceEquals(null, y))
            {
                return true;
            }

            if (ReferenceEquals(null, x))
            {
                return false;
            }

            if (ReferenceEquals(null, y))
            {
                return false;
            }

            return AreClones(x, y);
        }

        public int GetHashCode(T obj)
        {
            return -1;
        }

        /// <summary>
        /// Returns whether the provided objects are clones.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>True</c> when <paramref name="x"/> and <paramref name="y"/> are clones; <c>false</c> otherwise.</returns>
        protected abstract bool AreClones(T x, T y);
    }
}