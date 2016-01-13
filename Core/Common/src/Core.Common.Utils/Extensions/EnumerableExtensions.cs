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

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="OverflowException">The number of elements in source is larger than <see cref="int.MaxValue"/></exception>
        /// <seealso cref="Enumerable.Count{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
        public static int Count(this IEnumerable source)
        {
            return Enumerable.Count(source.Cast<object>());
        }
    }
}