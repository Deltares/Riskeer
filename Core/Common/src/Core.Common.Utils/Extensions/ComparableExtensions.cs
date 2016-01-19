using System;

namespace Core.Common.Utils.Extensions
{
    /// <summary>
    /// Class defines extension methods for <see cref="IComparable"/> objects.
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// Determines whether one object is greater then another.
        /// </summary>
        /// <param name="object1">The first object.</param>
        /// <param name="object2">The second object.</param>
        /// <returns>True if <paramref name="object1"/> is considered greater then <paramref name="object2"/>,
        /// false otherwise.</returns>
        /// <exception cref="ArgumentException">Object type of <paramref name="object1"/>
        /// is not the same as that of <paramref name="object2"/>.</exception>
        public static bool IsBigger(this IComparable object1, IComparable object2)
        {
            if (object1 == null)
            {
                return false; // Null not bigger then anything (or equal to null).
            }
            if (object2 == null)
            {
                return true; // Anything is greater then null.
            }

            return object1.CompareTo(object2) > 0;
        }

        /// <summary>
        /// Determines whether one object is smaller then another.
        /// </summary>
        /// <param name="object1">The first object.</param>
        /// <param name="object2">The second object.</param>
        /// <returns>True if <paramref name="object1"/> is considered smaller then <paramref name="object2"/>,
        /// false otherwise.</returns>
        /// <exception cref="ArgumentException">Object type of <paramref name="object1"/>
        /// is not the same as that of <paramref name="object2"/>.</exception>
        public static bool IsSmaller(this IComparable object1, IComparable object2)
        {
            if (object1 == null)
            {
                return object2 != null; // smaller than anything but null
            }

            return object1.CompareTo(object2) < 0;
        }

        /// <summary>
        /// Determines where one object is within the inclusive bounds of some range.
        /// </summary>
        /// <param name="value">Value to be checked.</param>
        /// <param name="limit1">First range value.</param>
        /// <param name="limit2">Second range value.</param>
        /// <returns>True if <paramref name="value"/> falls within the inclusive bounds, false otherwise.</returns>
        /// <exception cref="ArgumentException">Object type of <paramref name="value"/>
        /// is not the same as that of <paramref name="limit1"/> or <paramref name="limit2"/>.</exception>
        public static bool IsInRange(this IComparable value, IComparable limit1, IComparable limit2)
        {
            IComparable min;
            IComparable max;

            if (limit1.IsSmaller(limit2))
            {
                min = limit1;
                max = limit2;
            }
            else
            {
                min = limit2;
                max = limit1;
            }

            return (min.IsSmaller(value) && max.IsBigger(value)) || min.CompareTo(value) == 0 || max.CompareTo(value) == 0;
        }

        /// <summary>
        /// This method returns the clipped value of a value given an inclusive value range.
        /// </summary>
        /// <typeparam name="T">A comparable object type.</typeparam>
        /// <param name="value">The value to be clipped.</param>
        /// <param name="limit1">First range value.</param>
        /// <param name="limit2">Second range value.</param>
        /// <returns>The clipped value within the given validity range.</returns>
        /// <exception cref="ArgumentException">Object type of <paramref name="value"/>
        /// is not the same as that of <paramref name="limit1"/> or <paramref name="limit2"/>.</exception>
        public static T ClipValue<T>(this T value, T limit1, T limit2) where T : IComparable
        {
            T min;
            T max;

            if (limit1.IsSmaller(limit2))
            {
                min = limit1;
                max = limit2;
            }
            else
            {
                min = limit2;
                max = limit1;
            }

            if (value.IsSmaller(min))
            {
                return min;
            }
            if (value.IsBigger(max))
            {
                return max;
            }
            return value;
        }
    }
}