using System;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Class that contains extension methods for <see cref="double"/> to convert them to
    /// other value types.
    /// </summary>
    public static class DoubleConversionExtensions
    {
        /// <summary>
        /// Converts a <see cref="double"/> into a <see cref="decimal"/> that can be null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>null</c> when <paramref name="value"/> is <see cref="double.NaN"/>,
        /// or the decimal representation of the value otherwise.</returns>
        public static decimal? ToNullableDecimal(this double value)
        {
            if (double.IsNaN(value))
            {
                return null;
            }
            return Convert.ToDecimal(value);
        }

        /// <summary>
        /// Converts a nullable <see cref="double"/> into a <see cref="decimal"/> that can be null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>null</c> when <paramref name="value"/> is <c>null</c>,
        /// or the decimal representation of the value otherwise.</returns>
        public static decimal? ToNullableDecimal(this double? value)
        {
            if (value.HasValue)
            {
                return Convert.ToDecimal(value);
            }
            return null;
        }
    }
}