using System;
using System.Collections;
using System.Collections.Generic;

namespace DelftTools.Utils.Collections.Extensions
{
    /// <summary>
    /// Extension methods for IList 
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds a range of items to the list
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list</typeparam>
        /// <param name="destination">The target list where item need to be added</param>
        /// <param name="collection">The items to be added</param>
        public static void AddRange<T>(this IList<T> destination, IEnumerable<T> collection)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (T item in collection)
                destination.Add(item);
        }

        /// <summary>
        /// Adds a range of items to the list but leaving null items out
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list</typeparam>
        /// <param name="destination">The target list where item need to be added</param>
        /// <param name="collection">The items to be added</param>
        public static void AddRangeLeavingNullElementsOut<T>(this IList<T> destination, IEnumerable<T> collection)
            where T : class
        {
            AddRangeConditionally(destination, collection, x => x != null);
        }

        /// <summary>
        /// Adds a range of items to the list conditionaly. ie. When the predicate for an item evaluates to true it will be added
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list</typeparam>
        /// <param name="destination">The target list where item need to be added</param>
        /// <param name="collection">The items to be added</param>
        /// <param name="predicate">The items predicate or requirement that needs to be true before the item can be added</param>
        public static void AddRangeConditionally<T>(this IList<T> destination, IEnumerable<T> collection,
                                                    Func<T, bool> predicate)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (T item in collection)
                if (predicate(item))
                    destination.Add(item);
        }

        public static int BinarySearch(this IList semiSortedList, object value)
        {
            ArrayList realValues = ArrayList.Adapter(semiSortedList);
            return realValues.BinarySearch(value);
        }

        /// <summary>
        /// Performs a binary search on the list for the given value. If binary search is unsuccesful, for example because the list is partially sorted, 
        /// not strictly sorted or not sorted at all, the method falls back to partial search based on the binary search results. If that also fails, it
        /// falls back to a full search.
        /// Should be used on lists that are typically sorted, but sometimes contain not stricly sortable objects.
        /// </summary>
        /// <param name="semiSortedList">The (hopefully sorted) list</param>
        /// <param name="value">The value to look for</param>
        /// <returns>The index of the value in the list. -1 if not found</returns>
        public static int BinaryHintedSearch(this IList semiSortedList, object value)
        {
            ArrayList realValues = ArrayList.Adapter(semiSortedList);
            int index = realValues.BinarySearch(value);
            if (index >= 0)
            {
                if (realValues[index].Equals(value)) //sanity check: binary search doesn't work well on weak comparables
                {
                    return index;
                }
                //else: continue below with partial and/or fullsearch
            }
            else
            {
                //Value not found, 'index' is the nearest index (bitwise complement) returned by binary search.
                //Normally we would abort here, but unfortunately there's an issue with for example NetworkLocation 
                //not being strictly comparable. So it still might be in the list! Resort to manual search? Use the
                //index hint or not? Questions questions..
                index = ~index; //binary search hint (of where to start looking)
            }

            if (value.GetType().IsValueType)
                return -1; //for value types at least we know that their sorting is not ambiguous, so if we haven't 
                           //found it yet, we're not going to find it

            //if we get here, we are in trouble.. This becomes extremely slow if the value is not in the list at all.
            //perhaps introduce an interface to indicate if you ever want to get here at all?

            //continue search the hard way, but search smart first
            int startIndex = index - 10;
            int endIndex = index + 10;
            var count = realValues.Count;
            startIndex = startIndex >= 0 ? startIndex : 0;
            endIndex = endIndex < count - 1 ? endIndex : count - 1;
            for (int j = startIndex; j <= endIndex; j++)
            {
                if (realValues[j].Equals(value))
                {
                    return j;
                }
            }
            
            //still not found, do a full search (bleh):
            for (int j = 0; j < count; j++)
            {
                if (j >= startIndex && j <= endIndex)
                    continue; //we already checked this range in the code above, so don't do that again

                if (realValues[j].Equals(value))
                {
                    return j;
                }
            }
            return -1;
        }

        public static void Move<T>(this IList<T> list, int index, int newIndex)
        {
            var old = list[index];
            list.RemoveAt(index);
            list.Insert(newIndex, old);
        }
    }
}
