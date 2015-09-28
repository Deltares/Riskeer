using System;
using System.Collections.Generic;

namespace DelftTools.Utils
{
    public class MathUtils
    {

        /// <summary>
        /// Minimum value for a list of IComparable elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Min<T>(IEnumerable<T> values)
        {
            return Min<T>(values, Comparer<T>.Default);
        }

        private static T Min<T>(IEnumerable<T> values, IComparer<T> comparer)
        {
            bool first = true;
            T result = default(T);
            foreach (T value in values)
            {
                if (first)
                {
                    result = value;
                    first = false;
                }
                else
                {
                    if (comparer.Compare(result, value) > 0)
                    {
                        result = value;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Maximum value for a list of IComparable elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Max<T>(IEnumerable<T> values)
        {
            return Max<T>(values, Comparer<T>.Default);
        }

        private static T Max<T>(IEnumerable<T> values, IComparer<T> comparer)
        {
            bool first = true;
            T result = default(T);
            foreach (T value in values)
            {
                if (first)
                {
                    result = value;
                    first = false;
                }
                else
                {
                    if (comparer.Compare(result, value) < 0)
                    {
                        result = value;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// This method returns the clipped value of a value
        /// given a valid value range.
        /// </summary>
        /// <typeparam name="T">A comparible object.</typeparam>
        /// <param name="value">The value to be clipped.</param>
        /// <param name="min">Lowest allowed value (inclusive).</param>
        /// <param name="max">Highsest allowed value (inclusive).</param>
        /// <returns>The clipped value within the given validity range.</returns>
        public static T ClipValue<T>(T value, T min, T max) where T : IComparable
        {
            if (value.CompareTo(max) > 0) return max;
            if (value.CompareTo(min) < 0) return min;
            return value;
        }
    }
}