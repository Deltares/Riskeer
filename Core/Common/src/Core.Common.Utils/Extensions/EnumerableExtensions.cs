using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Utils.Extensions
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
        public static void ForEachElementDo<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}