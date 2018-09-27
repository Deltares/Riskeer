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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Util.Extensions
{
    /// <summary>
    /// This class defines extension methods for <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Perform a certain action for each element in a sequence.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">A sequence that contains elements to be acted upon.</param>
        /// <param name="action">The action that should be performed on each element.</param>
        /// <remarks>Do not define an action that effect <see cref="source"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static void ForEachElementDo<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (T item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Checks whether the elements from <paramref name="source"/> are not unique using the <paramref name="keySelector"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <typeparam name="TKey">The type for the key.</typeparam>
        /// <param name="source">A sequence that contains elements to be acted upon.</param>
        /// <param name="keySelector">The key selector to validate uniqueness.</param>
        /// <returns><c>true</c> when duplicates are present in <see cref="source"/>; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static bool HasDuplicates<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return source.Select(keySelector).Count() != source.Select(keySelector).Distinct().Count();
        }

        /// <summary>
        /// Checks whether the elements from <paramref name="source"/> have more than one unique 
        /// value using the <paramref name="keySelector"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <typeparam name="TKey">The type for the key.</typeparam>
        /// <param name="source">A sequence that contains elements to be acted upon.</param>
        /// <param name="keySelector">The key selector to validate uniqueness.</param>
        /// <returns><c>true</c> when there is more than one unique value in <see cref="source"/>; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public static bool HasMultipleUniqueValues<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return source.Select(keySelector).Distinct().Count() > 1;
        }
    }
}