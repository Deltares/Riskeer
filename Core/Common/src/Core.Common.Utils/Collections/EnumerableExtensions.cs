using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Utils.Collections
{
    public static class EnumerableExtensions
    {
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