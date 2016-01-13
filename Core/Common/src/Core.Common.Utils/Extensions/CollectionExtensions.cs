using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Utils.Extensions
{
    /// <summary>
    /// Class defining extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Remove all elements from a collection where a given check returns true.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The collection from which elements should be removed.</param>
        /// <param name="condition">The filtering method, that should return true if the
        /// given element should be removed from <paramref name="source"/>.</param>
        public static void RemoveAllWhere<T>(this ICollection<T> source, Func<T, bool> condition)
        {
            foreach (T item in source.Where(condition).ToArray())
            {
                source.Remove(item);
            }
        }
    }
}