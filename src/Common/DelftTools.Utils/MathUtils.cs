using System;

namespace DelftTools.Utils
{
    public static class MathUtils
    {
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
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            if (value.CompareTo(min) < 0)
            {
                return min;
            }

            return value;
        }
    }
}